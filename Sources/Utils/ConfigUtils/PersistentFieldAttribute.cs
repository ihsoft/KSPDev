// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>A base for any persitent field annotation.</summary>
/// <remarks>Descendands must initialize at least <see cref="_ordinaryValueProto"/> field. If
/// <see cref="_repeatedValueProto"/> is set then the field is considered a persistent
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
  protected Type _ordinaryValueProto;

  /// <summary>A proto to treat fields values as a collection of persistent values.</summary>
  protected Type _repeatedValueProto;
  
  protected AbstractPersistentFieldAttribute(string cfgPath) {
    this.path = cfgPath.Split('/');
  }
}

/// <summary>An attribute for fields that needs (de)serialization.</summary>
/// <remarks>
/// This form allows adjusting any <see cref="AbstractPersistentFieldAttribute"/> property
/// in the annotation, and has a shortcut to mark field as repeatable
/// (<c><see cref="isRepeatable"/> = true</c>).
/// <para> By default ordial values are handled via <see cref="StandardOrdinaryTypesProto"/>
/// and repeated fields via <see cref="GenericCollectionTypeProto"/>. These proto handlers can
/// be changed in the annotation by assigning values to properties
/// <see cref="ordinaryValueProto"/> and/or <see cref="repeatedValueProto"/>.</para>
/// <para>See more details and examples in <see cref="ConfigAccessor"/> module.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class PersistentFieldAttribute : AbstractPersistentFieldAttribute {
  public Type ordinaryValueProto {
    set { _ordinaryValueProto = value; }
    get { return _ordinaryValueProto; }
  }
  public Type repeatedValueProto {
    set { _repeatedValueProto = value; }
    get { return _repeatedValueProto; }
  }
  public bool isRepeatable {
    set { repeatedValueProto = value ? typeof(GenericCollectionTypeProto) : null; }
    get { return repeatedValueProto != null; }
  }
  
  public PersistentFieldAttribute(string cfgPath) : base(cfgPath) {
    ordinaryValueProto = typeof(StandardOrdinaryTypesProto);
    isRepeatable = false;
  }
}

}  // namespace
