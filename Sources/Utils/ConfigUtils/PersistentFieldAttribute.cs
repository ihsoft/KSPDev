// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>A simple attribute for fields that need (de)serialization.</summary>
/// <remarks>
/// This form allows adjusting any <see cref="AbstractPersistentFieldAttribute"/> property
/// in the annotation, and has a shortcut to mark a field as collection
/// (<c><see cref="isCollection"/> = true</c>).
/// <para>
/// By default ordinal values are handled via <see cref="StandardOrdinaryTypesProto"/>
/// and collection fields via <see cref="GenericCollectionTypeProto"/>. These proto handlers can
/// be changed in the annotation by assigning values to properties
/// <see cref="ordinaryTypeProto"/> and/or <see cref="collectionTypeProto"/>.
/// </para>
/// </remarks>
/// <example>
/// Below is a simple usage of the attribute.
/// <code><![CDATA[
/// class ClassWithPersistentFields {
///   [PersistentField("my/listField", isCollection = true)]
///   private List<string> sampleList = new List<string>();
/// 
///   internal struct ComplexType {
///     [PersistentField("val1", group = "nevermind")]
///     public bool boolVal;
///     [PersistentField("val2")]
///     public Color colorVal;
///   }
/// 
///   [PersistentField("my/setField", isCollection = true, group = "abc")]
///   private HashSet<ComplexType> sampleSet = new HashSet<ComplexType>();
/// 
///   void SaveConfigs() {
///     // Save a default group of fields: only field "sampleList" qualifies.
///     ConfigAccessor.WriteFieldsIntoFile("settings.cfg", instance: this);
///     /* The following structure in the file will be created:
///      * {
///      *   my
///      *   {
///      *     listField: string1
///      *     listField: string2
///      *   }
///      * }
///      */
/// 
///     // Save a specific group of fields: only field "sampleSet" belongs to group "abc".
///     sampleSet.Add(new ComplexType() { boolVal = true, colorVal = Color.black });
///     sampleSet.Add(new ComplexType() { boolVal = false, colorVal = Color.white });
///     ConfigAccessor.WriteFieldsIntoFile("settings.cfg", instance: this, group: "abc");
///     /* The following structure in the file will be created:
///      * {
///      *     setField
///      *     {
///      *       val1: True
///      *       val2: 0,0,0,1
///      *     }
///      *     setField
///      *     {
///      *       val1: false
///      *       val2: 1,1,1,1
///      *     }
///      *   }
///      * }
///      */
///   }
/// }
/// ]]></code>
/// <para>
/// Note that group is ignored in the nested types. I.e. in <c>ComplexType</c> in this case. Though,
/// if <c>ComplexType</c> was an immediate target of the <c>WriteFieldsIntoFile</c> call then the
/// group would be considered.
/// </para>
/// <para>
/// Visibility of the annotated field is also important. Persistent field attributes are only
/// visible in the child class if they were public or protected in the parent. Private field
/// annotations are not inherited and need to be handled at the level of the declaring class.
/// </para>
/// <code><![CDATA[
/// class Parent {
///   [PersistentField("parent_private")]
///   private int field1 = 1;
/// 
///   [PersistentField("parent_protected")]
///   protected int field2 = 2;
/// 
///   [PersistentField("parent_public")]
///   public int field3 = 3;
/// }
/// 
/// class Child : Parent {
///   [PersistentField("child_private")]
///   private int field1 = 10;
/// 
///   void SaveConfig() {
///     // Save all fields in the inherited type. 
///     ConfigAccessor.WriteFieldsIntoFile("settings.cfg", instance: this);
///     /* The following structure in the file will be created:
///      * {
///      *     parent_protected: 2
///      *     parent_public: 3
///      *     child_private: 10
///      * }
///      */
/// 
///     // Save all fields in the base type. 
///     ConfigAccessor.WriteFieldsIntoFile("settings.cfg", instance: (Parent) this);
///     /* The following structure in the file will be created:
///      * {
///      *     parent_private: 1
///      *     parent_protected: 2
///      *     parent_public: 3
///      * }
///      */
///   }
/// }
/// ]]></code>
/// <para>
/// The code above implies that in a common case unsealed class should put private fields in a group
/// other than default to avoid settings collision.
/// </para>
/// <para>
/// When type of the field is different from primitive C# type or common Unity 4 type you may need
/// provide custom value handlers to deal with (de)serializing. E.g. for an ordinary type it may
/// look like this:
/// </para>
/// <code><![CDATA[
/// class CustomTypes {
///   [PersistentField("my/custom/type", ordinaryTypeProto = typeof(MyTypeProto))]
///   private MyType field1;
/// }
/// ]]></code>
/// <para>
/// If your custom type is a collection that cannot be handled by the standard proto you can provide
/// your own collection proto handler. Note that if you do then annotated field will be treated as a
/// collection. In fact, when you set <c>isCollection = true</c> what actually happens is just
/// assigning <see cref="GenericCollectionTypeProto"/> as a collection proto handler.
/// </para>
/// <code><![CDATA[
/// class CustomTypes {
///   [PersistentField("my/custom/type", collectionTypeProto = typeof(MyCollectionProto))]
///   private MyCollection field1;
/// ]]>}
/// </code>
/// For more examples on custom proto handlers see <see cref="AbstractOrdinaryValueTypeProto"/> and
/// <see cref="AbstractCollectionTypeProto"/>.
/// </example>
/// <seealso cref="ConfigAccessor"/>
/// <seealso cref="AbstractOrdinaryValueTypeProto"/>
/// <seealso cref="AbstractCollectionTypeProto"/>
[AttributeUsage(AttributeTargets.Field)]
public sealed class PersistentFieldAttribute : AbstractPersistentFieldAttribute {
  /// <summary>A proto for handling the annotated field.</summary>
  /// <remarks>If field is a collection then this proto will be used to handle values of the
  /// collection items.</remarks>
  public Type ordinaryTypeProto {
    set { _ordinaryTypeProto = value; }
    get { return _ordinaryTypeProto; }
  }

  /// <summary>A proto for handling collection fields.</summary>
  /// <remarks>If this property is not <c>null</c> than the annotated field is trreated as a
  /// collection of values.</remarks>
  public Type collectionTypeProto {
    set { _collectionTypeProto = value; }
    get { return _collectionTypeProto; }
  }

  /// <summary>Specifies if the annotated field is a collection of values.</summary>
  public bool isCollection {
    set { collectionTypeProto = value ? typeof(GenericCollectionTypeProto) : null; }
    get { return collectionTypeProto != null; }
  }

  /// <inheritdoc/>
  public PersistentFieldAttribute(string cfgPath) : base(cfgPath) {
    ordinaryTypeProto = typeof(StandardOrdinaryTypesProto);
    isCollection = false;
  }
}

}  // namespace
