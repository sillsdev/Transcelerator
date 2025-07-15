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

### Transcelerator Question Pre-Processor

In addition to the Transcelerator plugin itself, the Transcelerator solution also includes a developer tool called Transcelerator Question Pre-Processor. It's main purpose is to support creating the localization source file and for updating localizations in other languages based on translations done in Transcelerator.

This plugin does not have an installer, so it needs to built locally, and copied over into Paratext's plugins folder. This can be accomplished easily by building with the `Debug - Copy to Paratext` configuration if you have set the environment variable `%ParatextInstallDir%` to target the appropriate installation location. To install for use with an installed version (as opposed to a locally built dev verison), it is easiest if you run VS as an administrator and build with the `Release - Copy to Paratext` configuration. Alternatively, you can have the plugin files copied to an unprotected staging folder and then copy them manually into the plugins folder of the installed Paratext software and provide admin rights just to allow the copy.

To run Transcelerator Question Pre-Processor, open Paratext. Then you will find the plugin on the *main* (top-level) Paratext menu, under Paratext, Advanced.

Note: The Transcelerator Question Pre-Processor can also be used to convert SFM-based questions (e.g., in the format used in Translator's Workplace) to the XML format that Transcelerator expects. However, it has been years ince the original conversion, and now the XML format is the one that is maintained. So it is unlikely that a SFM->XML conversion would ever be needed again, and the code to do that is not being regularly tested or maintained.

### Updating LocalizedPhrases source in Crowdin to reflect updates in Transcelerator

Whenever changes have been made in TxlQuestions.xml, the localization source file (LocalizedPhrases.xlf) should be updated in Crowdin. To do thins, do the following:
- Optionally (but recommended), download the current source version of LocalizedPhrases.xlf from Crowdin.
- Run the Transcelerator Question Pre-Processor in Paratext.
- Enter "en" as the BCP-47 locale.
- Click Generate XML.
- Do a global search/replace in the newly generated version to change all instances of `state="needs-translation"` to `state="translated"`. (This is a meaningless attribute for a source file, but it is helpul, especially for comparing versions, to have this attribute value consistent.)
- Optionally (but recommended), use a diff program (such as KDiff) to compare the updated LocalizedPhrases.xlf file with the source downloaded from Crowdin. Verify that all the changes are expected based on the changes in TxlQuestions.xml.
- Upload the new version in Crowdin. Confirm that the summary of changes matches your expections. (Revert if not!)
- Note that any change at all in a string -- even minor punctuation changes -- will result in the generation of a new localization ID, so existing translations for all languages will have to be updated. For very minor changes, especially when the correction in English might not even affect the translation, it is a good idea to update the translations for those strings right away so that nothing gets lost unnecessarily.
- If you are responsible for any localizations, now is a great time to translate any new/edited strings.
- if any localizations were updated:
  - download them and overwrite the existing LocalizedPhrases-aa.xlf file(s) in the Transcelerator folder.
  - Build and spot-check them in the UI.
  - Ideally, include the updated localizations along with the PR having the changes to TxlQuestions.xml. Otherwise, create a new PR with those changes.

### Updating LocalizedPhrases in Crowdin based on questions translated in Transcelerator

Note that currently Transcelerator does not allow for translating Answers or Notes, so the only way to localize those at this time is via Crowdin (or a thrid-party offline tool). To translate or edit the questions in Transcelerator, you will need a Paratext project that targets the language into which you intend to localize. After translating some or all of the questions in Transcelerator, do the following:
- Ensure that the existing localized version of LocalizedPhrases-aa.xlf in your local Transceleraotr folder is up-to-date with the latest translations in Crowdin. (If needed, download the current version from Crowdin to replace your local version and stage or commit those changes).
- Run the Transcelerator Question Pre-Processor plugin.
- Fill in the target locale.
- In the Existing Translations from Transcelerator box, enter the `Translations of Checking Questions.xml` found in the `pluginData\Transcelerator\Transcelerator` folder within your project data folder (in My Paratext Projects).
- If you want to generate a file that can be uploaded to Crowdin but you need to limit it to only include certain books, etc., you can specify a regex (e.g., ^JHN, to include only the questions for the book of John).
- If the translations done in Transcelertor have been approved already, check Mark approved.
- Click Generate XML.
- Optionally (but recommended), use a diff program (such as KDiff) to compare the updated LocalizedPhrases.xlf file with the source downloaded from Crowdin. Verify that all the changes are expected based on the changes in TxlQuestions.xml.
- Upload the new version in Crowdin for the appropriate language. Confirm that the summary of changes matches your expections. (Revert if not!)
- Spot-check the changes in Crowdin.
- Download the new version and overwrite the existing LocalizedPhrases-aa.xlf file(s) in the Transcelerator folder.
- Using git or your git client, confirm that the expected changes are present and ready to be staged.
- Build and spot-check the changes in the UI.
- Commit, push, and create a PR for the changes.