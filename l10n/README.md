## Transcelerator Localization

### Updating Crowdin with source string changes - UPLOAD TO CROWDIN NOT YET ENABLED

All the strings that are internationalized in the Transcelerator project are uploaded to Crowdin in Transcelerator.en.xlf

The L10nSharp tool ExtractXliff is run on the project to get any updates to the source strings resulting in a new Transcelerator.en.xlf file.

Overcrowdin is used to upload this file to Crowdin. * NOT YET *

This process is run automatically by a GitHub action if the commit comment mentions any of 'localize, l10n, i18n, internationalize, spelling' * NOT YET *

Because Transcelerator does not (yet) use GitVersion, the version number is hard-coded in build.proj.
The hard-coded version of the l10n.proj file should be updated to match the new current version
whenever the crowdin sources are being regenerated.

It can also be run manually as follows:
```
dotnet tool install -g overcrowdin
set CROWDIN_TRANSCELERATOR_KEY=TheApiKeyForTheTransceleratorProject (hint: look in Tools/API)
msbuild l10n.proj /t:UpdateCrowdin
```

### Updating LocalizedPhrases in Crowdin based on questions translated in Transcelerator

Note that currently Transcelerator does not allow for translating Answers or Notes, so the only way to localize those at this time is via Crowdin (or a thrid-party offline tool). To translate or edit the questions in Transcelerator, you will need a Paratext project that targets the language into which you intend to localize. After translating some or all of the questions in Transcelerator, run the Transcelerator Question Pre-Processor plugin, which is on the *main* Paratext menu, under Paratext, Advanced. (This will need to built locally, and copied over with %ParatextInstallDir% targeting the installation location.) Choose the option to Create/Update Localization, and fill in the target locale. The Existing Translations from Transcelerator will be Translations of Checking Questions.xml found in the pluginData\Transcelerator\Transcelerator folder within your project data folder (in My Paratext Projects). If you want to generate a file that can be uploaded to Crowdin but you need to limit it to only include certain books, etc., you can specify a regex (e.g., ^JHN, to include only the questions for the book of John).