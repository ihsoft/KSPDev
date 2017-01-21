# KPSPDev: ReleaseBuilder v2.0.0 (pre-release):
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
