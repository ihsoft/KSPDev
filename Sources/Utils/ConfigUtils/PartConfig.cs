// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using UnityEngine;

namespace KSPDev.ConfigUtils {

/// <summary>Helper methods to access and manipulate part's configs.</summary>
public static class PartConfig {
  /// <summary>Gets a config the part module.</summary>
  /// <remarks>
  /// It takes into account the module index, so in case of the part has multiple modules of the
  /// same type the right module config will be returned.
  /// </remarks>
  /// <param name="module">Module to get config for.</param>
  /// <param name="partNode">
  /// Part's config to use. If omitted then part's prefab config is used.
  /// </param>
  /// <returns>Either the found config node or an empty node. It's never <c>null</c>.</returns>
  public static ConfigNode GetModuleConfig(PartModule module, ConfigNode partNode = null) {
    ConfigNode res = null;
    if (module.part.partInfo != null && module.part.partInfo.partConfig != null) {
      var moduleIdx = module.part.Modules.IndexOf(module);
      var nodes = (partNode ?? module.part.partInfo.partConfig).GetNodes("MODULE");
      if (moduleIdx != -1 && moduleIdx < nodes.Length
          && nodes[moduleIdx].GetValue("name") == module.moduleName) {
        res = nodes[moduleIdx];
      }
    }
    if (res == null) {
      if (partNode == null) {
        Debug.LogWarningFormat("Cannot find config for module {0} in the config of part {1}",
                               module.moduleName, module.part.name);
      } else {
        Debug.LogWarningFormat("Cannot find config for module {0} in the custom's config: {1}",
                               module.moduleName, partNode);
      }
      res = new ConfigNode("MODULE");
    }
    return res;
  }
}

}  // namespace
