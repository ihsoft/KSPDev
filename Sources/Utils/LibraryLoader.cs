// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

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
    GameEvents.onVesselCreate.Add(LocalizeVessel);
    GameEvents.onVesselLoaded.Add(LocalizeVessel);
    GameEvents.onLanguageSwitched.Add(UpdateLocalizationVersion);
    //FIXME: Handle editor parts actions.
    GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
  }

  /// <summary>Reacts on an editor part event and localizaes the part when needed.</summary>
  /// <param name="eventType">The type of the event.</param>
  /// <param name="part">The part being acted on.</param>
  void OnEditorPartEvent(ConstructionEventType eventType, Part part) {
    if (eventType == ConstructionEventType.PartCreated
        || eventType == ConstructionEventType.PartCopied) {
      UpdatePartModulesLocalization(part);
    }
  }

  /// <summary>Loads all the localizable strings in a vessel.</summary>
  /// <param name="vessel">The vessel to load strings in.</param>
  void LocalizeVessel(Vessel vessel) {
    if (vessel.loaded) {
      Debug.LogFormat("Load vessel localizations in \"{0}\" from: {1}",
                      vessel, assemblyVersionStr);
      vessel.parts.ForEach(UpdatePartModulesLocalization);
    }
  }

  /// <summary>Invalidates all the localization caches and updates the current vessels.</summary>
  /// <remarks>It updates all the currently loaded vessels.</remarks>
  void UpdateLocalizationVersion() {
    LocalizableMessage.systemLocVersion++;
    Debug.LogWarningFormat("Localization version is updated to {0} in: {1}",
                           LocalizableMessage.systemLocVersion, assemblyVersionStr);
    // Update all the vessels.
    FlightGlobals.Vessels
        .SelectMany(x => x.parts)
        .ToList()
        .ForEach(UpdatePartModulesLocalization);
    //FIXME: In the editor find the parts in a different way.
  }

  /// <summary>Updates all the localizable strings in a part.</summary>
  /// <param name="part">The part to load the data in.</param>
  static void UpdatePartModulesLocalization(Part part) {
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
