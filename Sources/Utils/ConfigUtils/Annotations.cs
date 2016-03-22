// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using KSPDev.LogUtils;

namespace KSPDev.ConfigUtils {

/// <summary>Base class for handler attributes.</summary>
public class AbstractHandlerAttribute : Attribute {
  protected Type handlerType;

  protected AbstractHandlerAttribute(Type handlerType) {
    this.handlerType = handlerType;
  }
  
  /// <summary>Verifies if handler is of allowed type.</summary>
  /// <param name="allowSubclasses">Specifies if sublcasses of the allowed type are also allowed.
  /// </param>
  /// <param name="allowedTypes">List of allowed types.</param>
  protected void verifyHandlerType(bool allowSubclasses, params Type[] allowedTypes) {
    foreach (var allowedType in allowedTypes) {
      if (handlerType == allowedType || allowSubclasses && handlerType.IsSubclassOf(allowedType)) {
        return;
      }
    }
    Logger.logError("Cannot use handler {0} in custom attribute {1}", handlerType, GetType());
    handlerType = null;
  }
}

/// <summary>Base attribute to specify a handler of an ordinary value.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class CustomValueAttribute : AbstractHandlerAttribute {
  public CustomValueAttribute(Type handlerType) : base(handlerType) {
    verifyHandlerType(true /* allowSubclasses */,
                      typeof(SimpleValueHandler), typeof(CompoundValueHandler));
  }
}

/// <summary>Attribute to handle a class or struct that serialize as a config node.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class CompoundValueAttribute : CustomValueAttribute {
  public CompoundValueAttribute() : base(typeof(CompoundValueHandler)) {
  }
}

/// <summary>Attribute to handle types that serialize as a simple string.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class SimpleValueAttribute : CustomValueAttribute {
  public SimpleValueAttribute() : base(typeof(SimpleValueHandler)) {
  }
}

/// <summary>Base attribute to specify a handler of a repeated field.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class CustomRepeatableFieldAttribute : AbstractHandlerAttribute {
  public CustomRepeatableFieldAttribute(Type handlerType) : base(handlerType) {
    verifyHandlerType(true /* allowSubclasses */, typeof(RepeatableFieldHandler));
  }
}

/// <summary>An attribute to handle generic collections.</summary>
/// <remarks>Type must have exactly one generic argument. See more limitations in
/// <seealso cref="GenericContainerHandler"/>.</remarks>
[AttributeUsage(AttributeTargets.Field)]
public class GenericRepeatableFieldAttribute : CustomRepeatableFieldAttribute {
  public GenericRepeatableFieldAttribute() : base(typeof(GenericContainerHandler)) {
  }
}

}  // namespace
