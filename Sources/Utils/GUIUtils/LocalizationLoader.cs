// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using KSPDev.LogUtils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A utility class to localize the annotated members</summary>
/// <remarks>
/// <para>
/// It also monitors if the
/// <see cref="LocalizableMessage.systemLocVersion">localization version</see> has changed. If this
/// is the case, then all the localizable modules will be notified.
/// </para>
/// <para>This module is initialized from the KSPDev Utils loader.</para>
/// </remarks>
/// <seealso cref="LocalizableItemAttribute"/>
/// <seealso cref="IsLocalizableModule"/> 
/// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo1"/></example>
/// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo2"/></example>
public class LocalizationLoader : MonoBehaviour {
  /// <summary>Localizes the <see cref="PartModule"/> items.</summary>
  /// <remarks>
  /// <para>
  /// The localizable items must be declared as non-static public members. The following items are
  /// supported:
  /// <list type="bullet">
  /// <item>
  /// <see cref="KSPField"/>. This type may have multiple localization items: for <c>guiName</c>
  /// (spec=<c>null</c>) and for <c>guiUnits</c> (spec=<see cref="StdSpecTags.Units"/>).
  /// </item>
  /// <item><see cref="KSPEvent"/>.</item>
  /// <item><see cref="KSPAction"/>.</item>
  /// </list>
  /// </para>
  /// <para>
  /// The original KSP attributes don't need to specify <c>guiName</c> field since it will be
  /// overwritten anyways. However, it's a good idea to give a default value just in case.
  /// </para>
  /// <para>
  /// This method can be called at any time during the module's life. However, the
  /// <see cref="IsLocalizableModule.LocalizeModule"/> method looks the most appropriate.
  /// </para>
  /// </remarks>
  /// <param name="module">The module instance to localize.</param>
  /// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo2"/></example>
  /// <seealso cref="LocalizableItemAttribute"/>
  /// <seealso cref="IsLocalizableModule"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPEvent']"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPAction']"/>
  public static void LoadItemsInModule(PartModule module) {
    // This method may look ugly and over complicated, but it's because if it's performance
    // optimized. On a vessel with 100 parts this method can be called 1000 times. So every
    // millisecond matters.

    // Go thru all the KSP fields that may have the localizable content. 
    foreach (var field in module.Fields) {
      var locItems = (LocalizableItemAttribute[])field.FieldInfo.GetCustomAttributes(
          typeof(LocalizableItemAttribute), false);
      foreach (var locItem in locItems) {
        if (string.IsNullOrEmpty(locItem.tag)) {
          continue;  // Localization is disabled for the item.
        }
        if (string.IsNullOrEmpty(locItem.spec)) {
          field.guiName = locItem.GetLocalizedString();
        } else if (locItem.spec == StdSpecTags.Units) {
          field.guiUnits = locItem.GetLocalizedString();
        } else if (locItem.spec == StdSpecTags.ToggleEnabled
                   || locItem.spec == StdSpecTags.ToggleDisabled) {
          var toggle = field.uiControlFlight as UI_Toggle;
          if (toggle != null) {
            if (locItem.spec == StdSpecTags.ToggleEnabled) {
              toggle.enabledText = locItem.tag;
            } else {
              toggle.disabledText = locItem.tag;
            }
          } else {
            DebugEx.Error("Field {0}.{1} is not a UI_Toggle. Cannot handle specifier: {2}",
                          field.FieldInfo.FieldType.FullName,
                          field.FieldInfo.Name,
                          locItem.spec);
          }
        } else {
          DebugEx.Error("Bad specialization tag for field {0}.{1}: {2}",
                        field.FieldInfo.FieldType.FullName,
                        field.FieldInfo.Name,
                        locItem.spec);
        }
      }
    }

    // Go thru all the KSP events that may have the localizable content. 
    foreach (var @event in module.Events) {
      var info = module.GetType().GetMethod(@event.name);
      if (info != null) {
        var locItems = (LocalizableItemAttribute[])info.GetCustomAttributes(
            typeof(LocalizableItemAttribute), false);
        if (locItems.Length > 0 && !string.IsNullOrEmpty(locItems[0].tag)) {
          @event.guiName = locItems[0].GetLocalizedString();
        }
      }
    }

    // Go thru all the KSP actions that may have the localizable content. 
    foreach (var action in module.Actions) {
      var info = module.GetType().GetMethod(action.name);
      if (info != null) {
        var locItems = (LocalizableItemAttribute[])info.GetCustomAttributes(
            typeof(LocalizableItemAttribute), false);
        if (locItems.Length > 0 && !string.IsNullOrEmpty(locItems[0].tag)) {
          action.guiName = locItems[0].GetLocalizedString();
        }
      }
    }
  }

  /// <summary>Installs the event listeners to do the automatic modules localization.</summary>
  void Awake() {
    GameEvents.onLanguageSwitched.Add(OnUpdateLocalizationVersion);
  }

  #region Game event listeners. Must not be static.
  /// <summary>Invalidates all the localization caches and updates the current vessels.</summary>
  /// <remarks>It updates all the currently loaded vessels.</remarks>
  void OnUpdateLocalizationVersion() {
    LocalizableMessage.systemLocVersion++;
    DebugEx.Warning("Localization version is updated to {0} in: {1}",
                    LocalizableMessage.systemLocVersion, LibraryLoader.assemblyVersionStr);

    // PREFAB: Update the prefab modules.
    DebugEx.Info("PREFAB: Reload part prefabs...");
    PartLoader.LoadedPartsList
        .Select(a => a.partPrefab)
        .Where(p => p.Modules.Cast<PartModule>().Any(IsModuleOfThisVersion))
        .ToList()
        .ForEach(UpdateLocalizationInPartModules);

    // FLIGHT: Update the part modules in all the loaded vessels.
    if (HighLogic.LoadedSceneIsFlight) {
      DebugEx.Info("FLIGHT: Reload parts on the vessels from...");
      FlightGlobals.Vessels
          .Where(v => v.loaded)
          .SelectMany(v => v.parts)
          .Where(p => p.Modules.Cast<PartModule>().Any(IsModuleOfThisVersion))
          .ToList()
          .ForEach(UpdateLocalizationInPartModules);
    }

    // EDITOR: Update the part modules in all the game objects in the scene.
    if (HighLogic.LoadedSceneIsEditor) {
      DebugEx.Info("EDITOR: Reload parts in the world...");
      // It can be slow but we don't care - it's not a frequent operation.
      UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
          .Select(o => o.GetComponent<Part>())
          .Where(p => p != null)
          .ToList()
          .ForEach(UpdateLocalizationInPartHierarchy);
    }
  }
  #endregion

  #region Local utility methods
  /// <summary>Localizes the modules in the part and in all of its children parts.</summary>
  /// <param name="rootPart">The root part to start from.</param>
  static void UpdateLocalizationInPartHierarchy(Part rootPart) {
    if (rootPart.Modules.Cast<PartModule>().Any(IsModuleOfThisVersion)) {
      UpdateLocalizationInPartModules(rootPart);
    }
    rootPart.children.ForEach(UpdateLocalizationInPartHierarchy);
  }

  /// <summary>Updates all the localizable strings in a part.</summary>
  /// <param name="part">The part to load the data in.</param>
  static void UpdateLocalizationInPartModules(Part part) {
    DebugEx.Fine("Reload part {0}...", part);
    if (part.partInfo != null && part.partInfo.partConfig != null) {
      var moduleConfigs = part.partInfo.partConfig.GetNodes("MODULE");
      for (var i = 0 ; i < part.Modules.Count && i < moduleConfigs.Length; i++) {
        var module = part.Modules[i];
        if (!IsModuleOfThisVersion(module)) {
          continue;  // Not our version, not our problem.
        }
        var moduleConfig = moduleConfigs[i];
        // Update the custom PersistentField fields from the prefab.
        ConfigAccessor.ReadFieldsFromNode(
            moduleConfig, module.GetType(), module,
            group: StdPersistentGroups.PartConfigLoadGroup);
      }
    }
    foreach (var module in part.Modules) {
      // Notify the localizable modules about the change.
      var localizableModule = module as IsLocalizableModule;
      if (localizableModule != null) {
        try {
          localizableModule.LocalizeModule();
        } catch (Exception ex) {
          DebugEx.Error(
              "Exception in LocalizeModule of module {0}: {1}", localizableModule, ex.Message);
        }
      }
      // Refresh the context menu.
      var hasContextMenu = module as IHasContextMenu;
      if (hasContextMenu != null) {
        try {
          hasContextMenu.UpdateContextMenu();
        } catch (Exception ex) {
          DebugEx.Error(
              "Exception in UpdateContextMenu of module {0}: {1}", localizableModule, ex.Message);
        }
      }
    }
  }

  /// <summary>
  /// Checks if the module was built using the same version of the Utils assembly as this one.
  /// </summary>
  /// <param name="module">The module to check.</param>
  /// <returns><c>true</c> if the module refers our version of the Utils assembly.</returns>
  static bool IsModuleOfThisVersion(PartModule module) {
    return module.GetType().Assembly.GetReferencedAssemblies()
        .Any(a => a.FullName == typeof(LocalizationLoader).Assembly.FullName);
  }
  #endregion
}

}  // namespace
