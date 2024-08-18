#  CSharp Docs From XML Comments

## Description

The program enables the generation of Markdown documentation by utilizing a supplied DLL. It analyzes the chosen class and its related components (such as other classes, types, enums), and produces Markdown files for each of them, incorporating XML comments from the code (e.g summaries).

## Simple to use

1. Just select DLL

![image](https://gitlab.com/Phoenix510/csharp-docs-from-xml-comments/-/wikis/uploads/58966bbc650218d8bcb3a9e8511ceb37/image.png){width=800}

2. Choose class

![image](https://gitlab.com/Phoenix510/csharp-docs-from-xml-comments/-/wikis/uploads/1e5fa8454163b9a84d83ddf9e4c9f305/image.png){width=800}

3. Select items from which files are supposed to be generated

![image](https://gitlab.com/Phoenix510/csharp-docs-from-xml-comments/-/wikis/uploads/989317aa2d1310f3da52899ce1283a3b/image.png){width=800}

4. Generate 

![image](https://gitlab.com/Phoenix510/csharp-docs-from-xml-comments/-/wikis/uploads/9b78c1eb0728a6faf0142454778d33bb/image.png){width=800}

## Highly customizable

The application is highly customizable, allowing to disable namespace-like generation if you prefer a flat structure (all files in one folder), disable generation of files for nested dependencies (related classes/enums), and exclude summaries of properties/containers from generated files.

![image](https://gitlab.com/Phoenix510/csharp-docs-from-xml-comments/-/wikis/uploads/42f98fa54c0a72f3760e30295d959b27/image.png){width=800}

## Custom structure

There is an option to set a custom output structure, allowing alter namespaces that will be presented later as folders.

![image](https://gitlab.com/Phoenix510/csharp-docs-from-xml-comments/-/wikis/uploads/00a04d5192f79d2f3d05161ddb783802/image.png){width=707 height=396}

## Why?

From a need for a simple way to automate the generation of wikis based on XML comments.

## Quick start

- Download project, build the code and run. The best way  is to select a DLL that is located in a folder containing all of its dependencies, as some of them may need to be accessed during the analysis phase, e.g. directly from build folder.
- Documentation generation must be enabled - refer to [this article](https://learn.microsoft.com/en-us/visualstudio/ide/reference/generate-xml-documentation-comments?view=vs-2022).

## Contributing

If you wish to make a contribution, please fork the repository and create a pull request for the `main` branch.
