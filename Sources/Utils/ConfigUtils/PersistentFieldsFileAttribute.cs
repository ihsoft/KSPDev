// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>
/// A simple annotation to associate a persistent group with a configuration file.
/// </summary>
/// <remarks>Multiple annotations are allowed on the class. It's not required to have unique
/// filename/nodePath/group set in every annontation but it's highly recommended.  
/// <para>This assignment is ignored in the nested classes. Though, when using nested class as an
/// immediate target of the call the annotation will be handled just fine.</para>
/// </remarks>
/// <example>
/// <code>
/// using KSPDev.ConfigUtils;
///
/// [PersistentFieldsFile("settings.cfg", "Root/Default")]
/// [PersistentFieldsFile("settings-other.cfg", "", "abc")]
/// [PersistentFieldsFile("settings-nested-bad.cfg", "", "nevermind")]
/// class ClassWithPersistentFields {
///   [PersistentField("field1")]
///   private int intField = 0;
/// 
///   [PersistentFieldsFile("settings-nested-good.cfg", "Root/Nested", "nevermind")]
///   internal struct ComplexType {
///     [PersistentField("val1", group = "nevermind)]
///     private bool boolVal;
///     [PersistentField("val2", group = "nevermind)]
///     private Color colorVal = Color.white;
///   }
/// 
///   [PersistentField("complexField1", group = "abc")]
///   private ComplexType complexField;
/// 
///   void SaveFields() {
///     // Save a default group of fields. 
///     ConfigAccessor.WriteFieldsFromType(instance: this);
///     /* File will be created at "GameData/settings.cfg":
///      * Root
///      * {
///      *   Default
///      *   {
///      *     field1: 0
///      *   }
///      * }
///      */
/// 
///     // Save group "abc". Note that the complex type only defines fields for group "nevermind"
///     // but it's ignored. The group is only honored on the immediate type, i.e.
///     // ClassWithPersistentFields in this case. 
///     ConfigAccessor.WriteFieldsFromType(instance: this, group: "abc");
///     /* File will be created at "GameData/settings-other.cfg".
///      * {
///      *   complexField1
///      *   {
///      *     val1: False
///      *     val2: 1,1,1,1  // It's an RGBA format.
///      *   }
///      * }
///      */
/// 
///     // Try to use incorrect setup and save group "nevermind" for "this". 
///     // File will be created at "GameData/settings-nested-bad.cfg". And it will be empty since
///     // no fields for this group is defined in class ClassWithPersistentFields.
///     ConfigAccessor.WriteFieldsFromType(instance: this, group: "nevermind");
/// 
///     // Proper use of the nested complex type would be like this.
///     var test = new ComplexType() {
///         boolVal = true,
///         colorVal = Color.black
///     };
///     ConfigAccessor.WriteFieldsFromType(instance: test, group: "nevermind");
///     /* File will be created at "GameData/settings-nested-good.cfg".
///      * Root
///      * {
///      *   Nested
///      *   {
///      *     complexField1
///      *     {
///      *       val1: True
///      *       val2: 0,0,0,1  // It's an RGBA format.
///      *     }
///      *   }
///      * }
///      */
///
///     // The following call makes the similar output but with different values.    
///     ConfigAccessor.WriteFieldsFromType(instance: complexField, group: "nevermind");
///   }
/// }
/// </code>
/// Note that this annotation only adds or re-creates the node specified by <c>nodePath</c>. If
/// target file had other nodes they will not be overwritten. Though, you may expect the file
/// structure to be re-ordered and comments (if any) lost. The file is actualy changed, it's read,
/// updated, and saved.
/// </example>
/// <seealso cref="ConfigAccessor.ReadFieldsInType"/>
/// <seealso cref="ConfigAccessor.WriteFieldsFromType"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class PersistentFieldsFileAttribute : AbstractPersistentFieldsFileAttribute {
  /// <inheritdoc/>
  public PersistentFieldsFileAttribute(string configFilePath, string nodePath,
                                       string group = StdPersistentGroups.Default)
      : base(configFilePath, nodePath, group) {
  }
}

}  // namespace
