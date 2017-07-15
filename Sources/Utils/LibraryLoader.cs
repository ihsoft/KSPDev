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
    gameObject.AddComponent<LocalizationLoader>();
    gameObject.AddComponent<UISoundPlayer>();
  }
}

}  // namespace
