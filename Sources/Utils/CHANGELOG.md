# 0.23.1-pre:
* [Change] Allow any state transition in `SimpleStateMachine` when the strict mode is OFF: [`ProcessingUtils.SimpleStateMachine`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ProcessingUtils_SimpleStateMachine_1.htm).
* [Change] Drop `donCache` option from `UISoundPlayer`: [`UISoundPlayer.Play`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_GUIUtils_UISoundPlayer_Play.htm).
* [Change] Add an optional parameter to: [`Hierarchy.FindTransformByPath`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Hierarchy_FindTransformByPath_1.htm).
* [Enhancement] Add a new utility class for better logging of an object state: [`LogUtils.HostedDebugLog`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_LogUtils_HostedDebugLog.htm).
* [Enhancement] Add method overloads for [`ModelUtils.FindTransformByPath`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ModelUtils_Hierarchy.htm) to simplify finding the models in the part.
* [Fix #12] Keyboard input switch misses release key event.
* [Fix #13] AlignTransforms.SnapAlign sets a wrong direction to the source.

# 0.23.0 (May 11th, 2017):
* [Enhancement] Add a syntax surgar interface for `IJointLockState`: [`KSPInterfaces.IKSPDevJointLockState`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_KSPInterfaces_IKSPDevJointLockState.htm).
* [Enhancement] Add a syntax surgar interface for `OnPartDie` callback: [`KSPInterfaces.IsPartDeathListener`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_KSPInterfaces_IsPartDeathListener.htm).
* [Enhancement] Implement escaping of the path separator symbol in [`Hierarchy.FindTransformByPath`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Hierarchy_FindTransformByPath.htm).

# 0.22.1 (April 29th, 2017):
* [Enhancement] Add a new utility class to deal with the transformations orientation: [`ModelUtils.AlignTransforms`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ModelUtils_AlignTransforms.htm).
* [Enhancement] Add a new utility class to deal with the parts configs: [`ConfigUtils.PartConfig`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_PartConfig.htm).
* [Enhancement] Add an interface to allow custom types serializing into/from a string: [`ConfigUtils.IPersistentField`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_IPersistentField.htm).
* [Enhancement] Add a new serializable type to keep orientation and position: [`Types.PosAndRot`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_Types_PosAndRot.htm).
* [Enhancement] Add new methods to disable colliders on the parts or objects: [`Colliders.SetCollisionIgnores`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Colliders_SetCollisionIgnores.htm).
* [Fix] Fix the collider size for a cylinder created via [`Meshes.CreateCylinder`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Meshes_CreateCylinder.htm).

# 0.21.0 (March 8th, 2017):
* [Change] Refactor [`ProcessingUtils.AsyncCall`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ProcessingUtils_AsyncCall.htm) methods to use a standard `Action` type for the delegates. It's an _incompatible_ change!
* [Enhancement] Add method [`ProcessingUtils.AsyncCall.WaitForPhysics`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ProcessingUtils_AsyncCall_WaitForPhysics.htm) which allows a flexible waiting for the game physics updates.
* [Enhancement] Add method [`ModelUtils.Hierarchy.PatternMatch`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Hierarchy_PatternMatch.htm) which offers simple but commonly used text search patterns.
* [Enhancement] Major refactoring of [`ModelUtils.Hierarchy.FindTransformByPath`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Hierarchy_FindTransformByPath_1.htm). It can now handle paths of any complexity, and can deal with objects with the same names. A better help and examples are also provided.
* [Enhancement] Extend [`LogUtils.DbgFormatter`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_LogUtils_DbgFormatter.htm) with more methods to log vectors, quaternions and hierarchy paths.
* [Enhancement] Allow [`LogUtils.DbgFormatter.C2S`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_LogUtils_DbgFormatter_C2S__1.htm) to accept an arbitrary separator string for joining the collection elements.

# 0.20.0 (January 8th, 2017):
* [Fix] Handling "None" collider type in [`ModelUtils.Colliders`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ModelUtils_Colliders.htm).
* [Change] Deprecate `FSUtils.KspPath.makePluginPath` in favor of [`FSUtils.KspPath.MakeAbsPathForGameData`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_FSUtils_KspPaths_MakeAbsPathForGameData.htm).
* [Enhancement] Add more methods to deal with absolute and relative KSP paths into [`FSUtils.KspPath`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_FSUtils_KspPaths.htm).
* [Enhancement] Add `VersionLogger` to identify utils versions loaded for better troubleshooting.

# 0.19.0 (December 14th, 2016):
* [Change] Refactor event system in [`KeyboardInputSwitch`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_InputUtils_KeyboardInputSwitch.htm).

# 0.18.0 (December 13th, 2016):
* [Change] Improve support for compound persistent fields.
* [Enhancement] Support (de)serialization of `IConfigNode` types in [`PersistentFieldAttribute`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_PersistentFieldAttribute.htm).
* [Change] Move [`EventChecker`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_InputUtils_EventChecker.htm) into [`KSPDev.InputUtils`](http://ihsoft.github.io/KSPDev/Utils/html/N_KSPDev_InputUtils.htm) namespace.
* [Enhancement] New [`KeyboardInputSwitch`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_InputUtils_KeyboardInputSwitch.htm) class to handle `KeyCode` bindings.

# 0.17.0 (Nov 19, 2016):
* [Fix] Improved code samples and fixed some docs.
* [Enhancement] `MessageBoolValue` class to format boolean values.
* [Enhancement] `MessageEnumValue` class to format values of enum.
* [Enhancement] `IsPhysicalObject` interface.
* [Enhancement] `DbgFormatter` class for making common debug strings.
* [Deprecation] Move `Logger.C2S` into `DbgFormatter`.
* [Deprecation] Drop `Logger` class.
