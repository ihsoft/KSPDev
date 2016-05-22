// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>A base for any persistent fields file annotation.</summary>
/// <remarks>This attribute doesn't assume much logic so, you basically override it only to create
/// "shortcuts".</remarks>
/// <seealso cref="ConfigAccessor.ReadFieldsInType"/>
/// <seealso cref="ConfigAccessor.WriteFieldsFromType"/>
public abstract class AbstractPersistentFieldsFileAttribute : Attribute {
  /// <summary>A group tag which is handled by this annotation.</summary>
  public readonly string group;

  /// <summary>A path to the node which will be the root for the fields in the group.</summary>
  /// <remarks>By setting different root for every group and/or type you may combine multiple
  /// settings in the same config file. When <see cref="configFilePath"/> is empty this value is an
  /// absolute path on the game's database.</remarks>
  public readonly string nodePath;

  /// <summary>An optional relative path to the config file.</summary>
  /// <remarks>Absolute name is resolved via <see cref="FSUtils.KspPaths.makePluginPath"/>. If left
  /// empty then data is read from the game's database. Note, that database access is read-only.  
  /// </remarks>
  public readonly string configFilePath;

  /// <param name="configFilePath">A relative or an absolute path to the file. It's resolved via
  /// <see cref="FSUtils.KspPaths.makePluginPath"/>. If empty then data is read from database.
  /// </param>
  /// <param name="nodePath">A root for the persistent fields when saving or loading via this
  /// annotation.</param>
  /// <param name="group">A group of the annotation. When saving or loading persistent fields only
  /// the fields of this group will be considered. Must not be <c>null</c>.</param>
  protected AbstractPersistentFieldsFileAttribute(
      string configFilePath, string nodePath, string group) {
    this.configFilePath = configFilePath;
    this.nodePath = nodePath;
    this.group = group;
  }
}

}  // namespace
