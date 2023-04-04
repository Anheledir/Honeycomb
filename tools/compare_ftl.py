import os
import sys
import glob
from fluent.syntax import parse
from fluent.syntax.ast import Term, Message, Attribute, VariableReference, TextElement, Placeable, SelectExpression
from github import Github
import frontmatter

def get_entry_value(entry):
    if not entry.value:
        return "N/A"

    first_element = entry.value.elements[0]

    if isinstance(first_element, TextElement):
        return first_element.value
    elif isinstance(first_element, Placeable):
        if isinstance(first_element.expression, SelectExpression):
            selector = first_element.expression.selector
            if isinstance(selector, VariableReference):
                return f"{{{selector.name}}}"
            else:
                return "N/A"
        else:
            return "N/A"
    else:
        return "N/A"

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

        target_entries = {entry.id.name: entry for entry in target_ast.body if isinstance(entry, (Message, Term))}
        missing_entry_ids = set(reference_entries.keys()) - set(target_entries.keys())

        print(f"Comparing {reference_file} with {target_file}")
        print(f"Missing entry IDs: {missing_entry_ids}")

        missing_attributes = {}
        for entry_id in target_entries:
            if entry_id in reference_entries:
                reference_attributes = {attribute.id.name for attribute in reference_entries[entry_id].attributes}
                target_attributes = {attribute.id.name for attribute in target_entries[entry_id].attributes}
                missing_attr_ids = reference_attributes - target_attributes
                if missing_attr_ids:
                    missing_attributes[entry_id] = missing_attr_ids
                    print(f"Missing attributes for '{entry_id}': {missing_attr_ids}")

        if missing_entry_ids or missing_attributes:
            issues.append({"file": target_file, "missing_entry_ids": missing_entry_ids, "missing_attributes": missing_attributes})

    return issues


def create_issue(issue, reference_entries):
    user_name = os.environ["GITHUB_REPOSITORY"].split('/')[0]
    repo_name = os.environ["GITHUB_REPOSITORY"].split('/')[1]
    branch_name = os.environ["GITHUB_REF"].split('/')[-1]

    g = Github(os.environ["GITHUB_TOKEN"])
    repo = g.get_repo(f"{user_name}/{repo_name}")

    for entry_id in issue["missing_entry_ids"]:
        entry = reference_entries.get(entry_id)
        title = f"Missing translation for '{entry_id}' in {issue['file']}"
        body = f"**File:** [{issue['file']}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{issue['file']})\n\n"
        body += f"**Reference:** [{reference_file.split('/')[-1]}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{reference_file})\n\n"
        body += "| ID | Reference value |\n"
        body += "| -- | --------------- |\n"
        body += f"| {entry_id} | {get_entry_value(entry)} |\n"
        labels = ["translation", "help wanted"]
        repo.create_issue(title=title, body=body, labels=labels)

    for entry_id, missing_attr_ids in issue["missing_attributes"].items():
        entry = reference_entries[entry_id]
        title = f"Missing attributes for '{entry_id}' in {issue['file']}"
        body = f"**File:** [{issue['file']}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{issue['file']})\n\n"
        body += f"**Reference:** [{reference_file.split('/')[-1]}](https://github.com/{user_name}/{repo_name}/blob/{branch_name}/{reference_file})\n\n"
        body += "| ID | Attribute | Reference value |\n"
        body += "| -- | --------- | --------------- |\n"
        for missing_attr_id in missing_attr_ids:
            attr_value = get_entry_value(entry.attributes[entry_id][missing_attr_id])
            body += f"| {entry_id} | {missing_attr_id} | {attr_value} |\n"
        labels = ["translation", "help wanted"]
        repo.create_issue(title=title, body=body, labels=labels)



def main():
    reference_file = os.environ["REFERENCE_FTL"]
    target_files = [
        f for f in glob.glob("**/*.ftl", recursive=True)
        if f != reference_file and "Tests" not in f
    ]

    print(f"Comparing {reference_file} against {len(target_files)} target files...")
    issues = compare_ftl_files(reference_file, target_files)

    with open(reference_file, 'r') as f:
        reference_content = f.read()
        reference_ast = parse(reference_content)
    reference_entries = {entry.id.name: entry for entry in reference_ast.body if isinstance(entry, (Message, Term))}

    if issues:
        print(f"Found {len(issues)} issues. Creating GitHub issues...")
        for issue in issues:
            create_issue(issue, reference_entries)
        sys.exit(0)
    else:
        print("No issues found.")
        sys.exit(0)



if __name__ == "__main__":
    main()
