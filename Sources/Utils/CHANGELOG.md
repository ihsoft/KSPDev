# 0.35.0 (pre-release):
* [Change] Move specification tag constant from `LocalizationManager` into a specialized class [`GUIUtils.StdSpecTags`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_StdSpecTags.htm).

# 0.34.1 (May 19th, 2018):
* [Fix] Make the custom fields copy method as safe as possible. There are too many edge cases.

# 0.34 (May 16th, 2018):
* [Fix] Fix handling of the "contains" pattern in [`Hierarchy.PatternMatch`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Hierarchy_PatternMatch.htm). It didn't work.
* [Change] For compatibility with `KSP 1.4.3` deprecate [`PartConfig.GetModuleConfig`].
* [Enhancement] Add custom fields copy method: [`ConfigAccessor.CopyPartConfigFromPrefab`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ConfigUtils_ConfigAccessor_CopyPartConfigFromPrefab.htm).

# 0.33 (May 6th, 2018):
* [Fix] Fix the search in [Hierarchy.FindPartModelByPath](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Hierarchy_FindPartModelByPath.htm). It supposed to do the "breadth-first" search, but in fact was implementing the "depth-first" one. Now it's true BFS.
* [Fix] Fix reading of the initalized compound and collection fields: don't complain about the readonly fields.

# 0.32 (May 5th, 2018):
* [Change] Properly handle the localized stock resources names: [ResourceUtils.StockResourceNames](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ResourceUtils_StockResourceNames.htm).
* [Change] Deprecate `KSPUtilsGUILayout` class. Use `GUILayoutButtons` instead.
* [Change] Use stock `Action` as a callback in [GUIUtils.GuiActionsList](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_GuiActionsList.htm).
* [Enhancement] Upgrade `LocalizedMessage` to allow GUI hints to be passed from the localziation files into the mods: [GUIUtils.LocalizableMessage](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_LocalizableMessage.htm).
* [Enhancement] Add a new GUI layout class, the strings table: [GUIUtils.GUILayoutStringTable](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_GUILayoutStringTable.htm).
* [Enhancement] Add a new GUI layout class for tyhe buttons: [GUIUtils.GUILayoutButtons](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_GUILayoutButtons.htm).
* [Enhancement] Add a new math class: [MathUtils.Mathd](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_MathUtils_Mathd.htm).

# 0.31 (March 7th, 2018):
* [Chage] KSP 1.4.0 compatibility.
* [Enhancement] Add `onBeforeTransition` event into: [`ProcessingUtils.SimpleStateMachine`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ProcessingUtils_SimpleStateMachine_1.htm).
* [Enhancement] Better print `Vector3` and `Quaternion` objects with better precission in [`LogUtils.DebugEx`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_LogUtils_DebugEx.htm).
* [Enhancement] Add optional paraemetrs to skip the specified number of frames [`ProcessingUtils.AsyncCall`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ProcessingUtils_AsyncCall.htm).
* [Enhancement] Add methods to inject/withdraw part's menu events: [`PartUtils.PartModuleUtils`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_PartUtils_PartModuleUtils.htm).
* [Enhancement] Add a new method to align vessels via the attach nodes: [`AlignTransforms.SnapAlignNodes`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_AlignTransforms_SnapAlignNodes.htm).
* [Enhancement] Allow restricting the state machine handlers to init/shutdown sequence: [`SimpleStateMachine.AddStateHandlers`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ProcessingUtils_SimpleStateMachine_1_AddStateHandlers.htm).
* [Enhancement] Add a new method to load custom fields from the part's config: [`ConfigAccessor.ReadPartConfig`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ConfigUtils_ConfigAccessor_ReadPartConfig.htm).
* [Enhancement] Add a localization class to format a velocity value in a human friendly format: [`GUIUtils.VelocityType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_VelocityType.htm).
* [Enhancement] Add a utility class to create `PushButton` and `ToggleButton` controls: [`GUIUtils.KSPUtilsGUILayout`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_KSPUtilsGUILayout.htm).
* [Enhancement] Add a serializable `ConfigNode` to deal with a portion of the config in a form of simple node: [`Types.PersistentConfigNode`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_Types_PersistentConfigNode.htm).
* [Fix] Properly handle string paths in [`ConfigUtils.ConfigAccessor`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_ConfigAccessor.htm): treat an empty string as a root object reference.

# 0.30 (December 14th, 2017):
* [Enhancement] Add new method to align vessel via the nodes: [AlignTransforms.SnapAlignVessel](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_AlignTransforms_SnapAlignVessel.htm).
* [Enhancement] Add new method to place a vessel: [AlignTransforms.PlaceVessel](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_AlignTransforms_PlaceVessel.htm).
* [Change] Drop `traceUpdates` parameter in [`AsyncCall.WaitForPhysics`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ProcessingUtils_AsyncCall_WaitForPhysics.htm).
* [Fix] Handle persistent fields that are set to NULL: silently don't store them, and create on restore if there is a config node. See [`ConfigUtils.PersistentFieldAttribute`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_PersistentFieldAttribute.htm).
* [Fix] Properly calculate the rotation in [`ModelUtils.PersistentFieldAttribute.AlignTransforms.SnapAlign`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_AlignTransforms_SnapAlign.htm).

# 0.29 (October 23rd, 2017):
* [Enhancement] Add the `Fine()` logging methods to spit the logs when the game is set to the extending logging mode ("Verbose Logging"): [`LogUtils.HostedDebugLog`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_LogUtils_HostedDebugLog.htm).
* [Enhancement] Add an optional parameter `subFolder` to allow getting/creating the mods data folder: [`FSUtils.KspPaths`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_FSUtils_KspPaths.htm).
* [Enhancement] Add a full replacement for the stock logging methods with a new method `Fine` that can produce more logs when the user has requested it (via a stock game's setting): [`LogUtils.DebugEx`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_LogUtils_DebugEx.htm).
* [Enhancement] Add a method to disable collisions between a part and a vessel: [`Colliders.SetCollisionIgnores`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Colliders_SetCollisionIgnores_1.htm).
* [Change] Drop `noDefault` parameter in [`GUIUtils.MessageLookup`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_MessageLookup_1.htm).

# 0.28 (September 14th, 2017):
* [Change] Drop the auto-localization feature. It's too error prone. The modules must explicitly call [`LocalizationLoader.LoadItemsInModule`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_GUIUtils_LocalizationLoader_LoadItemsInModule.htm).
* [Enhancement] Add an interface for the modules that need localization. [`GUIUtils.IsLocalizableModule`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_IsLocalizableModule.htm).

# 0.27 (September 2nd, 2017):
* [Enhancement] Add methods to resolve the resources abbreviations: [`ResourceUtils.StockResourceNames`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ResourceUtils_StockResourceNames.htm).
* [Enhancement] Add a localization class to format a double value in a human friendly format: [`GUIUtils.CompactNumberType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_CompactNumberType.htm).
* [Enhancement] Add a localization class to format a cost (credits) value in a human friendly format: [`GUIUtils.CostType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_CostType.htm).
* [Enhancement] Add a localization class to format a pressure value in a human friendly format: [`GUIUtils.PressureType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_PressureType.htm).
* [Enhancement] Add a localization class to format a resource name value: [`GUIUtils.ResourceType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_ResourceType.htm).
* [Enhancement] Add a localization class to format a resource abbreviated name: [`GUIUtils.ResourceShortType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_ResourceShortType.htm).
* [Enhancement] Add the methods to deal with the module actions: [`PartUtils.PartModuleUtils`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_PartUtils_PartModuleUtils.htm).

# 0.26 (August 8th, 2017):
* [Fix] Drop cached UI sounds on scene change.
* [Fix] Support [`IPersistentField`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_IPersistentField.htm) in the KSP types proto.
* [Fix] Support auto-localization in the parts that were created by a third-party mod (e.g. `KIS`).
* [Enhancement] Add a sugar interface [`GUIUtils.IHasGUI`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_IHasGUI.htm).
* [Enhancement] Add a type for the common KSP object layers: See [`ModelUtils.KspLayer`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ModelUtils_KspLayer.htm).
* [Enhancement] Extend a sugar interface [`KSPInterfaces.IPartModule`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_KSPInterfaces_IPartModule.htm).
* [Enhancement] Add the transformation methods to deal with the [`Types.PosAndRot` type](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_Types_PosAndRot.htm). Also see [`PosAndRotExtensions`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_Extensions_PosAndRotExtensions.htm).
* [Enhancement] Add a localization class to format a Unity keyboard event: [`GUIUtils.KeyboardEventType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_KeyboardEventType.htm).
* [Enhancement] Add a utlilty class to deal with the modules's events: [`PartUtils.PartModuleUtils`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_PartUtils_PartModuleUtils.htm).
* [Enhancement] Add a utlilty class to deal with the part's model: [`PartUtils.PartModel`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_PartUtils_PartModel.htm).

# 0.25 (July 15th, 2017):
* [Change] Drop `GUIUtils.EnumType` class in favor of `GUIUtils.MessageLookup`.
* [Enhancement] Support automatic localization for the events, fields, and actions in the modules. See [`GUIUtils.LocalizableItemAttribute`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_LocalizableItemAttribute.htm).
* [Enhancement] Support (de)serialization of types `Vector2` and `Enum`. See [`ConfigUtils.KspTypesProto`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ConfigUtils_KspTypesProto.htm).
* [Enhancement] Add an extension to the `UnityEngine.Rect` type to support basic rectangles operations. See [`Extensions.RectExtensions`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_Extensions_RectExtensions.htm).
* [Enhancement] Add a new utility class to deal with the GUI windows. See [`GUIUtils.GuiWindow`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_GuiWindow.htm).
* [Enhancement] Allow defining a message which won't try to localize. See [`GUIUtils.LocalizableMessage`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_LocalizableMessage.htm).
* [Enhancement] Add a new utility class to deal with the GUI elements enabled state. See [`GUIUtils.GuiEnabledStateScope`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_GuiEnabledStateScope.htm).
* [Enhancement] Add a new utility class to deal with the GUI elements colors. See [`GUIUtils.GuiColorScope`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_GuiColorScope.htm).
* [Enhancement] Add a localization class to deal with enums and other ordinal types. See [`GUIUtils.MessageLookup`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_MessageLookup_1.htm).

# 0.24 (June 22nd, 2017):
* [Change] Allow any state transition in `SimpleStateMachine` when the strict mode is OFF: [`ProcessingUtils.SimpleStateMachine`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ProcessingUtils_SimpleStateMachine_1.htm).
* [Change] Drop `donCache` option from `UISoundPlayer`: [`UISoundPlayer.Play`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_GUIUtils_UISoundPlayer_Play.htm).
* [Change] Add an optional parameter to: [`Hierarchy.FindTransformByPath`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_ModelUtils_Hierarchy_FindTransformByPath_1.htm).
* [Change] Major refactoring of the state machine: [`ProcessingUtils.SimpleStateMachine`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ProcessingUtils_SimpleStateMachine_1.htm).
* [Change] Support KSP 1.3 localization in the `GUIUtils` [message classes](http://ihsoft.github.io/KSPDev/Utils/html/N_KSPDev_GUIUtils.htm).
* [Change] Drop a messaging type that doesn't support localization: `GUIUtils.MessageBoolValue`.
* [Change] Drop a messaging type that doesn't support localization: `GUIUtils.MessageSpecialFloatValue`.
* [Change] Drop a GUI helper that doesn't support localization: `GUIUtils.Formatter`.
* [Change] Drop a messaging type that doesn't support localization: `GUIUtils.MessageEnumValue<T>`. Use [`GUIUtils.EnumType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_EnumType_1.htm) instead.
* [Enhancement] Add method overloads for [`ModelUtils.FindTransformByPath`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ModelUtils_Hierarchy.htm) to simplify finding the models in the part.
* [Enhancement] Add a method to parse `PosAndRot` from a string: [`Types.PosAndRot`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_Types_PosAndRot.htm).
* [Enhancement] Add a method to log nullable values: [`DbgFormatter.Nullable`](http://ihsoft.github.io/KSPDev/Utils/html/M_KSPDev_LogUtils_DbgFormatter_Nullable__1.htm).
* [Enhancement] Add a new interface for the modules that need their context menu updated: [`GUIUtils.IHasContextMenu`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_IHasContextMenu.htm).
* [Enhancement] Add a new utility class for better logging of an object state: [`LogUtils.HostedDebugLog`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_LogUtils_HostedDebugLog.htm).
* [Enhancement] Add a new utility class to deal with the stock resource names: [`ResourceUtils.StockResourceNames`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_ResourceUtils_StockResourceNames.htm).
* [Enhancement] Add a new utility class to deal with the 3D sounds: [`SoundsUtils.SpatialSounds`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_SoundsUtils_SpatialSounds.htm).
* [Enhancement] Add a base class to deal with the localizable strings (KSP 1.3+): [`GUIUtils.LocalizableMessage`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_LocalizableMessage.htm).
* [Enhancement] Add a localization class to format a distance: [`GUIUtils.DistanceType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_DistanceType.htm).
* [Enhancement] Add a localization class to format a enum value: [`GUIUtils.EnumType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_EnumType_1.htm).
* [Enhancement] Add a localization class to format an angle value: [`GUIUtils.AngleType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_AngleType.htm).
* [Enhancement] Add a localization class to format a force value: [`GUIUtils.ForceType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_ForceType.htm).
* [Enhancement] Add a localization class to format a mass value: [`GUIUtils.MassType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_MassType.htm).
* [Enhancement] Add a localization class to format a percent value: [`GUIUtils.PercentType`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_GUIUtils_PercentType.htm).
* [Enhancement] A major improvement in the methods that deal with the mod's paths resolving: [`FSUtils.KspPaths`](http://ihsoft.github.io/KSPDev/Utils/html/T_KSPDev_FSUtils_KspPaths.htm).
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
