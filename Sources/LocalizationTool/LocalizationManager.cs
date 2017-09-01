// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.Localization;
using KSP.UI;
using KSPDev.ConfigUtils;
using KSPDev.LogUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KSPDev.LocalizationTool {

/// <summary>A utility class to manipulate the game's localization content.</summary>
static class LocalizationManager {
  /// <summary>Updates the game's localization database from the strings on the disk.</summary>
  /// <param name="configFilename">The file name with the localization data.</param>
  /// <param name="targetNode">
  /// The language node from database to update. It must not be a copy!
  /// </param>
  public static void UpdateLocalizationContent(string configFilename, ConfigNode targetNode) {
    var newNode = ConfigAccessor.GetNodeByPath(
        ConfigNode.Load(configFilename), "Localization/" + Localizer.CurrentLanguage);
    var oldTags = new HashSet<string>(targetNode.values.DistinctNames());
    var newTags = new HashSet<string>(newNode.values.DistinctNames());
    Debug.LogWarningFormat(
        "Update localization config: added={0}, deleted={1}, updated={2}, file={3}",
        newTags.Except(oldTags).Count(),
        oldTags.Except(newTags).Count(),
        newTags.Intersect(oldTags).Count(),
        configFilename);
    // Update the existing and new tags. 
    newNode.values.Cast<ConfigNode.Value>().ToList()
        .ForEach(value => Localizer.Tags[value.name] = Regex.Unescape(value.value));
    // Drop the deleted tags.
    oldTags.Except(newTags).ToList()
        .ForEach(tag => Localizer.Tags.Remove(tag));
    // Update the database config.
    targetNode.values.Clear();
    newNode.values.Cast<ConfigNode.Value>().ToList()
        .ForEach(targetNode.values.Add);
  }

  /// <summary>Updates localizable strings in the part definiton.</summary>
  /// <remarks>
  /// The methods reads the current content from the part's config on disk and applies values to the
  /// localizable part fields. An up to date localization content must be loaded in the game for
  /// this method to actuall update the parts.
  /// </remarks>
  /// <param name="partInfo"></param>
  /// <seealso cref="UpdateLocalizationContent"/>
  /// <seealso cref="Extractor.localizablePartFields"/>
  public static void LocalizePartInfo(AvailablePart partInfo) {
    if (partInfo.partUrlConfig == null) {
      Debug.LogErrorFormat("Skip part {0} since it doesn't have a config", partInfo.name);
      return;
    }

    var partConfig = ConfigNode.Load(partInfo.partUrlConfig.parent.fullPath);
    // Don't request "PART" since it can be a ModuleManager syntax.
    partConfig = partConfig != null && partConfig.nodes.Count > 0 ? partConfig.nodes[0]: null;
    if (partConfig == null) {
      Debug.LogErrorFormat("Cannot find config for: {0}", partInfo.partUrlConfig.parent.fullPath);
      return;
    }

    Debug.LogFormat("Update strings in part {0}", partInfo.name);
    Extractor.localizablePartFields.ToList().ForEach(name => {
      var newValue = partConfig.GetValue(name);
      if (newValue != null) {
        ReflectionHelper.SetReflectedString(partInfo, name, newValue);
      }
    });
  }

  /// <summary>Updates data in all the open part menus.</summary>
  public static void LocalizePartMenus() {
    // The editor's tooltip caches the data, and we cannot update it. So just reset it.
    if (HighLogic.LoadedSceneIsEditor) {
      UIMasterController.Instance.DestroyCurrentTooltip();
    }
    UnityEngine.Object.FindObjectsOfType(typeof(UIPartActionWindow))
        .OfType<UIPartActionWindow>()
        .ToList()
        .ForEach(m => {
          Debug.LogFormat("Localize menu for part {0}", DbgFormatter.PartId(m.part));
          m.titleText.text = m.part.partInfo.title;
        });
  }
}

}  // namespace
