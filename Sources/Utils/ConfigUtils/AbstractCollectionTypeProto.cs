// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections;

namespace KSPDev.ConfigUtils {

/// <summary>A base class for a proto of a collection of values.</summary>
/// <remarks>Collection of collections is not supported. Though, descendands may use own
/// (de)serialization approach to handle nested collections.
/// <para>All descendants of this class must implement a constructor which accepts a single
/// argument: the type of the collection. Constructor can throw <see cref="ArgumentException"/>
/// if passed type is unacceptable.</para>
/// </remarks>
/// <example>As a good example of overriding of this class see
/// <see cref="GenericCollectionTypeProto"/>. Though, it tries to be universal and, hence, works
/// via reflection. You don't need to deal with reflections as long as your custom proto used for
/// the fields of known types only.
/// <code>
/// class MyBooleanCollection {
///   public void AddItem(bool itemValue) {
///     // ...some custom code...
///   }
///   public IEnumerable GetMyVeryCustomIterator() {
///     // ...some custom code...
///     return res;
///   }
/// }
///
/// class MyBooleanCollectionProto : AbstractCollectionTypeProto {
///   public MyBooleanCollectionProto() : base(typeof(bool)) {}
///
///   public override Type GetItemType() {
///     return typeof(bool);
///   }
///   public override IEnumerable GetEnumerator(object instance) {
///     return (instance as MyBooleanCollection).GetMyVeryCustomIterator(); 
///   }
///   public override void AddItem(object instance, object item) {
///     (instance as MyBooleanCollection).AddItem((bool) item);
///   }
/// }
/// </code>
/// </example>
/// <seealso cref="ConfigAccessor"/>
/// <seealso cref="AbstractPersistentFieldAttribute"/>
public abstract class AbstractCollectionTypeProto {
  private AbstractCollectionTypeProto() {
    // Disallow default constructor. This class will only be created via reflection.
  }
  
  /// <param name="containerType">A type of the collection (i.e. an immediate field's type).</param>
  protected AbstractCollectionTypeProto(Type containerType) {}

  /// <summary>Returns type of items in the collection.</summary>
  /// <returns>An item type.</returns>
  public abstract Type GetItemType();
  
  /// <summary>Returns enumerable object for the collection.</summary>
  /// <param name="instance">An instance to get the enumerable for.</param>
  /// <returns>An enumerable of objects. Type of the items is determined by the relevant
  /// <see cref="AbstractOrdinaryValueTypeProto"/>.</returns>
  public abstract IEnumerable GetEnumerator(object instance);
  
  /// <summary>Adds an item into the collection.</summary>
  /// <param name="instance">A collection instance to add item into.</param>
  /// <param name="item">An item to add. The item must be of the same type as
  /// <see cref="GetItemType"/> specifies.</param>
  public abstract void AddItem(object instance, object item);
}

}  // namespace
