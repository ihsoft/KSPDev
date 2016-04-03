// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>
/// Simple persitent fields file annotation that requires all arguments to be set in constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class PersistentFieldsFileAttribute : AbstractPersitentFieldsFileAttribute {
  public PersistentFieldsFileAttribute(string configFilePath, string nodePath,
                                       string group = StdPersistentGroups.Default)
      : base(configFilePath, nodePath, group) {
  }
}

}  // namespace
