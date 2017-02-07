# KSPDev: Kerbal Development tools - LogConsole

`LogConsole` is an advanced console for in-game logging system. Supports persistense, filtering,
records grouping, and many more. See [here](http://imgur.com/a/rwAyt) how it may look.

Read discussions, ask questions and suggest features on
[forum](http://forum.kerbalspaceprogram.com/index.php?/topic/150786-12-kspdev-logconsole-utils).

#Main features

* Full screen window improves visibility.
* Advanced system of logs filtering. You can blacklist logs you don't want to see.
* Stack trace for any log record allows you figuring out the source.
* Each log record has a "source", a short version of full stack trace that says where this record came
  from.
* Log records has timestamps which help in retrospective analysis of the events.
* Ability to save the logs into files for even better retrospective analysis.
* A quick filter can be applied to see logs of the specific types only: INFO, WARNING, ERROR or
  EXCEPTION.
* Pause mode to freeze the view when logs records are added to fast.
* Two special modes for handling high frequency logs (e.g. when logging from `Update()` method). In
  _Condensed_ and _Smart_ modes multiple repeated records are collapsed into just one line!
* Console settings can be adjusted via `settings.cfg` file.
