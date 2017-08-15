// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.ConfigUtils {

/// <summary>A proto for handling all KSP specific types.</summary>
public class KspTypesProto : AbstractOrdinaryValueTypeProto {
  /// <inheritdoc/>
  public override bool CanHandle(Type type) {
    return type == typeof(Color) || type == typeof(Color32)
        || type == typeof(Vector2)
        || type == typeof(Vector3) || type == typeof(Vector3d)
        || type == typeof(Vector4)
        || type == typeof(Quaternion) || type == typeof(QuaternionD)
        || type == typeof(Matrix4x4)
        || typeof(IPersistentField).IsAssignableFrom(type)
        || type.IsEnum;
  }
  
  /// <inheritdoc/>
  public override string SerializeToString(object value) {
    var persistent = value as IPersistentField;
    if (persistent != null) {
      return persistent.SerializeToString();
    }
    if (value is Color) {
      return ConfigNode.WriteColor((Color) value);
    }
    if (value is Color32) {
      return ConfigNode.WriteColor((Color32) value);
    }
    if (value is Vector2) {
      return ConfigNode.WriteVector((Vector2) value);
    }
    if (value is Vector3) {
      return ConfigNode.WriteVector((Vector3) value);
    }
    if (value is Vector3d) {
      return ConfigNode.WriteVector((Vector3d) value);
    }
    if (value is Vector4) {
      return ConfigNode.WriteVector((Vector4) value);
    }
    if (value is Quaternion) {
      return ConfigNode.WriteQuaternion((Quaternion) value);
    }
    if (value is QuaternionD) {
      return ConfigNode.WriteQuaternion((QuaternionD) value);
    }
    if (value is Matrix4x4) {
      return ConfigNode.WriteMatrix4x4((Matrix4x4) value);
    }
    if (value.GetType().IsEnum) {
      return ConfigNode.WriteEnum((Enum) value);
    }
    throw new ArgumentException("Unexpected type: " + value.GetType());
  }
  
  /// <inheritdoc/>
  public override object ParseFromString(string value, Type type) {
    try {
      if (typeof(IPersistentField).IsAssignableFrom(type)) {
        var itemValue = Activator.CreateInstance(type);
        ((IPersistentField)itemValue).ParseFromString(value);
        return itemValue;
      }
      if (type == typeof(Color)) {
        return ConfigNode.ParseColor(value);
      }
      if (type == typeof(Color32)) {
        return ConfigNode.ParseColor32(value);
      }
      if (type == typeof(Vector2)) {
        return ConfigNode.ParseVector2(value);
      }
      if (type == typeof(Vector3)) {
        return ConfigNode.ParseVector3(value);
      }
      if (type == typeof(Vector3d)) {
        return ConfigNode.ParseVector3D(value);
      }
      if (type == typeof(Vector4)) {
        return ConfigNode.ParseVector4(value);
      }
      if (type == typeof(Quaternion)) {
        return ConfigNode.ParseQuaternion(value);
      }
      if (type == typeof(QuaternionD)) {
        return ConfigNode.ParseQuaternionD(value);
      }
      if (type == typeof(Matrix4x4)) {
        return ConfigNode.ParseMatrix4x4(value);
      }
      if (type.IsEnum) {
        return ConfigNode.ParseEnum(type, value);
      }
      throw new ArgumentException("Unexpected type: " + type);
    } catch (Exception ex) {
      throw new ArgumentException(ex.Message);
    }
  }
}

}  // namespace
