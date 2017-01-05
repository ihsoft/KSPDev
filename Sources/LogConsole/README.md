## KSPDev: Kerbal Development tools - LogConsole
####Mod for developers who create mods for [Kerbal Space Program](http://www.kerbalspaceprogram.com/)

[KSPDev.LogConsole](https://github.com/ihsoft/KSPDev/tree/master/Sources/LogConsole) is an advanced console for in-game logging
system. Just install it in place of the built-in one and have all the benefits. See [here](http://imgur.com/a/rwAyt) how it may look for you.

####Main features
* Full screen window improves visibility.
* Advanced system of logs filtering. You can blacklist logs you don't want to see.
* Stack trace for any log record allows you figuring out the source.
* Each log record has a "source", a short version of full stack trace that says where this record came from.
* Log records has timestamps which helps in retrospective analysis of the events.
* Ability to save the logs into files for even better retrospective analysis.
* Apply a quick filter to see logs of the specific types only: INFO, WARNING, ERROR or EXCEPTION.
* Log records are added too fast? No problem! There is a pause mode.
* Made a bug in `Update()` method and log records are being written at a rate of 100 per second? Problem solved: make use of the two
new modes. In _Condensed_ and _Smart_ modes high frequency records are collapsed into just one line!
