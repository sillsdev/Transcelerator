## What is Transcelerator?


* Transcelerator helps Bible translation teams translate comprehension checking questions in order to prepare to test the quality and clarity of the vernacular Scripture translation. Transcelerator functions both as a plugin in Paratext and as a platform.Bible extension.

## How do I get set up to develop Transcelerator as a Paratext plugin?

* To get set up to use and build Transcelerator, you will need to download [Paratext](https://paratext.org/).
* There are some post-build commands that will attempt to copy the plugin files to a (potentially) useful location if you are building using the "Debug - Copy to Paratext" or "Release - Copy to Paratext" configurations. Depending on your individual needs, you might want to tweak the details, but if you do, please don't include your tweaks in a pull request. When building the "Release - Copy to Paratext" configuration for the first time, it will attempt to set up the necessary directory structure by copying files into the default install location(s) for Paratext. You need to be running Visual Studio as an administrator, or the robocopy command(s) will fail.
* To learn to use Transcelerator, see the [wiki](https://github.com/sillsdev/Transcelerator/wiki) and the [Tutorial](https://software.sil.org/transcelerator/features/tutorial/) page on the Transcelerator website.
* Unit tests depend on [NUnit](https://nunit.org/). I recommend using [Jet Brains Resharper](https://www.jetbrains.com/resharper/), which has built-in test running capabilities.
* The [Paratext Demo Plugins](https://github.com/ubsicap/paratext_demo_plugins) repository has more advanced information about the Paratext plugin architecture, which will explain more about how to build a plugin like Transcelerator.

## How do I get set up to develop Transcelerator as a platform.Bible (aka, "Paranext") extension? 

### Outline of file and folder structure

Note: This is based on the paranext-extension-template. As the extension framework is developed, changes to the template should be merged into this repository.

- `package.json` contains information about the Transcelerator extension npm package. It is required for Paranext to use the extension properly. It is copied into the build folder
- `src` contains the source code for the Transcelerator extension
  - `src/main.ts` is the main entry file for the Transcelerator extension
  - `src/types/transcelerator.d.ts` is the Transcelerator extension's types file that defines how other extensions can use this extension through the `papi`. It is copied into the build folder
  - `*.web-view.tsx` files will be treated as React WebViews
  - `*.web-view.html` files are a conventional way to provide HTML WebViews (no special functionality)
- `public` contains static files that are copied into the build folder
  - `public/manifest.json` is the manifest file that defines the Transcelerator extension and important properties for Paranext
  - `public/package.json` defines the npm package for the Transcelerator extension and is required for Paranext to use it appropriately
  - `public/assets` contains asset files the Transcelerator extension and its WebViews can retrieve using the `papi-extension:` protocol
- `dist` is a generated folder containing the built extension files
- `release` is a generated folder containing a zip of the built Transcelerator extension files

### To install

#### Configure paths to `paranext-core` repo

In order to interact with `paranext-core`, you must point `package.json` to your installed `paranext-core` repository:

1. Follow the instructions to install [`paranext-core`](https://github.com/paranext/paranext-core#developer-install). We recommend you clone `paranext-core` in the same parent directory in which you cloned this repository so you do not have to reconfigure paths to `paranext-core`.
2. If you cloned `paranext-core` anywhere other than in the same parent directory in which you cloned this repository, update the paths to `paranext-core` in this repository's `package.json` to point to the correct `paranext-core` directory.

#### Install dependencies

Run `npm install` to install local and published dependencies

### To run

#### Running Paranext with the Transcelerator extension

To run Paranext with the Transcelerator extension:

`npm start`

Note: The built extension will be in the `dist` folder. In order for Paranext to run the Transcelerator extension, you must provide the directory to the built extension to Paranext via a command-line argument. This command-line argument is already provided in this `package.json`'s `start` script. If you want to start Paranext and use the Transcelerator extension any other way, you must provide this command-line argument or put the `dist` folder into Paranext's `extensions` folder.

#### Building the Transcelerator extension independently

To watch extension files (in `src`) for changes:

`npm run watch`

To build the Transcelerator extension once:

`npm run build`

### To package for distribution

To package the Transcelerator extension into a zip file for distribution:

`npm run package`

## To update

The `paranext-extension-template` will be updated regularly, and will sometimes receive updates that help with breaking changes on `paranext-core`. So we recommend you periodically update the Transcelerator extension by merging in the latest template updates. You can do so by following [these instructions](https://github.com/paranext/paranext-extension-template/wiki/Merging-Template-Changes-into-Your-Extension).

### Special features of the template

The paranext-extension-template has special features and specific configuration to make building an extension for Paranext easier. Following are a few important notes:

#### React WebView files - `.web-view.tsx`

Paranext WebViews must be treated differently than other code, so this template makes doing that simpler:

- WebView code must be bundled and can only import specific packages provided by Paranext (see `externals` in `webpack.config.base.ts`), so this template bundles React WebViews before bundling the main extension file to support this requirement. The template discovers and bundles files that end with `.web-view.tsx` in this way.
  - Note: while watching for changes, if you add a new `.web-view.tsx` file, you must either restart webpack or make a nominal change and save in an existing `.web-view.tsx` file for webpack to discover and bundle this new file.
- WebView code and styles must be provided to the `papi` as strings, so you can import WebView files with [`?inline`](#special-imports) after the file path to import the file as a string.

#### Special imports

- Adding `?inline` to the end of a file import causes that file to be imported as a string after being transformed by webpack loaders but before bundling dependencies (except if that file is a React WebView file, in which case dependencies will be bundled). The contents of the file will be on the file's default export.
  - Ex: `import myFile from './file-path?inline`
- Adding `?raw` to the end of a file import treats a file the same way as `?inline` except that it will be imported directly without being transformed by webpack.

#### Misc features

- Paranext extension code must be bundled all together in one file, so webpack bundles all the code together into one main extension file.
- Paranext extensions can interact with other extensions, but they cannot import and export like in a normal Node environment. Instead, they interact through the `papi`. As such, the `src/types` folder contains this extension's declarations file that tells other extensions how to interact with it through the `papi`.

#### Two-step webpack build

This extension is built by webpack (`webpack.config.ts`) in two steps: a WebView bundling step and a main bundling step:

##### Build 1: TypeScript WebView bundling

Webpack (`./webpack/webpack.config.web-view.ts`) prepares TypeScript WebViews for use and outputs them into temporary build folders adjacent to the WebView files:

- Formats WebViews to match how they should look to work in Paranext
- Transpiles React/TypeScript WebViews into JavaScript
- Bundles dependencies into the WebViews
- Embeds Sourcemaps into the WebViews inline

##### Build 2: Main and final bundling

Webpack (`./webpack/webpack.config.main.ts`) prepares the main extension file and bundles the extension together into the `dist` folder:

- Transpiles the main TypeScript file and its imported modules into JavaScript
- Injects the bundled WebViews into the main file
- Bundles dependencies into the main file
- Embeds Sourcemaps into the file inline
- Packages everything up into an extension folder `dist`

## Contribution guidelines

* Contributions that further the goals of this project are welcomed.
* If your pull request does not contain passing unit tests that demonstrate the value and quality of your contribution, it will reduce the likelihood that your submission will be accepted.
* If you contact me in advance of making any changes, I can probably give you some guidance and let you know if I think your proposed changes are on track.
* Please visit the [TXL Jira page](https://jira.sil.org/secure/Dashboard.jspa?selectPageId=10760) to see the outstanding Jira issues.

## Who do I talk to?

* Technical lead: [Tom Bogle](mailto:Transcelerator_feedback@sil.org)
* To discuss the concepts and principles behind Transcelerator (e.g., if you disagree with the idea of using "canned" questions as basis for checking comprehension), contact your translation consultant and/or local translation department representative.

