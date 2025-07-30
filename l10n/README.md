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

Whenever changes are made to `TxlQuestions.xml`, the localization source file (`LocalizedPhrases.xlf`) should be updated in Crowdin. To do this, follow these steps:
1. (Optional but recommended) Download the current source version of `LocalizedPhrases.xlf` from Crowdin.
2. Run the Transcelerator Question Pre-Processor in Paratext.
3. Enter "en" as the BCP-47 locale.
4. Click Generate XML.
5. Do a global search/replace in the newly generated version to change all instances of `state="needs-translation"` to `state="translated"`.
   > *(Although this attribute is meaningless for a source file, it’s helpful—especially for diffing—to keep the value consistent.)*
6. (Optional but recommended) Use a diff tool (such as KDiff) to compare the updated `LocalizedPhrases.xlf` file with the version downloaded from Crowdin.
   > Verify that all the changes are expected based on the edits made in `TxlQuestions.xml`.
7. Upload the updated file to Crowdin. Confirm that the summary of changes matches your expectations.
   > *(Revert the upload if anything looks wrong!)*
8. **Important:** Any change to a string—even minor punctuation edits—will result in a new localization ID. This means existing translations will have to be re-entered for all languages.
   > For trivial changes (e.g., fixing a typo that doesn’t affect translation), consider updating the translations for those strings right away so they’re not lost unnecessarily.
9. If you are responsible for any localizations, now is a great time to translate any new or modified strings.
10. If any localizations were updated:
  - Download them from Crowdin and overwrite the existing `LocalizedPhrases-aa.xlf` file(s) in the Transcelerator folder.
  - Using Git or your Git client, confirm that the expected changes are present and ready to be staged.
  - Build and spot-check them in the UI.
  - Ideally, include the updated localizations in the same PR as the changes to `TxlQuestions.xml`. Otherwise, create a separate PR with the localization updates.

### Updating LocalizedPhrases in Crowdin Based on Questions Translated in Transcelerator

Some localizers may prefer to use Transcelerator itself (i.e., “dogfooding”) to translate checking questions as part of the process of localizing them for use in Transcelerator. To do this, they will need a Paratext project targeting the language into which they want to localize. The developer who is supporting them should be an Observer on the Paratext project.

Once a localizer has translated some or all of the questions in Transcelerator, follow these steps:

1. Use Send/Receive in Paratext to get the latest version of the project
   > This will include the plugin data from Transcelerator (including the `Translations of Checking Questions.xml` file).
2. Update your local `LocalizedPhrases-aa.xlf` file.
   > Ensure the localized file in your local Transcelerator folder reflects the latest Crowdin translations. If needed, download the current version from Crowdin and replace your local file. Stage or commit those changes as appropriate.
3. Run the Transcelerator Question Pre-Processor plugin.
   > This tool helps convert your translated questions into the XLIFF format that both Crowdin and Transcelerator use.
4. Specify the target locale.
5. Provide the translated questions
   > In the *Existing Translations from Transcelerator* box, browse to and select the `Translations of Checking Questions.xml` file. This file is located in the `pluginData\Transcelerator\Transcelerator` folder inside your Paratext project’s data directory (typically `c:\My Paratext Projects`).
6. (Optional) Filter output by Scripture reference
   > If you want to limit the generated file to questions from specific books, enter a regular expression in the provided field (e.g., `^JHN` to include only questions for John).
7. If the translations done in Transcelerator have been approved already, check *Mark approved*.
8. Click Generate XML.
9. (Optional but recommended) Compare changes
   > Use a diff program (e.g., KDiff3) to compare the updated `LocalizedPhrases-aa.xlf` file with the one downloaded from Crowdin. Confirm that all changes are expected based on what you translated in Transcelerator.
10. Upload the new version in Crowdin for the appropriate language.
   > Carefully review the summary of changes. If anything looks wrong, revert it.
11. Spot-check the changes in Crowdin.
12. Download and overwrite local file
   > Download the version now in Crowdin and replace the `LocalizedPhrases-aa.xlf` file(s) in the Transcelerator folder. *The downloaded file could contain additional localizations of answers, notes, alternative forms, and questions from other books (if you uploaded a filtered set of questions), as well as any additional localizations or edits made in Crowdin after the upload.*
13. Review in version control
   > Use Git (or your preferred Git GUI client) to inspect the changes. Confirm that the updates are what you expect and are ready to be staged.
14. Build and verify in the UI
   > Rebuild and run Transcelerator. Select the UI language corresponding to the locale being updated, and verify that the localized questions (and other material, as appropriate) appear correctly in the questions grid. *Pay special attention to any questions that were localized as `Omit*`. These questions should not appear in the question grid at all, nor should they be included in the output when generating a checking script.*
15. Commit, push, and create a pull request for review on GitHub.

*Note: Transcelerator currently does not allow for translating Answers or Notes, nor for translating multiple alternative forms of questions, so the only way to localize those at this time is via Crowdin (or a third-party offline tool).*
