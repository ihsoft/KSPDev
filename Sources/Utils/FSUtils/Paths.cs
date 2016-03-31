// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.IO;

namespace KSPDev.FSUtils {

/// <summary>A helper class to deal with plugins file structure.</summary>
public static class KspPaths {
  /// <summary>Standard plug-ins folder.</summary>
  private const string PluginsRootFolder = "GameData/";

  /// <summary>Returns full path to the plugins root folder.</summary>
  public static string pluginsRoot {
    get {
      return KSPUtil.ApplicationRootPath + PluginsRootFolder;
    }
  }

  /// <summary>Makes a full name for a file living in plugin's folder.</summary>
  /// <param name="folder">A folder path relative to the plugin's root. That said, if code needs a
  /// a file belonging to a particular plugin then its name must be the first component of the path.
  /// E.g. <c>folder="KSPDev"</c> will address files in <c>KSPDev</c> plugin directory.</param>
  /// <param name="fileName">A file name to make path for. Can be empty but not <c>null</c>.</param>
  /// <returns>A full path to the plugin's file.</returns>
  public static string makePluginResourceName(string folder, string fileName) {
    if (folder.Length == 0) {
      return pluginsRoot + fileName;
    }
    if (folder[folder.Length - 1] != '/' && folder[folder.Length - 1] != '\\') {
      folder += '/';
    }
    return Path.Combine(pluginsRoot, folder) + fileName;
  }
}

}
