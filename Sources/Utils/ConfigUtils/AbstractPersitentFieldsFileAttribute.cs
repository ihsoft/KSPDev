// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>A base for any persistent fields file annotation.</summary>
/// <remarks>See more details and examples in <see cref="ConfigAccessor"/> module.</remarks>
public abstract class AbstractPersitentFieldsFileAttribute : Attribute {
  /// <summary>A group tag which will be handled by this annotation.</summary>
  public readonly string group;

  /// <summary>A path to the node which will be the root for the fields in the group.</summary>
  /// <remarks>By setting different root for every group and/or type you may combine multiple
  /// settings in the same config file.</remarks>
  public readonly string[] nodePath;

  /// <summary>Relative path to the config file.</summary>
  /// <remarks>The path is relative to the game's "GameData" folder.</remarks>
  public readonly string configFilePath;

  /// <param name="configFilePath">A relative or an absolute path to the file. It's resolved via
  /// <see cref="FSUtils.KspPaths.makePluginPath"/>.</param>
  /// <param name="nodePath">A root for the persistent fields when saving or loading via this
  /// annotation.</param>
  /// <param name="group">A group of the annotation. When saving or loading persistent fields only
  /// the fields of this group will be considered. Must not be <c>null</c>.</param>
  protected AbstractPersitentFieldsFileAttribute(
      string configFilePath, string nodePath, string group) {
    this.configFilePath = configFilePath;
    this.nodePath = nodePath.Split('/');
    this.group = group;
  }
}

}  // namespace
