// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.IO;

namespace KSPDev.FSUtils {

/// <summary>A helper class to deal with plugins file structure.</summary>
public static class KspPaths {
  /// <summary>Standard plug-ins folder.</summary>
  public static readonly string pluginsRootFolder = "GameData" + Path.DirectorySeparatorChar;

  /// <summary>Returns full path to the plugins root folder.</summary>
  public static string pluginsRoot {
    get {
      return KSPUtil.ApplicationRootPath + pluginsRootFolder;
    }
  }

  /// <summary>Makes an absolute file path given a relative one.</summary>
  /// <param name="path">A relative or an absolute part. If the first case the path is translated
  /// from the game's plugin root folder ("GameData" as of KSP 1.0.5). If argument is an absolute
  /// path then it's returned as is.</param>
  /// <returns>Absolute file path.</returns>
  public static string makePluginPath(string path) {
    return Path.Combine(pluginsRoot, path);
  }
}

}  // namespace
