// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.ConfigUtils {

/// <summary>
/// A handler that manages ordinary fields. All type specific handling is done via a proto.
/// </summary>
sealed class OrdinaryFieldHandler {
  readonly PersistentField persistentField;
  readonly Type valueType;
  readonly AbstractOrdinaryValueTypeProto simpleTypeProto;

  /// <param name="persistentField">Persitent field descriptor.</param>
  /// <param name="valueType">
  /// Type to handle. If field is a collection then this type is a type of the collection's item.
  /// </param>
  /// <param name="simpleTypeProtoType">
  /// Proto that handles (de)serializing (in)to a simple string. If this proto cannot handle
  /// <paramref name="valueType"/> then the type will be attempted to be handled as a complex type.
  /// </param>
  internal OrdinaryFieldHandler(
      PersistentField persistentField, Type valueType, Type simpleTypeProtoType) {
    this.persistentField = persistentField;
    this.valueType = valueType;
    this.simpleTypeProto = Activator.CreateInstance(simpleTypeProtoType)
        as AbstractOrdinaryValueTypeProto;
  }

  /// <summary>Converts field value into a form suitable for storing into config file.</summary>
  /// <remarks>
  /// Values that can be handled by the proto are transformed into simple strings, and saved as
  /// string values into the config. Structs and classes are considred "compound types" (see
  /// <see cref="IsCompound()"/>), i.e. types that have nested fields in them. Such types are
  /// converted into a config node.
  /// </remarks>
  /// <param name="value">Field's value to convert.</param>
  /// <returns>String or <see cref="ConfigNode"/>.</returns>
  internal object SerializeValue(object value) {
    if (simpleTypeProto.CanHandle(valueType)) {
      return simpleTypeProto.SerializeToString(value);
    }
    if (IsCompound()) {
      return persistentField.SerializeCompoundFieldsToNode(value);
    }
    Debug.LogErrorFormat("{0} doesn't know how to store value type: {1}", GetType(), valueType);
    return null;
  }

  /// <summary>Converts a value from config into an actual fiel's value.</summary>
  /// <param name="cfgValue">
  /// String if value can be handled by the proto or <see cref="ConfigNode"/> if type is compound.
  /// </param>
  /// <returns>Field's value.</returns>
  internal object DeserializeValue(object cfgValue) {
    if (simpleTypeProto.CanHandle(valueType)) {
      try {
        return simpleTypeProto.ParseFromString((string) cfgValue, valueType);
      } catch (ArgumentException ex) {
        Debug.LogErrorFormat(
            "Cannot parse value \"{0}\" as {1}: {2}", cfgValue, valueType, ex.Message);
        return null;
      }
    }
    if (IsCompound()) {
      var instance = Activator.CreateInstance(valueType);
      persistentField.DeserializeCompoundFieldsFromNode((ConfigNode) cfgValue, instance);
      return instance;
    }
    Debug.LogErrorFormat("{0} doesn't know how to parse value type: {1}", GetType(), valueType);
    return null;
  }

  /// <summary>Determines if the field is complex type consiting of more fields.</summary>
  /// <returns><c>true</c> if type can have nested persitent fields.</returns>
  internal bool IsCompound() {
    return !simpleTypeProto.CanHandle(valueType) &&
        (valueType.IsValueType && !valueType.IsEnum  // IsStruct
         || valueType.IsClass && valueType != typeof(string) && !valueType.IsEnum);  // IsClass
  }
}

}  // namespace
