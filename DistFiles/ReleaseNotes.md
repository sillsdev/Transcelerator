## _VERSION_ _DATE_

## 3.2.0 23 May 2025

- Made it easier to see when questions belong to a variant in order to exclude an entire variant as a group.
- Added support for localizers who need to omit certain questions, comments, answers, and alternatives.
- Added partial Tok Pisin localization.
- Improved layout in Generate Checking Script dialog box so that localized text fits better.
- Fixed bugs to make regular expression helper controls more usable in the Question Adjustments dialog box.

## 3.1.20 9 July 2024

- Added an option to omit the Scripture verses before each question in the generated script.
- Added alternatives to several questions, added a few new questions and made a few small edits (mostly punctuation and spelling corrections) to answers and notes.
- Updated the British English localization

## 3.1.3 6 August 2022

- Internationalized the Help system and localized it into Spanish. (Additional localizations can be done using [crowdin](https://crowdin.com/project/transcelerator).)

## 3.1.0 3 March 2022

- Added ability to control text size in main grid.

## 3.0.0 17 November 2021

- Changed to use the new interfaces compatible with Paratext 9.2.
- This version also fixes several deficiencies, including performance issues when sorting on the Translation column.
- Improvements to the Add Question dialog box
- Improved interaction with biblical terms
- Properly handles read-only projects
- Includes some new questions (primarily in the Old Testament)

## 2.0.5 22 January 2021

- Changed verse numbers to output using a span element with class "verse" so they can be formatted using styles in word processing programs (e.g., MS Word) that recognize the span elements that have the class attribute set. Note: If you use an external CSS, you will need to regenerate it or manually edit it in order for verse numbers to display as superscripted in the generated HTML checking script files. (To regenerate the external CSS, in the __Generate Checking Script__ dialog box on the __Appearance__ tab, select the __Overwrite Existing CSS File__ option.)

## 2.0.3 11 January 2021

- Added Help system
- Added (optional) Edit column

## 2.0.0 3 December 2020

- 64-bit version of Transcelerator for use with Paratext 9.1 and later.

## 1.5.2 27 October 2020

- Minimum version needed to produce compatible files to import into Scripture Forge.
- Various improvements to filtering and modifying questions.

## 1.5.0 23 September 2020

- Added ability to localize the user interface. (Includes partial localization into Spanish.)

## 1.4.18 31 July 2020

- Added option to output verse numbers in script (Note: If using an external CSS file, to get verse numbers to appear superscripted, you will either need to allow Transcelerator to overwrite your CSS file or edit the existing one to add verse {vertical-align: super; font-size: .80em; color:DimGray;}.
- Major improvements to question content and script generation.
- New option to control how out-of-order detail questions are handled in script.
- Improvements to New Question dialog, including ability to add questions out of verse order.

## 1.3.17 11 May 2020

- Added missing questions for Luke 22-24.

## 1.3.11 16 April 2019

- Added Installer support for Paratext 9.

## 1.3.9 9 January 2019

- Added (draft quality) localizations of all questions, answers, and notes for French and Spanish.

## 1.3.1 9 August 2018

- Added support for display of questions, answers, and notes in languages other than English.

## 1.2.1 15 September 2015

- Improved ability to add, modify, and exclude questions, including the capability of adding questions for verses that do not have any existing questions. These kinds of changes no longer require an immediate restart of Transcelerator to load and process each change, so the performance is now much better!

## 1.1.5439 15 September 2015

- Fixed installer problems.

## 1.1.5430 19 November 2014

- Updated key term rules based on changes to biblical terms list included with Paratext 7.5.

## 1.1.5175 3 March 2014

- Updates/corrections to questions, mostly related to content for 1 and 2 Kings.

## 1.1.5164 20 February 2014

- Fixed some crashing bugs in the Phrase Substitution dialog box and improved error reporting. Removed ability to sort on columns in that dialog box since rows are ordered.
- Updated key term rules based on changes to biblical terms list included with Paratext.

## 1.1.5154 10 February 2014

- Prevent crash when Paratext fails to load biblical terms.
- Updated key term rules based on changes to biblical terms list included with Paratext.

## 1.1.5149 5 February 2014

- First publicly promoted (stable) version of Transcelerator.
- Transcelerator's automatic keyboard switching now works correctly with Keyman.
- Transcelerator now includes questions for all the Old Testament books. Though they are not as thorough as the questions for Genesis and the New Testament and were not written explicitly for the purpose of comprehension checking, they may serve as a helpful starting place. We would welcome any contributions of additional questions.

## 1.1.5070 4 December 2013

- Lots of improvements to questions and various bug fixes.

## 1.0.1 9 April 2013

- Remember user settings
- Lots of other changes to tweak settings for Paratext and improve interaction with Paratext.
- Enabled keyboard switching.
- Added Psalms and Proverbs to master questions list.

## 1.0.0 28 March 2013

- First release of Transcelerator as a Paratext plugin.

