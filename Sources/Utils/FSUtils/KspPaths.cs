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

  /// <summary>A standard directory name to place the mod's binary.</summary>
  /// <remarks>
  /// This name is not mandatory, and is not enforced by the game's core. It's a community adopted
  /// name.
  /// </remarks>
  public static readonly string PluginFolderName = "Plugins";

  /// <summary>
  /// A standard name to place the configs that should be ignored by the game's core.
  /// </summary>
  /// <remarks>
  /// The files in this directory will be ignored by the game during the loading process. This
  /// is a common palce to put tghe mod's settings. A directory with such name can be placed
  /// anywhere within the <c>GameData</c> folder. However, it's usually a subfolder of
  /// <see cref="PluginFolderName"/>.
  /// </remarks>
  public static readonly string PluginDataFolderName = "PluginData";

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

  /// <summary>Returns a relative game's path to the mod's root folder.</summary>
  /// <example>
  /// <para>
  /// Given the mod's assembly was loaded from
  /// <c>GameData/ModFolder1/ModFolder2/Plugins/mod.dll</c>, the returned path will be
  /// <c>GameData/ModFolder1/ModFolder2/</c> because of <c>Plugins</c> folder name is considered to
  /// be a common name for the mod's  binaries.
  /// </para>
  /// <para>
  /// If the mod's DLL is located in the folder other than <c>Plugins</c>, then just the parent
  /// folder is returned. E.g. for <c>GameData/ModFolder1/ModFolder2/MyDLLs/mod.dll</c>, the result
  /// would be <c>GameData/ModFolder1/ModFolder2/MyDLLs/</c>.
  /// </para>
  /// </example>
  /// <param name="target">The target to resolve the assembly for.</param>
  /// <returns>
  /// An absolute path. There is always a trailing directory separator symbol.
  /// </returns>
  public static string GetModsPath(Type target) {
    var parent = Path.GetDirectoryName(target.Assembly.Location);
    if (parent.EndsWith(PluginFolderName, StringComparison.Ordinal)) {
      parent = Path.GetDirectoryName(parent);  // It will just chomp of the last folder name.
    }
    return parent + Path.DirectorySeparatorChar;
  }

  /// <inheritdoc cref="GetModsPath(Type)"/>
  public static string GetModsPath(object target) {
    return GetModsPath(target.GetType());
  }

  /// <summary>Returns a relative game's path to the file located in the data folder.</summary>
  /// <param name="obj">The object instance to use to resolve the mod's assembly.</param>
  /// <param name="fileName">The data file name.</param>
  /// <param name="createMissingDirs">
  /// Instructs the method to create all the directories and subdirectories in the specified path,
  /// should they not already exist.
  /// </param>
  /// <param name="subFolder">The optional sub-folder name to add to the path.</param>
  /// <returns>An absolute path.</returns>
  public static string GetModsDataFilePath(object obj, string fileName,
                                           bool createMissingDirs = false,
                                           string subFolder = null) {
    var dataDirectory = Path.Combine(GetModsPath(obj), PluginDataFolderName);
    if (subFolder != null) {
      dataDirectory = Path.Combine(dataDirectory, subFolder);
    }
    Directory.CreateDirectory(dataDirectory);
    return Path.Combine(dataDirectory, fileName);
  }
}

}  // namespace
