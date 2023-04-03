import os
import sys
import glob
from fluent.syntax import parse
from fluent.syntax.ast import Term, Message
import frontmatter
import json
from github import Github, GithubException


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


def create_issue(issue, repo, pr_number):
    file_name = f"missing_translations_issue_{issue['target_file'].split('/')[-1].split('.')[0]}.md"
    created_issue = False

    # Check if an issue already exists with the same title
    existing_issues = repo.get_issues(state="all")
    for existing_issue in existing_issues:
        if existing_issue.title == f"Missing Translations Detected for {issue['target_file'].split('/')[-1]}":
            print(f"Skipping issue creation for {issue['target_file']} since an issue already exists with the same title.")
            created_issue = True
            break

    # Create a new issue if one doesn't exist already
    if not created_issue:
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

            if "missing_attributes" in issue:
                for entry in issue["missing_attributes"]:
                    for missing_attr in entry["missing_attributes"]:
                        ref_entry = reference_entries[entry['entry_id']]
                        ref_attr = next((attr for attr in ref_entry.attributes if attr.id.name == missing_attr), None)
                        ref_translation = ref_attr.value.elements[0].value if ref_attr.value else ""
                        issue_file.write(f"| `{entry['entry_id']}` | `{missing_attr}` | {ref_translation} |\n")

            issue_file.write("\n")
            issue_file.write(f"[Download {issue['target_file'].split('/')[-1]}](https://github.com/{repo.owner.login}/{repo.name}/blob/{pr.head.ref}/{issue['target_file']})\n")
            issue_file.write(f"[Download {reference_file.split('/')[-1]}](https://github.com/{repo.owner.login}/{repo.name}/blob/{pr.head.ref}/{reference_file})\n")

            issue_file.write(f"\nCommit: [{pr.head.sha[:7]}](https://github.com/{repo.owner.login}/{repo.name}/commit/{pr.head.sha})\n")
            issue_file.write(f"Pull Request: [#{pr_number}](https://github.com/{repo.owner.login}/{repo.name}/pull/{pr_number})\n")

        repo.create_issue(title=f"Missing Translations Detected for {issue['target_file'].split('/')[-1]}", body=open(file_name).read())
        created_issue = True

    return created_issue



def validate_reference_file(file_path):
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"The reference file '{file_path}' does not exist.")
    
    if os.path.getsize(file_path) == 0:
        raise ValueError(f"The reference file '{file_path}' is empty.")


def main():
    reference_file = os.environ["REFERENCE_FTL"]
    validate_reference_file(reference_file)
    target_files = [f for f in glob.glob("**/*.ftl", recursive=True) if f != reference_file and "Tests" not in f]

    issues = compare_ftl_files(reference_file, target_files)
    user_name, repo_name = os.environ["GITHUB_REPOSITORY"].split('/')

    existing_issues = []
    for issue in issues:
        # Check if issue already exists
        existing_issue = get_existing_issue(issue, repo_name)
        if existing_issue:
            existing_issues.append(existing_issue)
        else:
            create_issue(issue, repo_name)

    if existing_issues:
        print("Some issues already exist. Please check the following:")
        for issue in existing_issues:
            print(issue["html_url"])
        sys.exit(1)
    else:
        sys.exit(0)



if __name__ == "__main__":
    main()
