#  CSharp Docs From XML Comments

## Description

The program enables the generation of Markdown documentation by utilizing a supplied DLL. It analyzes the chosen class and its related components (such as other classes, types, enums), and produces Markdown files for each of them, incorporating XML comments from the code (e.g summaries).

## Why?

From a need for a simple way to automate the generation of wikis based on XML comments.

## Quick start

- Download project, build the code and run. The best way  is to select a DLL that is located in a folder containing all of its dependencies, as some of them may need to be accessed during the analysis phase, e.g. directly from build folder.
- Documentation generation must be enabled - refer to [this article](https://learn.microsoft.com/en-us/visualstudio/ide/reference/generate-xml-documentation-comments?view=vs-2022).

## Contributing

If you wish to make a contribution, please fork the repository and create a pull request for the `main` branch.
