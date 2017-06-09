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

  /// <summary>Returns full path to the plugins root folder (a.k.a. <c>GameData</c>).</summary>
  /// <value>The full path to the plugins folder.</value>
  public static string pluginsRoot {
    get {
      return Path.GetFullPath(new Uri(KSPUtil.ApplicationRootPath + pluginsRootFolder).LocalPath);
    }
  }

  /// <summary>
  /// Makes full absolute path from the provided relative path in the game's <c>GameData</c> folder.
  /// </summary>
  /// <remarks>
  /// If joining of all the provided parts gives a full path then it's only normalized. In case of
  /// path is relative it's resolved against game's <c>GameData</c> folder.
  /// <para>Note that method doesn't care if the path exists.</para>
  /// </remarks>
  /// <param name="pathParts">Path parts for an absolute or relative path.</param>
  /// <returns>
  /// Absolute path. All relative casts (e.g. '..') will be resolved, and all
  /// directory separators will be translated to the platform format (e.g. '/' will become '\' on
  /// Windows). 
  /// </returns>
  public static string MakeAbsPathForGameData(params string[] pathParts) {
    var path = string.Join("" + Path.DirectorySeparatorChar, pathParts);
    if (!Path.IsPathRooted(path)) {
      path = Path.Combine(pluginsRoot, path);
    }
    return Path.GetFullPath(new Uri(path).LocalPath);
  }

  /// <summary>
  /// Normalizes path by resolving all upcasts. Works for both relative and absolute paths.
  /// </summary>
  /// <remarks>Note that method doesn't care if the path exists.</remarks>
  /// <param name="path">Path to normalize.</param>
  /// <returns>
  /// Path with no <c>.</c> or <c>..</c> casts. All directory separators will be translated to
  /// <c>/</c> regardless to the platform settings.
  /// </returns>
  /// <seealso cref="MakeRelativePathToGameData"/>
  public static string NormalizePath(string path) {
    if (Path.IsPathRooted(path)) {
      // For absolute path just request Path for the full path, and it'll do the heavy lifting. 
      return Path.GetFullPath(new Uri(path).LocalPath)
          .Replace(Path.DirectorySeparatorChar, '/');
    }
    // For a relative path bind it GameData, and then get a relative part.  
    path = MakeAbsPathForGameData(path);
    return MakeRelativePathToGameData(path);
  }

  /// <summary>Returns path relative to the game's GameData folder.</summary>
  /// <remarks>
  /// Note that method doesn't care if the path exists. The returned path will always use <c>/</c>
  /// as directory separator regardless to the platform.
  /// </remarks>
  /// <param name="pathParts">Path parts tp consutruct an absolute or relative path.</param>
  /// <returns>
  /// Relative path. All relative casts (e.g. '..') will be resolved, and all
  /// directory separators will be translated to <c>/</c> regardless to the platform settings.
  /// </returns>
  /// <example>
  /// Let's say mod's exact location is not known (e.g. as it is for MiniAVC) and the mod needs to
  /// load a texture. In order to do it the mod needs to know a <c>GameData</c> relative path which
  /// can be used as a prefix to the texture. Below is a sample code that figures it out.
  /// <code><![CDATA[
  /// var assembly = Assembly.GetExecutingAssembly();
  /// var relPath = KspPaths.MakeRelativePathToGameData(assembly.Location);
  /// Debug.LogWarningFormat("Assembly {0} is loaded from {1}", assembly.FullName, relPath);
  /// // Pretend the mod's DLL lives in 'Plugins' subfolder.
  /// var textureFolder = KspPaths.NormalizePath(Path.GetDirectoryName(relPath) + "/../Textures");
  /// // Get a texture from 'Textures' folder that lives in the mods's root.
  /// var texture = GameDatabase.Instance.GetTexture(textureFolder + "/MyTexture.png", false);
  /// ]]></code>
  /// </example>
  /// <seealso cref="NormalizePath"/>
  public static string MakeRelativePathToGameData(params string[] pathParts) {
    var path = string.Join("" + Path.DirectorySeparatorChar, pathParts);
    if (!Path.IsPathRooted(path)) {
      path = Path.GetFullPath(new Uri(Path.Combine(pluginsRoot, path)).LocalPath);
    }
    var rootUri = new Uri(pluginsRoot);
    return rootUri.MakeRelativeUri(new Uri(path))
        .ToString()
        .Replace(Path.DirectorySeparatorChar, '/');
  }
}

}  // namespace
