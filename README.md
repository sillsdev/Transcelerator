### What is this repository for? ###

* Transcelerator helps Bible translation teams translate comprehension checking questions in order to prepare to test the quality and clarity of the vernacular Scripture translation.

### How do I get set up? ###

* To get set up to use and build Transcelerator, you will need to download [Paratext](http://paratext.org/). (I'm sorry, but at this time Paratext is not open source and has license restrictions. I have no control over that, but if you talk to me, I might be able to help you get it.)
* There are some post-build commands that will attempt to copy the plugin files to a (potentially) useful location if you are building using the "Debug - Copy to Paratext" or "Release - Copy to Paratext" configurations. Depending on your individual needs, you might want to tweak the details, but if you do, please don't include your tweaks in a pull request. When building the "Release - Copy to Paratext" configuration for the first time, it will attempt to set up the necessary directory structure by copying files into the default install location for Paratext 7 and/or 8. If those folders are not already there, you need to be running Visual Studio as an administrator, or the xcopy command(s) will fail.
* To learn to use Transcelerator, see the [wiki](https://bitbucket.org/paratext/transcelerator/wiki/Home), especially the [Getting Started](https://bitbucket.org/paratext/transcelerator/wiki/Getting%20Started) page.
* Unit tests depend on [NUnit](http://nunit.org/). I recommend using [Jet Brains Resharper](http://www.jetbrains.com/resharper/), which has built-in test running capabilities.
* The [Paratext Plugins Wiki](https://bitbucket.org/paratext/paratext-demo-plugins/wiki/Home) has more advanced information about the Paratext plugin architecture, which will explain more about how to build a plugin like Transcelerator.

### Contribution guidelines ###

* I welcome contributions that further the goals of this project.
* If your pull request does not contain passing unit tests that demonstrate the value and quality of your contribution, it will reduce the likelihood that your submission will be accepted.
* If you contact me in advance of making any changes, I can probably give you some guidance and let you know if I think your proposed changes are on track.
* Please visit the [TXL Jira page](https://jira.sil.org/secure/Dashboard.jspa?selectPageId=10760) to see the outstanding Jira issues.

### Who do I talk to? ###

* Technical lead: [Tom Bogle](mailto:Transcelerator_feedback@sil.org)
* To discuss the concepts and principles behind Transcelerator (e.g., if you disagree with the idea of using "canned" questions as basis for checking comprehension), contact Mark Penny.