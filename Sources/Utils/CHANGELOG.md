### 0.20.0 (January 8th, 2017):
- [Fix] Handling "None" collider type in [`ModelUtils.Colliders`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ModelUtils_Colliders.htm).
- [Change] Deprecate `FSUtils.KspPath.makePluginPath` in favor of [`FSUtils.KspPath.MakeAbsPathForGameData`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_FSUtils_KspPaths_MakeAbsPathForGameData.htm).
- [Enhancement] Add more methods to deal with absolute and relative KSP paths into [`FSUtils.KspPath`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_FSUtils_KspPaths.htm).
- [Enhancement] Add `VersionLogger` to identify utils versions loaded for better troubleshooting.

### 0.19.0 (December 14th, 2016):
- [Change] Refactor event system in [`KeyboardInputSwitch`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_InputUtils_KeyboardInputSwitch.htm).

### 0.18.0 (December 13th, 2016):
- [Change] Improve support for compound persistent fields.
- [Enhancement] Support (de)serialization of `IConfigNode` types in [`PersistentFieldAttribute`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_PersistentFieldAttribute.htm).
- [Change] Move [`EventChecker`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_InputUtils_EventChecker.htm) into [`KSPDev.InputUtils`](http://ihsoft.github.io/KSPDev/Utils/html/N_KSPDev_InputUtils.htm) namespace.
- [Enhancement] New [`KeyboardInputSwitch`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_InputUtils_KeyboardInputSwitch.htm) class to handle `KeyCode` bindings.

### 0.17.0 (Nov 19, 2016):
- [Fix] Improved code samples and fixed some docs.
- [Enhancement] `MessageBoolValue` class to format boolean values.
- [Enhancement] `MessageEnumValue` class to format values of enum.
- [Enhancement] `IsPhysicalObject` interface.
- [Enhancement] `DbgFormatter` class for making common debug strings.
- [Deprecation] Move `Logger.C2S` into `DbgFormatter`.
- [Deprecation] Drop `Logger` class.
