# Access Control Service with local STS #

## Hands-on Lab ##

### Introduction ###

Hands-On Labs are sets of step-by-step guides that are designed to help you learn how to use key Windows Azure services and features.  Each Lab provides instructions to guide you through the process of developing a complete application.

In this Hands-On Lab, you will learn how to:

- Use the portal to add business identity providers through their metadata documents

- Use the portal to establish claims transformation rules for normalizing the user's attributes

- Do all of the above via management API

- Outsource authentication of a web application to ACS

> **Note:** You can download the latest build of the Windows Azure Training Kit which includes a tested version of this HOL from here: http://www.microsoft.com/download/en/details.aspx?id=8396.

### Repository Structure ###

In the **root** folder of this repository you will find the Hands-On Lab (HOL) document, **HOL.md**. Before beginning with the HOL exercises, make sure you have followed all the required steps indicated at the setup section of the HOL document. 

In the **Source** folder you will find the source code of each of the exercises, as well as the assets and setup scripts. Throughout the HOL you will be instructed to open and explore the different solutions from the source folder. It is typically comprised of the following subfolders:

- **Assets:** This folder contains files that are used throughout the exercises.
- **_Exercise Name_:** Each exercise that requires a programming solution has its own code folder.
  - **Begin:** The begin solution is the initial incomplete solution that you will finish by following the steps of the corresponding exercise.
	- **End:** The end solution is the final result you will achieve at the end of an exercise.
- **Setup:** This folder contains the dependency files and the setup scripts necessary to initialize specific configurations of the lab, being its execution is required in the majority of the Hands-on Labs.

### Get Started ###

In order to run the solutions of the exercises provided by this lab you will first need configure your environment and install any necessary prerequisites such as runtimes, components, or libraries. For your ease, you can download and run the dependency checker [here] (http://contentinstaller.blob.core.windows.net/dependency-checker/DC.exe) to automatically check and install all the requirements.  Each lab also includes setup instructions for getting started.

### Contributing to the Repository ###

If you find any issues or opportunties for improving this hands-on lab, fix them!  Feel free to contribute to this project by [forking](http://help.github.com/fork-a-repo/) this repository and make changes to the content.  Once you've made your changes, share them back with the community by sending a pull request. Please see [How to send pull requests](http://help.github.com/send-pull-requests/) for more information about contributing to github projects.

### Reporting Issues ###

If you find any issues with this hands-on lab that you can't fix, feel free to report them in the [issues](https://github.com/microsoft-dpe/HOL-BuildingASocialGameWithWindowsAzure/issues) section of this repository.