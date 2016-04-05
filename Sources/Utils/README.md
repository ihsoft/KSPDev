#KSPDev - Client utilites
####Mod for [Kerbal Space Program](http://www.kerbalspaceprogram.com/)

KSPDev_Utils is a set of handy tools that simplify development of KSP addons.

Main features

* Extensive set of methods to work with config files ([ConfigUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_ConfigUtils.htm)):
  * Save or load simple values without dealing with string<=>type conversion. The type will be detected from the argument, and built-in converters will handle any C# or KSP/Unity type transformation.
  * Use meta programming to mark configuration fields in your classes. Then just specify file name and have them loaded/saved in one single method call.
  * Use meta programming to mark fields that are represented by a class or struct. These types will be (de)serialized as config nodes. Meta handler can also deal with collections!
  * Go further and define all configuration logic completely via annotations. After that you call just one method with minumum of argumenst to have all your mod's settings saved.
* Convinience methods to output formatted debug logs ([LogUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_LogUtils.htm)).
* Unity GUI helper to properly handle UI actions ([GUIUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_GUIUtils.htm)).
* Basic set of methods to deal with in-game file paths ([FSUtils](http://ihsoft.github.io/KSPDevUtils_Doc/html/N_KSPDev_FSUtils.htm)).

Detailed documentation with code snippets is here: [KSPDev_Utils](http://ihsoft.github.io/KSPDevUtils_Doc)
