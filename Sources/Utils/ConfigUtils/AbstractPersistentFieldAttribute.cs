// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.
  
using System;
  
namespace KSPDev.ConfigUtils {
  
/// <summary>A base for any persitent field annotation.</summary>
/// <remarks>Descendands must initialize at least <see cref="_ordinaryTypeProto"/> field. If
/// <see cref="_collectionTypeProto"/> is set then the field is considered a persistent
/// collection of values.
/// <para>See more examples in <see cref="PersistentFieldsFileAttribute"/>.</para>
/// </remarks>
/// <example>A "shortcut" attributes could be declared like this: 
/// <code>
/// class MyTypeAttribute : AbstractPersistentFieldAttribute {
///   public MyTypeAttribute(string cfgPath) : base(cfgPath) {
///     _ordinaryTypeProto = typeof(PrimitiveTypesProto);
///   }
/// }
///
/// class MyTypesCollectionAttribute : AbstractPersistentFieldAttribute {
///   public MyTypesCollectionAttribute(string cfgPath) : base(cfgPath) {
///     _ordinaryTypeProto = typeof(PrimitiveTypesProto);
///     _collectionTypeProto = typeof(GenericCollectionTypeProto);
///   }
/// }
/// </code>
/// </example>
/// <seealso cref="AbstractOrdinaryValueTypeProto"/>
/// <seealso cref="AbstractCollectionTypeProto"/>
/// <seealso cref="ConfigAccessor"/>
public abstract class AbstractPersistentFieldAttribute : Attribute {
  /// <summary>Relative path to the value or node. Case-insensitive.</summary>
  /// <remarks>Absolute path depends on the context.</remarks>
  public readonly string[] path;

  /// <summary>A tag for a group of fields.</summary>
  /// <remarks>Group can be used when reading/writing values via <see cref="ConfigAccessor"/>
  /// to process only a subset of the persistent fields of the class. It's case-insensitive.
  /// </remarks>
  public string group = "";

  /// <summary>A proto that (de)serializes field's value as a simple string.</summary>
  /// <remarks>This type must be a descendant of <see cref="AbstractOrdinaryValueTypeProto"/>.
  /// </remarks>
  protected Type _ordinaryTypeProto;

  /// <summary>A proto that handles field's value as a collection of persistent values.</summary>
  /// <remarks>If it's <c>null</c> then field is assumed to be not a collection.</remarks>
  /// <para>This type must be a descendant of <see cref="AbstractCollectionTypeProto"/>.
  /// </para>
  protected Type _collectionTypeProto;

  /// <param name="cfgPath">A path to the fields's value in the config. Components must be separated
  /// by symbol '/'. The path is relative, the absolute path is determined when doing actual
  /// (de)serialization. The path is case-insensitive.</param>
  protected AbstractPersistentFieldAttribute(string cfgPath) {
    this.path = cfgPath.Split('/');
  }
}

}  // namespace
