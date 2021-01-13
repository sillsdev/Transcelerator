### What is this repository for? ###

* Transcelerator helps Bible translation teams translate comprehension checking questions in order to prepare to test the quality and clarity of the vernacular Scripture translation.

### How do I get set up? ###

* To get set up to use and build Transcelerator, you will need to download [Paratext](https://paratext.org/).
* There are some post-build commands that will attempt to copy the plugin files to a (potentially) useful location if you are building using the "Debug - Copy to Paratext" or "Release - Copy to Paratext" configurations. Depending on your individual needs, you might want to tweak the details, but if you do, please don't include your tweaks in a pull request. When building the "Release - Copy to Paratext" configuration for the first time, it will attempt to set up the necessary directory structure by copying files into the default install location(s) for Paratext. You need to be running Visual Studio as an administrator, or the robocopy command(s) will fail.
* To learn to use Transcelerator, see the [wiki](https://bitbucket.org/paratext/transcelerator/wiki/Home), especially the [Getting Started](https://bitbucket.org/paratext/transcelerator/wiki/Getting%20Started) page.
* Unit tests depend on [NUnit](https://nunit.org/). I recommend using [Jet Brains Resharper](https://www.jetbrains.com/resharper/), which has built-in test running capabilities.
* The [Paratext Demo Plugins](https://github.com/ubsicap/paratext_demo_plugins) repository has more advanced information about the Paratext plugin architecture, which will explain more about how to build a plugin like Transcelerator.

### Contribution guidelines ###

* Contributions that further the goals of this project are welcomed.
* If your pull request does not contain passing unit tests that demonstrate the value and quality of your contribution, it will reduce the likelihood that your submission will be accepted.
* If you contact me in advance of making any changes, I can probably give you some guidance and let you know if I think your proposed changes are on track.
* Please visit the [TXL Jira page](https://jira.sil.org/secure/Dashboard.jspa?selectPageId=10760) to see the outstanding Jira issues.

### Who do I talk to? ###

* Technical lead: [Tom Bogle](mailto:Transcelerator_feedback@sil.org)
* To discuss the concepts and principles behind Transcelerator (e.g., if you disagree with the idea of using "canned" questions as basis for checking comprehension), contact your translation consultant and/or local translation department representative.