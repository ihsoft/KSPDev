// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.UI.Screens;
using KSPDev.GUIUtils;
using KSPDev.FSUtils;
using System;
using System.Linq;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>
/// Initialization class that installs the game event listeners needed by the library.
/// </summary>
/// <remarks>
/// It only acts on the very first invocation since the same versions of the library can be copied
/// in multiple folders. This class also logs the version and the location from which the library is
/// loaded. It helps understanding which libraries are actually loaded in case of there are multiple
/// libraries/versions in the game.
/// </remarks>
[KSPAddon(KSPAddon.Startup.Instantly, true /*once*/)]
class LibraryLoader : MonoBehaviour {
  /// <summary>Loaded library indentifier.</summary>
  public static string assemblyVersionStr { get; private set; }

  /// <summary>Tells if the loader has already initialized.</summary>
  static bool loaded;

  void Awake() {
    if (loaded) {
      gameObject.DestroyGameObject();
      return;  // Only let the loader to work once per version.
    }
    loaded = true;

    var assembly = GetType().Assembly;
    assemblyVersionStr = string.Format(
        "{0} (v{1})",
        KspPaths.MakeRelativePathToGameData(assembly.Location),
        assembly.GetName().Version);
    Debug.LogFormat("Loading KSPDevUtils: {0}", assemblyVersionStr);

    // Install the localization callbacks. The object must not be destroyed.
    UnityEngine.Object.DontDestroyOnLoad(gameObject);
    GameEvents.onVesselCreate.Add(UpdateLocalizationInVessel);
    GameEvents.onVesselLoaded.Add(UpdateLocalizationInVessel);
    GameEvents.onLanguageSwitched.Add(UpdateLocalizationVersion);
    GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
    GameEvents.onEditorLoad.Add(OnEditorLoad);
  }

  /// <summary>Reacts on an editor part event and localizes the part when needed.</summary>
  /// <param name="eventType">The type of the event.</param>
  /// <param name="part">The part being acted on.</param>
  static void OnEditorPartEvent(ConstructionEventType eventType, Part part) {
    if (eventType == ConstructionEventType.PartCreated
        || eventType == ConstructionEventType.PartCopied) {
      Debug.LogFormat("EDITOR: Load localizations for a new part \"{0}\" (id={1}) from {2}",
                      part.name , part.craftID, assemblyVersionStr);
      UpdateLocalizationInPartModules(part);
    }
  }

  /// <summary>Localizes a vessel which is laoded in the editor.</summary>
  /// <param name="shipConstruct">The ship's parts data.</param>
  /// <param name="loadType">Unused.</param>
  static void OnEditorLoad(ShipConstruct shipConstruct, CraftBrowserDialog.LoadType loadType) {
    Debug.LogFormat("EDITOR: Load vessel localizations in \"{0}\" from {1}",
                    shipConstruct.shipName, assemblyVersionStr);
    shipConstruct.parts.ForEach(UpdateLocalizationInPartModules);
  }

  /// <summary>Loads all the localizable strings in a vessel.</summary>
  /// <param name="vessel">The vessel to load strings in.</param>
  static void UpdateLocalizationInVessel(Vessel vessel) {
    if (vessel.loaded) {
      Debug.LogFormat("FLIGHT: Load vessel localizations in \"{0}\" from: {1}",
                      vessel, assemblyVersionStr);
      vessel.parts.ForEach(UpdateLocalizationInPartModules);
    }
  }

  /// <summary>Invalidates all the localization caches and updates the current vessels.</summary>
  /// <remarks>It updates all the currently loaded vessels.</remarks>
  static void UpdateLocalizationVersion() {
    LocalizableMessage.systemLocVersion++;
    Debug.LogWarningFormat("Localization version is updated to {0} in: {1}",
                           LocalizableMessage.systemLocVersion, assemblyVersionStr);

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

  /// <summary>Localizes the modules in the part and in all of its children parts.</summary>
  /// <param name="rootPart">The root part to start from.</param>
  static void UpdateLocalizationInPartHierarchy(Part rootPart) {
    Debug.LogFormat("EDITOR: Load localizations for the existing part \"{0}\" (id={1}) from {2}",
                    rootPart.name , rootPart.craftID, assemblyVersionStr);
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
}

}  // namespace
