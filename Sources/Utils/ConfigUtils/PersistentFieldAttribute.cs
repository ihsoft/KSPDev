// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {
  
/// <summary>An annotation for fields that needs (de)serialization.</summary>
/// <remarks>See help topics and examples in <seealso cref="ConfigReader"/>.</remarks>
[AttributeUsage(AttributeTargets.Field)]
public class PersistentFieldAttribute : Attribute {
  public readonly string[] cfgPath;
  public readonly string cfgGroup;
  public readonly bool isRepeatable;
  
  public PersistentFieldAttribute(string cfgPath, string group = null, bool repeatable = false) {
    this.cfgPath = cfgPath.Split('/');
    this.cfgGroup = group;
    this.isRepeatable = repeatable;
  }
}
  
}  // namespace
