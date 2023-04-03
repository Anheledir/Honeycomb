import os
import sys
import glob
from fluent.syntax import parse
from fluent.syntax.ast import Term, Message
import frontmatter
import json


def compare_ftl_files(reference_file, target_files):
    issues = {}

    with open(reference_file, 'r') as f:
        reference_content = f.read()
        reference_ast = parse(reference_content)

    reference_entries = {entry.id.name: entry for entry in reference_ast.body if isinstance(entry, (Message, Term))}

    for target_file in target_files:
        with open(target_file, 'r') as f:
            target_content = f.read()
            target_ast = parse(target_content)

        target_entries = {entry.id.name: entry for entry in target_ast.body if isinstance(entry, (Message, Term))}
        missing_ids = set(reference_entries.keys()) - set(target_entries.keys())

        if missing_ids:
            issues[target_file] = {"missing_ids": missing_ids, "target_file": target_file}

        for ref_id, ref_entry in reference_entries.items():
            if ref_id in target_entries:
                target_entry = target_entries[ref_id]
                missing_attributes = []

                for ref_attr in ref_entry.attributes:
                    attr_name = ref_attr.id.name
                    target_attr = next((attr for attr in target_entry.attributes if attr.id.name == attr_name), None)

                    if target_attr is None:
                        if target_file not in issues:
                            issues[target_file] = {}
                        if "missing_attributes" not in issues[target_file]:
                            issues[target_file]["missing_attributes"] = {}
                        if ref_id not in issues[target_file]["missing_attributes"]:
                            issues[target_file]["missing_attributes"][ref_id] = []

                        issues[target_file]["missing_attributes"][ref_id].append(attr_name)
                        issues[target_file]["target_file"] = target_file

    return reference_entries, issues


def create_issue(issue):
    file_name = f"missing_translations_issue_{issue['target_file'].split('/')[-1].split('.')[0]}.md"
    user_name, repo_name = os.environ["GITHUB_REPOSITORY"].split('/')
    branch_name = os.environ["GITHUB_HEAD_REF"]
    commit_sha = os.environ["GITHUB_SHA"]

    with open(os.environ["GITHUB_EVENT_PATH"], "r") as event_file:
        event_data = json.load(event_file)
    pr_number = event_data["number"]

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
            for missing_id, missing_attrs in issue["missing_attributes"].items():
                ref_entry = reference_entries[missing_id]
                ref_translation = ref_entry.value.elements[0].value if ref_entry.value else ""
                for missing_attr in missing_attrs:
                    issue_file.write(f"| `{missing_id}` | `{missing_attr}` | {ref_translation} |\n")

        issue_file.write("\n")
        issue_file.write(f"[Download {issue['target_file'].split('/')[-1]}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{issue['target_file']})\n")
        issue_file.write(f"[Download {reference_file.split('/')[-1]}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{reference_file})\n")
        
        issue_file.write(f"\nCommit: [{commit_sha[:7]}](https://github.com/{user_name}/{repo_name}/commit/{commit_sha})\n")
        issue_file.write(f"Pull Request: [#{pr_number}](https://github.com/{user_name}/{repo_name}/pull/{pr_number})\n")

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
        created_issues = [create_issue(issue) for issue in issues]
        if any(created_issues):
            sys.exit(1)
        else:
            sys.exit(0)
    else:
        sys.exit(0)



if __name__ == "__main__":
    main()
