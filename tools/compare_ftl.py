import os
import sys
import glob
from fluent.syntax import parse
from fluent.syntax.ast import Message, Term, VariableReference
from fluent.syntax.parser import ParseError
from github import Github
import frontmatter


def compare_ftl_files(reference_file, target_files):
    issues = []

    with open(reference_file, 'r') as f:
        reference_content = f.read()
        reference_ast = parse(reference_content)

    reference_entries = {
        entry.id.name: entry
        for entry in reference_ast.body
        if isinstance(entry, (Message, Term))
    }

    for target_file in target_files:
        with open(target_file, 'r') as f:
            target_content = f.read()
            try:
                target_ast = parse(target_content)
            except ParseError as e:
                print(f"Error parsing {target_file}: {e}")
                continue

        target_ids = {entry.id.name for entry in target_ast.body if isinstance(entry, (Message, Term))}
        missing_ids = set(reference_entries.keys()) - target_ids

        if missing_ids:
            issues.append({"file": target_file, "missing_ids": missing_ids})

    return issues


def create_issue(issue):
    print(f"Creating issue for {issue['file']}...")
    token = os.environ["GH_TOKEN"]
    repo_name = os.environ["GITHUB_REPOSITORY"]
    branch_name = os.environ["GITHUB_HEAD_REF"]

    g = Github(token)
    repo = g.get_repo(repo_name)

    issue_title = f"Missing Translations Detected for {issue['file']}"
    issue_body = f"Missing translations in `{issue['file']}`:\n\n"
    issue_body += "| ID | Reference Value |\n|----|----------------|\n"

    for entry_id in issue["missing_ids"]:
        entry = reference_entries[entry_id]
        issue_body += f"| {entry_id} | {get_entry_value(entry)} |\n"

    issue_body += f"\n[Download {issue['file'].split('/')[-1]}](https://github.com/{repo_name}/blob/{branch_name}/{issue['file']})\n"

    created_issue = repo.create_issue(title=issue_title, body=issue_body, labels=["translation", "help wanted"])
    print(f"Issue created: {created_issue.html_url}")


def get_entry_value(entry):
    first_element = entry.value.elements[0]

    if isinstance(first_element, TextElement):
        return first_element.value
    elif isinstance(first_element, Placeable):
        selector = first_element.expression.selector

        if isinstance(selector, VariableReference):
            return f"{{{selector.name}}}"
        elif isinstance(selector, FunctionReference):
            return f"{selector.name}()"
    else:
        return "UNKNOWN"


def main():
    reference_file = os.environ["REFERENCE_FTL"]
    target_files = [
        f for f in glob.glob("**/*.ftl", recursive=True)
        if f != reference_file and "Tests" not in f
    ]

    print(f"Comparing {reference_file} against {len(target_files)} target files...")
    issues = compare_ftl_files(reference_file, target_files)

    if issues:
        print(f"Found {len(issues)} issues. Creating GitHub issues...")
        for issue in issues:
            create_issue(issue)
        sys.exit(1)
    else:
        print("No issues found.")
        sys.exit(0)


if __name__ == "__main__":
    main()
