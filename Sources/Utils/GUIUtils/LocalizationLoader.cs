// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.UI.Screens;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A utility class to localize the annotated members</summary>
/// <remarks>
/// It also monitors if a new localizable module is created or loaded, or if the
/// <see cref="LocalizableMessage.systemLocVersion">localization version</see> has changed. If this
/// is the case, then all the modules will be automatically updated.
/// </remarks>
/// <seealso cref="LocalizableItemAttribute"/>
/// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo1"/></example>
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
  /// <see cref="PartModule.OnAwake"/> method looks the most appropriate since it's called each time
  /// the module is created. The other methods may be called differently depending on the loaded
  /// scene.
  /// </para>
  /// </remarks>
  /// <param name="module">The module instance to localize.</param>
  /// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo1"/></example>
  /// <seealso cref="LocalizableItemAttribute"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPEvent']"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPAction']"/>
  public static void LoadItemsInModule(PartModule module) {
    module.Fields.Cast<BaseField>().ToList()
        .ForEach(LocalizeKSPField);

    // Hash the KSP annotated methods by the name. We only take the methods with zero arguments. 
    var methodItemTypes = new[] { typeof(KSPEvent), typeof(KSPAction) };
    var methodsByName = module.GetType()
        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .Where(m =>
             m.GetParameters().Length == 0
             && m.GetCustomAttributes(true).Any(o => methodItemTypes.Contains(o.GetType())))
        .ToDictionary(k => k.Name);

    module.Events
        .Where(e => methodsByName.ContainsKey(e.name))
        .ToList()
        .ForEach(x => LocalizeKSPEvent(methodsByName[x.name], x));
    module.Actions
        .Where(e => methodsByName.ContainsKey(e.name))
        .ToList()
        .ForEach(x => LocalizeKSPAction(methodsByName[x.name], x));
  }

  /// <summary>Installs the event listeners to do the automatic modules localization.</summary>
  void Awake() {
    GameEvents.onLanguageSwitched.Add(OnUpdateLocalizationVersion);
    GameEvents.onVesselCreate.Add(OnNewVessel);
    GameEvents.onVesselLoaded.Add(OnNewVessel);
    GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
    GameEvents.onEditorLoad.Add(OnEditorLoad);
  }

  #region Game event listeners. Must not be static.
  /// <summary>Reacts on an editor part event and localizes the part when needed.</summary>
  /// <param name="eventType">The type of the event.</param>
  /// <param name="part">The part being acted on.</param>
  void OnEditorPartEvent(ConstructionEventType eventType, Part part) {
    if (eventType == ConstructionEventType.PartCreated
        || eventType == ConstructionEventType.PartCopied) {
      Debug.LogFormat("EDITOR: Load localizations for a new part \"{0}\" (id={1}) from {2}",
                      part.name , part.craftID, LibraryLoader.assemblyVersionStr);
      UpdateLocalizationInPartModules(part);
    }
  }

  /// <summary>Localizes a vessel which is laoded in the editor.</summary>
  /// <param name="shipConstruct">The ship's parts data.</param>
  /// <param name="loadType">Unused.</param>
  void OnEditorLoad(ShipConstruct shipConstruct, CraftBrowserDialog.LoadType loadType) {
    Debug.LogFormat("EDITOR: Load vessel localizations in \"{0}\" from {1}",
                    shipConstruct.shipName, LibraryLoader.assemblyVersionStr);
    shipConstruct.parts.ForEach(UpdateLocalizationInPartModules);
  }

  /// <summary>Loads all the localizable strings in a vessel.</summary>
  /// <param name="vessel">The vessel to load strings in.</param>
  void OnNewVessel(Vessel vessel) {
    if (vessel.loaded) {
      Debug.LogFormat("FLIGHT: Load vessel localizations in \"{0}\" from: {1}",
                      vessel, LibraryLoader.assemblyVersionStr);
      vessel.parts.ForEach(UpdateLocalizationInPartModules);
    }
  }

  /// <summary>Invalidates all the localization caches and updates the current vessels.</summary>
  /// <remarks>It updates all the currently loaded vessels.</remarks>
  void OnUpdateLocalizationVersion() {
    LocalizableMessage.systemLocVersion++;
    Debug.LogWarningFormat("Localization version is updated to {0} in: {1}",
                           LocalizableMessage.systemLocVersion, LibraryLoader.assemblyVersionStr);

    // FLIGHT: Update the part modules in all the laoded vessels.
    if (HighLogic.LoadedSceneIsFlight) {
      FlightGlobals.Vessels
          .Where(v => v.loaded)
          .SelectMany(v => v.parts)
          .ToList()
          .ForEach(UpdateLocalizationInPartModules);
    }

    // EDITOR: Update the part modules in all the game object in the scene.
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
  /// <summary>Loads the localized string(s) for a KSP field.</summary>
  /// <param name="kspField">The field to load localization for.</param>
  static void LocalizeKSPField(BaseField kspField) {
    var locItems = kspField.FieldInfo.GetCustomAttributes(false)
        .OfType<LocalizableItemAttribute>();
    foreach (var locItem in locItems) {
      if (string.IsNullOrEmpty(locItem.tag)) {
        continue;  // Localization is disabled for the item.
      }
      if (string.IsNullOrEmpty(locItem.spec)) {
        kspField.guiName = locItem.GetLocalizedString();
      } else if (locItem.spec == KspFieldUnitsSpec) {
        kspField.guiUnits = locItem.GetLocalizedString();
      } else {
        Debug.LogWarningFormat("Bad specialization tag for field {0}.{1}: {2}",
                               kspField.FieldInfo.FieldType.FullName,
                               kspField.FieldInfo.Name,
                               locItem.spec);
      }
    }
  }

  /// <summary>Loads the localized string for a KSP event.</summary>
  /// <param name="attrs">The attributes on the event.</param>
  /// <param name="event">The event to localize.</param>
  static void LocalizeKSPEvent(ICustomAttributeProvider attrs, BaseEvent @event) {
    var locItem = attrs.GetCustomAttributes(false)
        .OfType<LocalizableItemAttribute>()
        .FirstOrDefault();
    if (locItem != null && !string.IsNullOrEmpty(locItem.tag)) {
      @event.guiName = locItem.GetLocalizedString();
    }
  }

  /// <summary>Loads the localized string for a KSP action.</summary>
  /// <param name="attrs">The attributes provider on the action.</param>
  /// <param name="action">The action to localize.</param>
  static void LocalizeKSPAction(ICustomAttributeProvider attrs, BaseAction action) {
    var locItem = attrs.GetCustomAttributes(false)
        .OfType<LocalizableItemAttribute>()
        .FirstOrDefault();
    if (locItem != null && !string.IsNullOrEmpty(locItem.tag)) {
      action.guiName = locItem.GetLocalizedString();
    }
  }

  /// <summary>Localizes the modules in the part and in all of its children parts.</summary>
  /// <param name="rootPart">The root part to start from.</param>
  static void UpdateLocalizationInPartHierarchy(Part rootPart) {
    Debug.LogFormat("EDITOR: Load localizations for the existing part \"{0}\" (id={1}) from {2}",
                    rootPart.name , rootPart.craftID, LibraryLoader.assemblyVersionStr);
    UpdateLocalizationInPartModules(rootPart);
    rootPart.children.ForEach(UpdateLocalizationInPartHierarchy);
  }

  /// <summary>Updates all the localizable strings in a part.</summary>
  /// <param name="part">The part to load the data in.</param>
  static void UpdateLocalizationInPartModules(Part part) {
    part.Modules.Cast<PartModule>().ToList()
        .ForEach(module => {
          LocalizationLoader.LoadItemsInModule(module);
          var hasContextMenu = module as IHasContextMenu;
          if (hasContextMenu != null) {
            hasContextMenu.UpdateContextMenu();
          }
        });
  }
  #endregion
}

}  // namespace
