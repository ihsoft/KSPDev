// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.ComponentModel;

namespace KSPDev.ConfigUtils {

/// <summary>A unified interface to convert values from/into serialized form.</summary>
/// <remarks>Used for handling non-primitive type serialization.
/// See: <seealso cref="CustomValueHandlerAttribute"/> and <seealso cref="ConfigReader"/>.</remarks>
public interface ICustomValueHandler {
  /// <summary>Converts value into a serialized string representation.</summary>
  /// <remarks>Avoid using standard <c>ToString</c> method since its output is not formally defined
  /// and can change from version to version.</remarks>
  /// <param name="value">A value to serialize into string.</param>
  /// <returns>A serilalized string.</returns>
  string ToString(object value);

  /// <summary>Converts a serialized string into an actual type value.</summary>
  /// <param name="strValue">A serialized value string.</param>
  /// <returns>A deserialized value.</returns>
  /// <exception cref="ArgumentException">When string cannot be parsed.</exception>
  object ToValue(string strValue);
}

// FIXME
public abstract class SimpleValueHandler {
  protected readonly PersistentField field;
  
  protected SimpleValueHandler(PersistentField field) {
    this.field = field;
  }
  
  public abstract object CreateFromString(Type type, string strValue);
  public abstract string SaveToString(object obj);
}

// TODO: docs
public class TrivialValueHandler : SimpleValueHandler {
  public TrivialValueHandler(PersistentField field) : base(field) {
  }
  
  /// <summary>Converts a serialized string into an actual type value.</summary>
  /// <param name="strValue">A serialized value string.</param>
  /// <returns>A deserialized value.</returns>
  /// <exception cref="ArgumentException">When string cannot be parsed.</exception>
  public override object CreateFromString(Type type, string strValue) {
    try {
      return TypeDescriptor.GetConverter(type).ConvertFromString(strValue);
    } catch (Exception ex) {
      throw new ArgumentException(ex.Message);
    }
  }
  
  public override string SaveToString(object obj) {
    //FIXME: use type for argument to handle repeated case.
    return null;
  }
}

}  // namespace
