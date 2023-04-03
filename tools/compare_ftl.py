import os
import sys
import glob
from fluent.syntax import parse
from fluent.syntax.ast import Term, Message
import frontmatter
import json


def compare_ftl_files(reference_file, target_files):
    issues = []

    with open(reference_file, 'r') as f:
        reference_content = f.read()
        reference_ast = parse(reference_content)

    reference_entries = {entry.id.name: entry for entry in reference_ast.body if isinstance(entry, (Message, Term))}

    for target_file in target_files:
        with open(target_file, 'r') as f:
            target_content = f.read()
            target_ast = parse(target_content)

        for entry in target_ast.body:
            if isinstance(entry, (Message, Term)):
                entry.file = target_file

        target_entries = {entry.id.name: entry for entry in target_ast.body if isinstance(entry, (Message, Term))}
        missing_ids = set(reference_entries.keys()) - set(target_entries.keys())

        if missing_ids:
            issues.append({"target_file": target_file, "missing_ids": missing_ids})

        for ref_id, ref_entry in reference_entries.items():
            if ref_id in target_entries:
                target_entry = target_entries[ref_id]
                missing_attributes = []

                for ref_attr in ref_entry.attributes:
                    attr_name = ref_attr.id.name
                    target_attr = next((attr for attr in target_entry.attributes if attr.id.name == attr_name), None)

                    if target_attr is None:
                        missing_attributes.append(attr_name)

                if missing_attributes:
                    issues.append({"target_file": target_file, "entry_id": ref_id, "missing_attributes": missing_attributes})
                else:
                    # Add empty `missing_attributes` key to avoid issues with create_issue function
                    issues.append({"file": target_file, "entry_id": ref_id, "missing_attributes": []})

    return issues


    def get_existing_issues(repo):
    # Get all issues in the repository with the translation label
    query = f"repo:{repo} label:translation is:issue state:open"
    response = requests.get(f"https://api.github.com/search/issues?q={query}")

    if response.status_code != 200:
        raise ValueError(f"Failed to fetch issues: {response.content}")

    return {issue["title"]: issue for issue in response.json()["items"]}


def create_issue(issue, repo, pr_number):
    file_name = f"missing_translations_issue_{issue['target_file'].split('/')[-1].split('.')[0]}.md"

    existing_issues = get_existing_issues(repo)
    issue_title = f"Missing Translations Detected for {issue['target_file'].split('/')[-1]}"

    if issue_title in existing_issues:
        print(f"Issue already exists: {existing_issues[issue_title]['html_url']}")
        return False

    with open(file_name, "w") as issue_file:
        issue_file.write("---\ntitle: Missing Translations Detected for ")
        issue_file.write(issue['target_file'].split('/')[-1])
        issue_file.write("\nlabels: translation, help wanted\n---\n\n")
        issue_file.write("The following translations are missing for ")
        issue_file.write(issue['target_file'].split('/')[-1])
        issue_file.write(":\n\n")

        issue_file.write("| ID | Missing Attributes | Reference Translation |\n")
        issue_file.write("|----|--------------------|-----------------------|\n")

        if "missing_ids" in issue:
            for missing_id in issue["missing_ids"]:
                ref_entry = reference_entries[missing_id]
                ref_translation = ref_entry.value.elements[0].value if ref_entry.value else ""
                issue_file.write(f"| `{missing_id}` | | {ref_translation} |\n")

    return True


def validate_reference_file(file_path):
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"The reference file '{file_path}' does not exist.")
    
    if os.path.getsize(file_path) == 0:
        raise ValueError(f"The reference file '{file_path}' is empty.")


def main():
    reference_file = os.environ["REFERENCE_FTL"]
    target_files = [f for f in glob.glob("**/*.ftl", recursive=True) if f != reference_file and "Tests" not in f]

    issues = compare_ftl_files(reference_file, target_files)

    if issues:
        created_issues = [create_issue(issue, os.environ["GITHUB_REPOSITORY"], os.environ["NUMBER"]) for issue in issues]
        if any(created_issues):
            sys.exit(1)
        else:
            sys.exit(0)
    else:
        sys.exit(0)


if __name__ == "__main__":
    main()
