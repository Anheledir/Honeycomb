import os
import sys
import glob
import json
from fluent.syntax import parse
from fluent.syntax.ast import Term, Message
from fluent.syntax.ast import Placeable
from fluent.syntax.ast import TextElement, StringLiteral, SelectExpression
from fluent.syntax.ast import VariableReference
import frontmatter

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

        target_ids = {entry.id.name for entry in target_ast.body if isinstance(entry, (Message, Term))}
        missing_ids = set(reference_entries.keys()) - target_ids

        if missing_ids:
            issues.append({"file": target_file, "missing_entries": {entry_id: reference_entries[entry_id] for entry_id in missing_ids}})

    return issues


def create_issue(issue, reference_file):
    def get_entry_value(entry):
        first_element = entry.value.elements[0]
        if isinstance(first_element, TextElement):
            return first_element.value
        elif isinstance(first_element, StringLiteral):
            return first_element.value
        elif isinstance(first_element, Placeable) and isinstance(first_element.expression, SelectExpression):
            selector = first_element.expression.selector
            if isinstance(selector, VariableReference):
                return f"{{{selector.id.name}}}"
            else:
                return f"{{{selector.name}}}"
        else:
            return "<complex value>"


    file_name = f"missing_translations_issue_{issue['file'].split('/')[-1].split('.')[0]}.md"
    user_name, repo_name = os.environ["GITHUB_REPOSITORY"].split('/')
    branch_name = os.environ["GITHUB_HEAD_REF"]
    commit_sha = os.environ["GITHUB_SHA"]

    with open(os.environ["GITHUB_EVENT_PATH"], "r") as event_file:
        event_data = json.load(event_file)
    pr_number = event_data["number"]

    with open(file_name, "w") as issue_file:
        issue_file.write("---\ntitle: Missing Translations Detected\nlabels: translation, help wanted\n---\n\n")
        issue_file.write(f"Missing translations in `{issue['file']}`:\n\n")
        issue_file.write("| ID | Reference Translation |\n")
        issue_file.write("|----|-----------------------|\n")

        for entry_id, entry in issue["missing_entries"].items():
            issue_file.write(f"| {entry_id} | {get_entry_value(entry)} |\n")

        issue_file.write("\n")
        issue_file.write(f"[Download {issue['file'].split('/')[-1]}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{issue['file']})\n")
        issue_file.write(f"[Download {reference_file.split('/')[-1]}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{reference_file})\n")
        issue_file.write(f"\nCommit: [{commit_sha[:7]}](https://github.com/{user_name}/{repo_name}/commit/{commit_sha})\n")
        issue_file.write(f"Pull Request: [#{pr_number}](https://github.com/{user_name}/{repo_name}/pull/{pr_number})\n")

    return True


def main():
    reference_file = os.environ["REFERENCE_FTL"]
    target_files = [f for f in glob.glob("**/*.ftl", recursive=True) if f != reference_file and "Tests" not in f.split(os.sep)]

    issues = compare_ftl_files(reference_file, target_files)

    if issues:
        for issue in issues:
            create_issue(issue, reference_file)
        sys.exit(0)
    else:
        sys.exit(0)

if __name__ == "__main__":
    main()