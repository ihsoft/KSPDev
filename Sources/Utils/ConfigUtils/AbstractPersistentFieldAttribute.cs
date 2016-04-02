// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.
  
using System;
  
namespace KSPDev.ConfigUtils {
  
/// <summary>A base for any persitent field annotation.</summary>
/// <remarks>Descendands must initialize at least <see cref="_ordinaryTypeProto"/> field. If
/// <see cref="_collectionTypeProto"/> is set then the field is considered a persistent
/// collection of values.
/// <para>See more details and examples in <see cref="ConfigAccessor"/> module.</para>
/// </remarks>
public abstract class AbstractPersistentFieldAttribute : Attribute {
  /// <summary>Relative path to the value or node. Case-insensitive.</summary>
  /// <remarks>Absolute path depends on the context.</remarks>
  public readonly string[] path;

  /// <summary>A tag to separate set of fields into different configuration groups.</summary>
  /// <remarks>Group name can be used when reading/writing values via <see cref="ConfigAccessor"/>
  /// to process only a subset of the persistent fields of the class. It's case-insensitive.
  /// </remarks>
  public string group = "";

  /// <summary>
  /// A proto to use to handle types that can be (de)serialized as a simple string.
  /// </summary>
  protected Type _ordinaryTypeProto;

  /// <summary>A proto to treat fields values as a collection of persistent values.</summary>
  protected Type _collectionTypeProto;
  
  protected AbstractPersistentFieldAttribute(string cfgPath) {
    this.path = cfgPath.Split('/');
  }
}

}  // namespace
