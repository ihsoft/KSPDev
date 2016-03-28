// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.ComponentModel;

namespace KSPDev.ConfigUtils {

/// <summary>A proto for handling C# primitive types.</summary>
public class PrimitiveTypesProto : AbstractOrdinaryValueTypeProto {
  public override bool CanHandle(Type type) {
    return type.IsPrimitive || type.IsEnum || type == typeof(string);
  }
  
  public override string SerializeToString(object value) {
    return value.ToString();
  }
  
  public override object ParseFromString(string value, Type type) {
    try {
      return TypeDescriptor.GetConverter(type).ConvertFromString(value);
    } catch (Exception ex) {
      throw new ArgumentException(ex.Message);
    }
  }
}

}  // namespace
