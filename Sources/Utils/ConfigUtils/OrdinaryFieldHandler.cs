// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using KSPDev.LogUtils;

namespace KSPDev.ConfigUtils {

/// <summary>
/// A handler that manages ordinary fields. All type specific handling is done via a proto.
/// </summary>
internal sealed class OrdinaryFieldHandler {
  public readonly Type valueType;
  public readonly PersistentField persistentField;
  
  private readonly AbstractOrdinaryValueTypeProto simpleTypeProto;
    
  public OrdinaryFieldHandler(
      PersistentField persistentField, Type valueType, Type simpleTypeProtoType) {
    this.valueType = valueType;
    this.persistentField = persistentField;
    this.simpleTypeProto = Activator.CreateInstance(simpleTypeProtoType)
        as AbstractOrdinaryValueTypeProto;
  }

  /// <summary>Converts field value into a form suitable for storing into config file.</summary>
  /// <remarks>Values that can be handled by <see cref="simpleTypeProto"/> are transformed into
  /// a simple strings, and saved as string values into the config. Structs and classes are considred
  /// "compound types" (see <see cref="IsCompound()"/>), i.e. types that have nested fields in
  /// them. Such types are converted into a config node.</remarks>
  /// <param name="value">A field's value to convert.</param>
  /// <returns>String or <see cref="ConfigNode"/>.</returns>
  public object SerializeValue(object value) {
    if (simpleTypeProto.CanHandle(valueType)) {
      return simpleTypeProto.SerializeToString(value);
    }
    if (IsCompound()) {
      return persistentField.SerializeCompoundFieldsToNode(value);
    }
    Logger.logError("{0} doesn't know how to store value type: {1}", GetType(), valueType);
    return null;
  }

  /// <summary>Converts a value from config into an actual fiel's value.</summary>
  /// <param name="cfgValue">A string if value is handled by <see cref="simpleTypeProto"/> or
  /// <see cref="ConfigNode"/> if type is compound.</param>
  /// <returns>Value of <see cref="valueType"/> type.</returns>
  public object DeserializeValue(object cfgValue) {
    if (simpleTypeProto.CanHandle(valueType)) {
      try {
        return simpleTypeProto.ParseFromString((string) cfgValue, valueType);
      } catch (ArgumentException ex) {
        Logger.logError(string.Format(
            "Cannot parse value \"{0}\" as {1}: {2}", cfgValue, valueType, ex.Message));
        return null;
      }
    }
    if (IsCompound()) {
      var instance = Activator.CreateInstance(valueType);
      persistentField.DeserializeCompoundFieldsFromNode((ConfigNode) cfgValue, instance);
      return instance;
    }
    Logger.logError("{0} doesn't know how to parse value type: {1}", GetType(), valueType);
    return null;
  }

  /// <summary>Determines if the field is complex type consiting of more fields.</summary>
  /// <returns><c>true</c> if type can have nested persitent fields.</returns>
  public bool IsCompound() {
    return !simpleTypeProto.CanHandle(valueType) &&
        (valueType.IsValueType && !valueType.IsEnum  // IsStruct
         || valueType.IsClass && valueType != typeof(string) && !valueType.IsEnum);  // IsClass
  }
}

}  // namespace
