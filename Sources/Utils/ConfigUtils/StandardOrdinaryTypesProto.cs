// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>Proto to handle all primitive C# types and common Unity types.</summary>
internal sealed class StandardOrdinaryTypesProto : AbstractOrdinaryValueTypeProto {
  private static readonly PrimitiveTypesProto primitiveTypesProto = new PrimitiveTypesProto();
  private static readonly KspTypesProto unityTypesProto = new KspTypesProto();

  /// <inheritdoc/>
  public override bool CanHandle(Type type) {
    return primitiveTypesProto.CanHandle(type) || unityTypesProto.CanHandle(type);
  }

  /// <inheritdoc/>
  public override string SerializeToString(object value) {
    return primitiveTypesProto.CanHandle(value.GetType())
        ? primitiveTypesProto.SerializeToString(value)
        : unityTypesProto.SerializeToString(value);
  }
  
  /// <inheritdoc/>
  public override object ParseFromString(string value, Type type) {
    return primitiveTypesProto.CanHandle(type)
        ? primitiveTypesProto.ParseFromString(value, type)
        : unityTypesProto.ParseFromString(value, type);
  }
}

}  // namespace
