using System;
using System.Reflection;
using System.Linq;
using KSPDev.LogUtils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace KSPDev.ConfigUtils {

//TODO: add help
//TODO: rename file
public static class PersistentFieldsFactory {
    
  //public readonly List<PersistentField> fields = new List<PersistentField>();

  /// <summary>Determines which field modifiers are to be considered in the traversing.</summary>
  /// <remarks>If the found field modifier is incompatible with the traverse mode then an error is
  /// generated, and the field is ignored.</remarks>
  private enum FieldModifiers {
    /// <summary>Consider both <c>static</c> and <c>non-static</c> fields.</summary>
    ANY,
    /// <summary>Consider only <c>static</c> and ignore <c>non-static</c> fields.</summary>
    STATIC_ONLY,
    /// <summary>Consider only <c>non-static</c> and ignore <c>static</c> fields.</summary>
    INSTANCE_ONLY,
  }

  public static List<PersistentField> GetPersistentFields(object obj, string group = null) {
    return GetPersistentFields(obj.GetType(), FieldModifiers.ANY, group);
  }

  public static List<PersistentField> GetPersistentFields(Type type, string group = null) {
    return GetPersistentFields(type, FieldModifiers.STATIC_ONLY, group);
  }

  private static List<PersistentField> GetPersistentFields(
      Type type, FieldModifiers modifiers, string group) {
    var result = new List<PersistentField>();

    //UNDONE
    Logger.logWarning("Looking ro fields of group {0} in type {1}", group, type);
    var fieldsInfo = FindAnnotatedFields(type, modifiers, group);
    //UNDONE
    Logger.logWarning("Found {0} fields for {1}", fieldsInfo.Count(), type);

    foreach (var fieldInfo in fieldsInfo) {
      //UNDONE
      Logger.logWarning("Handling {0}.{1}...", type.FullName, fieldInfo.Name);

      var fieldAttr =
          fieldInfo.GetCustomAttributes(typeof(PersistentFieldAttribute), true).First()
          as PersistentFieldAttribute;

      // FIXME: pass annotation instead of cfg path.
      var persistentField = new PersistentField(fieldInfo, fieldAttr.path);
      
      // Determine if field has value handler annotation.
      try {
        var valueType = persistentField.fieldInfo.FieldType;
          
        // If field is repeatable then create the appropriate handler.
        if (fieldAttr.isRepeatable) {
          persistentField.repeatableFieldHandler =
              (RepeatableFieldHandler) Activator.CreateInstance(fieldAttr.repetableHandler,
                                                                new object[] { persistentField });
          valueType = persistentField.repeatableFieldHandler.GetItemType();
        }

        // Create value handler.
        var valueHandler = Activator.CreateInstance(fieldAttr.valueHandler,
                                                    new object[] { persistentField });
        if (fieldAttr.isCompound) {
          persistentField.compoundValueHandler = (CompoundValueHandler) valueHandler;
          // Read non-static fields from the compound type. Ignore static fields of the type since
          // it can be used by multiple persistent fields or as an item in a repeated field.          
          persistentField.compoundTypeFields =
              GetPersistentFields(valueType, FieldModifiers.INSTANCE_ONLY, group).ToArray();
        } else {
          persistentField.simpleValueHandler = (SimpleValueHandler) valueHandler;
        }
      } catch (Exception ex) {
        Logger.logError("Ignoring field {0}.{1}: {2}", type.FullName, fieldInfo.Name, ex.Message);
        continue;
      }
      result.Add(persistentField);
    }

    return result;
  }

  private static IEnumerable<FieldInfo> FindAnnotatedFields(
      IReflect type, FieldModifiers modifiers = FieldModifiers.ANY,
      string group = null) {
    // Don't consider inherited fields.  
    BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
    if (modifiers == FieldModifiers.ANY || modifiers == FieldModifiers.STATIC_ONLY) {
      flags |= BindingFlags.Static;
    }
    if (modifiers == FieldModifiers.ANY || modifiers == FieldModifiers.INSTANCE_ONLY) {
      flags |= BindingFlags.Instance;
    }
    return type.GetFields(flags).Where(f => GroupFilter(f, group));
  }

  private static bool GroupFilter(ICustomAttributeProvider fieldInfo, string group) {
    // We need descendants of PersistentFieldAttribute as well.
    var attributes = fieldInfo.GetCustomAttributes(typeof(PersistentFieldAttribute), true);
    if (attributes.Length == 0) {
      return false;
    }
    var annotation = attributes[0] as PersistentFieldAttribute;
    return group == null || annotation.group.Equals(group);
  }
}

}  // namespace
