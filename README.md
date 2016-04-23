# KSPDev - Mods and tools for KSP modders

_The only version existing for now is pre-lrease 0.10.1.0. Though, the existing functionality is not expected to change. It's highly expected to be extended with new methods/abilities._

## Licensing

All code is disributed under "[Public Domain license](https://en.wikipedia.org/wiki/Public_domain)". In other words, you can do
whatever you want with the code.

Though, if you put a link to this GitHub repository it will be highly appreciated. And it may help other developers :)

## KSPDev - Debug log console
####Mod for developers who create mods for [Kerbal Space Program](http://www.kerbalspaceprogram.com/)

[KSPDev.LogConsole](https://github.com/ihsoft/KSPDev/tree/master/Sources/LogConsole) is an advanced console for in-game logging
system. Just install it in place of the built-in one and have all the benefits.

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

##KSPDev - Client utilites
####Library for developers who create mods for [Kerbal Space Program](http://www.kerbalspaceprogram.com/)

[KSPDev_Utils](https://github.com/ihsoft/KSPDev/tree/master/Sources/Utils) is a set of handy tools that simplify development of KSP
addons. Just add the assembly into your project and save a lot of development efforts.

####Main features

* Extensive set of methods to work with config files
([ConfigUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_ConfigUtils.htm)):
  * Save or load simple values without dealing with string<=>type conversion. The type will be detected from the argument, and
  built-in converters will handle any C# or KSP/Unity type transformation.
  * Use [attribute-oriented programming](https://en.wikipedia.org/wiki/Attribute-oriented_programming) to mark configuration fields in
  your classes. Then just specify file name and have them loaded/saved in one single method call.
  * Use attributes to mark fields that are represented by a class or struct. These types will be (de)serialized as config nodes.
  Attribute handlers can also deal with collections! No need to persist every item or a structure field separately.
  * Go further and define all configuration logic completely via attributes. After that you call just one method with minimum of
  arguments to have all your mod's settings saved. Or loaded - you choose.
* Convinience methods to output formatted debug logs ([LogUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_LogUtils.htm)).
* Unity GUI helper to properly handle UI actions ([GUIUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_GUIUtils.htm)).
* Basic set of methods to deal with in-game file paths ([FSUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_FSUtils.htm)).

Detailed documentation with code snippets is here: [KSPDev_Utils](http://ihsoft.github.io/KSPDevUtils_Doc)
