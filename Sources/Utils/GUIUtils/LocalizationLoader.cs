// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

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
  /// <summary>
  /// Specification for the <see cref="KSPField"/> <c>guiUnits</c> localization. 
  /// </summary>
  /// <remarks>
  /// Use it when specifying a <see cref="LocalizableItemAttribute"/> for a field with the units.
  /// </remarks>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField_WithUnits"/></example>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public const string KspFieldUnitsSpec = "units";

  /// <summary>Localizes the <see cref="PartModule"/> items.</summary>
  /// <remarks>
  /// <para>
  /// The localizable items must be declared as non-static public members. The following items are
  /// supported:
  /// <list type="bullet">
  /// <item>
  /// <see cref="KSPField"/>. This type may have multiple localization items: for <c>guiName</c>
  /// (spec=<c>null</c>) and for <c>guiUnits</c> (spec=<see cref="KspFieldUnitsSpec"/>).
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
        } else if (locItem.spec == KspFieldUnitsSpec) {
          field.guiUnits = locItem.GetLocalizedString();
        } else {
          Debug.LogWarningFormat("Bad specialization tag for field {0}.{1}: {2}",
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
    Debug.LogWarningFormat("Localization version is updated to {0} in: {1}",
                           LocalizableMessage.systemLocVersion, LibraryLoader.assemblyVersionStr);

    // FLIGHT: Update the part modules in all the loaded vessels.
    if (HighLogic.LoadedSceneIsFlight) {
      FlightGlobals.Vessels
          .Where(v => v.loaded)
          .SelectMany(v => v.parts)
          .ToList()
          .ForEach(UpdateLocalizationInPartModules);
    }

    // EDITOR: Update the part modules in all the game objects in the scene.
    if (HighLogic.LoadedSceneIsEditor) {
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
    HostedDebugLog.Info(rootPart, "EDITOR: Load localizations for the existing part from {0}",
                        LibraryLoader.assemblyVersionStr);
    UpdateLocalizationInPartModules(rootPart);
    rootPart.children.ForEach(UpdateLocalizationInPartHierarchy);
  }

  /// <summary>Updates all the localizable strings in a part.</summary>
  /// <param name="part">The part to load the data in.</param>
  static void UpdateLocalizationInPartModules(Part part) {
    foreach (var module in part.Modules.OfType<IsLocalizableModule>()) {
      module.LocalizeModule();
    }
  }
  #endregion
}

}  // namespace
