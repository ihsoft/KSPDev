// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Reflection;
using System.Collections;

namespace KSPDev.ConfigUtils {

/// <summary>A proto for a field with simple generic collection.</summary>
/// <remarks>Generic must have exactly one arguent, implement method <c>Add</c> for adding new
/// items, and implement <see cref="IEnumerable"/>.</remarks>
public sealed class GenericCollectionTypeProto : AbstractCollectionTypeProto {
  private readonly Type itemType;
  private readonly MethodInfo addMethod;
    
  public GenericCollectionTypeProto(Type containerType) : base(containerType) {
    if (!containerType.IsGenericType || containerType.GetGenericArguments().Length != 1) {
      throw new ArgumentException(string.Format(
          "{0} requires generic container type as field value but found: {1}",
          GetType(), containerType));
    }
    itemType = containerType.GetGenericArguments()[0];

    addMethod = containerType.GetMethod("Add");
    if (addMethod == null) {
      throw new ArgumentException(string.Format(
          "Type {0} doesn't have Add() method", containerType));
    }
  }
    
  public override Type GetItemType() {
    return itemType;
  }
  
  public override IEnumerable GetEnumerator(object instance) {
    return instance as IEnumerable;
  }
  
  public override void AddItem(object instance, object item) {
    addMethod.Invoke(instance, new[] {item});
  }
}

}  // namespace
