// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.Localization;
using KSPDev.FSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSPDev.LocalizationTool {

[KSPAddon(KSPAddon.Startup.MainMenu, true /*once*/)]
class Scanner : MonoBehaviour {
  //FIXME
  void Awake() {
    var locItems = EmitAllItemsForPrefix("KAS-1.0/");
    ConfigStore.WriteLocItems(locItems,
                              Localizer.CurrentLanguage,
                              KspPaths.GetModsDataFilePath(this, "agg-localization.cfg"));
  }

  /// <summary>
  /// Finds all the entities that macthes the prefix, and extracts the items for them.
  /// </summary>
  /// <param name="prefix">
  /// The file path prefix. It's relative to the game's <c>GameData</c> directory.
  /// </param>
  /// <returns>The all items for the prefix.</returns>
  public static List<LocItem> EmitAllItemsForPrefix(string prefix) {
    var res = new List<LocItem>();

    // Extract strings for the parts.    
    var parts = PartLoader.LoadedPartsList
        .Where(x => x.partUrl.StartsWith(prefix, StringComparison.CurrentCulture));
    foreach (var part in parts) {
      res.AddRange(Extractor.EmitItemsForPart(part));
    }

    // Extract strings from the assembliy(ies). 
    var types = AssemblyLoader.loadedAssemblies
        .Where(x => x.url.StartsWith(prefix, StringComparison.CurrentCulture))
        .SelectMany(x => x.types)
        .SelectMany(x => x.Value);
    foreach (var type in types) {
      res.AddRange(Extractor.EmitItemsForType(type));
    }

    return res;
  }
}

}  // namesapce
