## What is Transcelerator?

Transcelerator™ helps Bible translation teams translate comprehension checking questions in order to prepare to test the quality and clarity of the vernacular Scripture translation. Transcelerator functions both as a plugin in Paratext and as a platform.Bible extension.

## How do I get set up to develop Transcelerator as a Paratext plugin?

* To get set up to use and build Transcelerator, you will need to download [Paratext](https://paratext.org/).
* There are some post-build commands that will attempt to copy the plugin files to a (potentially) useful location if you are building using the "Debug - Copy to Paratext" or "Release - Copy to Paratext" configurations. Depending on your individual needs, you might want to tweak the details, but if you do, please don't include your tweaks in a pull request. When building the "Release - Copy to Paratext" configuration for the first time, it will attempt to set up the necessary directory structure by copying files into the default install location(s) for Paratext. You need to be running Visual Studio as an administrator, or the robocopy command(s) will fail.
* To learn to use Transcelerator, see the [wiki](https://github.com/sillsdev/Transcelerator/wiki) and the [Tutorial](https://software.sil.org/transcelerator/features/tutorial/) page on the Transcelerator website.
* Unit tests depend on [NUnit](https://nunit.org/). I recommend using [Jet Brains Resharper](https://www.jetbrains.com/resharper/), which has built-in test running capabilities.
* The [Paratext Demo Plugins](https://github.com/ubsicap/paratext_demo_plugins) repository has more advanced information about the Paratext plugin architecture, which will explain more about how to build a plugin like Transcelerator.

## How do I get set up to develop Transcelerator as a Platform.Bible extension?

This is still under development. Transcelerator cannot yet be used in Platform.Bible

### Outline of file and folder structure

Note: This is based on the paranext-extension-template. As the extension framework is developed, changes to the template should be merged into this repository. See the [paranext-extension-template wiki](https://github.com/paranext/paranext-extension-template/wiki/Extension-Anatomy) for details about the anatomy of an extension (folder structure, etc.).

### To install

#### Configure paths to `paranext-core` repo

In order to interact with `paranext-core`, you must point `package.json` to your installed `paranext-core` repository:

1. Follow the instructions to install [`paranext-core`](https://github.com/paranext/paranext-core#developer-install). We recommend you clone `paranext-core` in the same parent directory in which you cloned this repository so you do not have to reconfigure paths to `paranext-core`.
2. If you cloned `paranext-core` anywhere other than in the same parent directory in which you cloned this repository, update the paths to `paranext-core` in this repository's `package.json` to point to the correct `paranext-core` directory.

#### Install dependencies

1. Follow the instructions to install [`paranext-core`](https://github.com/paranext/paranext-core#developer-install).
2. In this repo, run `npm install` to install local and published dependencies

### To run

#### Running Platform.Bible with the Transcelerator extension

To run Platform.Bible with the Transcelerator extension:

`npm start`

Note: The built extension will be in the `dist` folder. In order for Platform.Bible to run the Transcelerator extension, you must provide the directory to the built extension to Platform.Bible via a command-line argument. This command-line argument is already provided in this `package.json`'s `start` script. If you want to start Platform.Bible and use the Transcelerator extension any other way, you must provide this command-line argument or put the `dist` folder into Platform.Bible's `extensions` folder.

#### Building the Transcelerator extension independently

To watch extension files (in `src`) for changes:

`npm run watch`

To build the Transcelerator extension once:

`npm run build`

### To package for distribution

To package the Transcelerator extension into a zip file for distribution:

`npm run package`

## To update

The `paranext-extension-template` will be updated regularly and will sometimes receive updates that help with breaking changes. The Transcelerator extension should be updated periodically by merging in the latest template updates. You can do so by following [these instructions](https://github.com/paranext/paranext-extension-template/wiki/Merging-Template-Changes-into-Your-Extension).

### Special features in this project

This project has special features and specific configuration to make building an extension for Platform.Bible easier. See [Special features of `paranext-multi-extension-template`](https://github.com/paranext/paranext-multi-extension-template#special-features-of-the-template) for information on these special features.

## Contribution guidelines

* Contributions that further the goals of this project are welcomed.
* If your pull request does not contain passing unit tests that demonstrate the value and quality of your contribution, it will reduce the likelihood that your submission will be accepted.
* If you contact me in advance of making any changes, I can probably give you some guidance and let you know if I think your proposed changes are on track.
* Please visit the [TXL Jira page](https://jira.sil.org/secure/Dashboard.jspa?selectPageId=10760) to see the outstanding Jira issues.

## Who do I talk to?

* Technical lead: [Tom Bogle](mailto:Transcelerator_feedback@sil.org)
* To discuss the concepts and principles behind Transcelerator (e.g., if you disagree with the idea of using "canned" questions as basis for checking comprehension), contact your translation consultant and/or local translation department representative.

