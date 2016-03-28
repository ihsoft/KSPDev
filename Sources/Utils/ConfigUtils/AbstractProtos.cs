// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections;

namespace KSPDev.ConfigUtils {

/// <summary>A base class for a proto of a single value.</summary>
/// <remarks>All descendands of this class must implement a default constructor.</remarks>
public abstract class AbstractOrdinaryValueTypeProto {
  /// <summary>Tells if proto can handle the specified type.</summary>
  /// <param name="type">A type in question.</param>
  /// <returns><c>true</c> if proto can (de)serialize values of the type.</returns>
  public abstract bool CanHandle(Type type);

  /// <summary>Serializes <paramref name="value"/> into a string.</summary>
  /// <remarks>In general avoid using <c>ToString()</c> methods to produce the serialized value.
  /// Such methods are not designed to be unambiguous.</remarks>
  /// <param name="value">A value to serialize.</param>
  /// <returns>A string representation of the vlaue. It doesn't need to be human readable.</returns>
  public abstract string SerializeToString(object value);

  /// <summary>Makes a vlaue from the string representation.</summary>
  /// <param name="value">A string produced by <seealso cref="SerializeToString"/>.</param>
  /// <param name="type">A type to convert the value into.</param>
  /// <returns>A new and initialized instance of the requested type.</returns>
  /// <exception cref="ArgumentException">If value cannot be parsed.</exception>
  public abstract object ParseFromString(string value, Type type);
}
  
/// <summary>A base class for a proto of a collection of values.</summary>
/// <remarks>Collection of collections is not supported. Though, descendands may use own
/// (de)serialization approach to handle nested collections.
/// <para>All descendants of this class must implement a constructor which accepts a single
/// argument: the type of the collection. Constructor can throw <seealso cref="ArgumentException"/>
/// if passed type is unacceptable.</para>
/// </remarks>
public abstract class AbstractCollectionTypeProto {
  private AbstractCollectionTypeProto() {}  // Disallow default constructor.
  protected AbstractCollectionTypeProto(Type containerType) {}

  /// <summary>Returns type of items in the collection.</summary>
  /// <returns>An item type.</returns>
  public abstract Type GetItemType();
  
  /// <summary>Returns enumerable object for the repeated field.</summary>
  /// <param name="instance">An instance to get the enumerable for.</param>
  /// <returns>An enumerable of objects. Type of the items is determined by the relevant
  /// <seealso cref="AbstractOrdinaryValueTypeProto"/></returns>
  public abstract IEnumerable GetEnumerator(object instance);
  
  /// <summary>Adds an item into the collection.</summary>
  /// <param name="instance">A collection instance to add item into.</param>
  /// <param name="item">An item to add. The item must be of the same type as
  /// <seealso cref="GetItemType"/> specifies.</param>
  public abstract void AddItem(object instance, object item);
}

}  // namespace
