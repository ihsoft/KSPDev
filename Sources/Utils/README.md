##KSPDev - Client utilites
####Library for developers who create mods for [Kerbal Space Program](http://www.kerbalspaceprogram.com/)

[KSPDev_Utils](https://github.com/ihsoft/KSPDev/tree/master/Sources/Utils) is a set of handy tools that simplify development of KSP
addons. Just add the assembly into your project and save a lot of development efforts. See detailed documentation with code snippets here: [KSPDev_Utils](http://ihsoft.github.io/KSPDev/Utils).

####In nutshell

KSPDev Utils offers a lot of different tools. Here is some examples but there are more features (see docs)!
* Extensive set of methods to work with config files
  * Save or load simple values without dealing with string<=>type conversion. The type will be detected from the argument, and
  built-in converters will handle any C# or KSP/Unity type transformation.
  * Use [attribute-oriented programming](https://en.wikipedia.org/wiki/Attribute-oriented_programming) to mark configuration fields in
  your classes. Then just specify file name and have them loaded/saved in one single method call.
  * Use attributes to mark fields that are represented by a class or struct. These types will be (de)serialized as config nodes.
  Attribute handlers can also deal with collections! No need to persist every item or a structure field separately.
  * Go further and define all configuration logic completely via attributes. After that you call just one method with minimum of
  arguments to have all your mod's settings saved. Or loaded - you choose.
* Various helpers to deal with GUI. E.g. GUI sounds, context windows popups, messages, etc.
* Basic set of methods to deal with in-game file paths.
* Well documented KSP interfaces.
* Methods to deal with procedural models in the game.
* Different helpers for common processing tasks like state machine or a delaying a method call.
