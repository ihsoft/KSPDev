# KPSPDev: ReleaseBuilder v2.1 (August 15th, 2017):
* [Enhancement] Add new configuration parameter [RELEASE_NAME_FREE_FORMAT](https://github.com/ihsoft/KSPDev/wiki/ReleaseBuilder-Schema-1.1).
* [Enhancement] Support [auto-generated version components](https://msdn.microsoft.com/en-us/library/system.reflection.assemblyversionattribute%28v=vs.110%29.aspx#Anchor_6) for BUILD and REVISION.

# KPSPDev: ReleaseBuilder v2.0.0 (January 21st, 2017):
* [Change] All settings are simplified and renamed to give better context. JSON settings created
  for builder older than 1.2 won't work!
* [Change] Introduced JSON field `JSON_SCHEMA_VERSION` to help managing changes in JSON semantics
  in the future versions.
* [Change] Deprecated `POST_BUILD_COPY` setting. Such actions should be done via project's
  post-build events.
* [Change] Merge excutable and the builder class into one file for simplicity.
* [Enhancement] Do safety checks on the constructed paths: read paths must not be above project's
  root; write paths must not be above release folder path.
* [Enhancement] Major improvement of errors handling and reporting.
* [Enhancement] Added auto-detection of GitHub repository for `PROJECT_ROOT`.
* [Enhancement] Using Python built-in ZIP archiving capability. No need in external archiver
  anymore.
* [Enhancement] Implemented `-j` option that allows specifying arbitrary JSON settings file.
  When used in this mode working directory is assumed to be directory of the JSON file. Script
  doesn't need to be located in the building repository anymore.
* [Enhancement] Added support of macros in all paths.
* [Enhancement] Add more program arguments to better control builder behavior. Running wihtout any
  argument now shows help screen.
