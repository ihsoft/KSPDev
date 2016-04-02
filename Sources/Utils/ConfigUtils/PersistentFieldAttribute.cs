// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>An attribute for fields that needs (de)serialization.</summary>
/// <remarks>
/// This form allows adjusting any <see cref="AbstractPersistentFieldAttribute"/> property
/// in the annotation, and has a shortcut to mark field as collection
/// (<c><see cref="isCollection"/> = true</c>).
/// <para> By default ordial values are handled via <see cref="StandardOrdinaryTypesProto"/>
/// and collection fields via <see cref="GenericCollectionTypeProto"/>. These proto handlers can
/// be changed in the annotation by assigning values to properties
/// <see cref="ordinaryTypeProto"/> and/or <see cref="collectionTypeProto"/>.</para>
/// <para>See more details and examples in <see cref="ConfigAccessor"/> module.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class PersistentFieldAttribute : AbstractPersistentFieldAttribute {
  public Type ordinaryTypeProto {
    set { _ordinaryTypeProto = value; }
    get { return _ordinaryTypeProto; }
  }
  public Type collectionTypeProto {
    set { _collectionTypeProto = value; }
    get { return _collectionTypeProto; }
  }
  public bool isCollection {
    set { collectionTypeProto = value ? typeof(GenericCollectionTypeProto) : null; }
    get { return collectionTypeProto != null; }
  }
  
  public PersistentFieldAttribute(string cfgPath) : base(cfgPath) {
    ordinaryTypeProto = typeof(StandardOrdinaryTypesProto);
    isCollection = false;
  }
}

}  // namespace
