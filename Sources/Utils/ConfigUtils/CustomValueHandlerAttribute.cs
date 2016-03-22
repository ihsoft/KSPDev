// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using KSPDev.LogUtils;

namespace KSPDev.ConfigUtils {
  
/// <summary>An annotation to specify a custom value converter for a persitent field.</summary>
/// <remarks>Example usage:
/// <code>[PersistentField("some/path/in/config/my_color")]
/// [CustomValueHandler(typeof(UnityColorValueHandler))]
/// private static Color myColor = Color.white;</code>
/// <para>See help topics and more examples in <seealso cref="ConfigReader"/>.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public class CustomValueHandlerAttribute_OLD : Attribute {
  private readonly ICustomValueHandler handler;
  
  /// <summary>Tells if handler is capable to convert values.</summary>
  /// <remarks>
  /// Conversion methods must not be called if this property returns <c>false</c>.
  /// </remarks>
  public bool isValid {
    get { return handler != null; }
  }
  
  /// <summary>Creates attribute for customer value handler.</summary>
  /// <remarks>Attribute constructor can only accept primitive and const types as parameters. For
  /// this reason constructor gets a type of the handler instead of the actual instance. Such
  /// approach requires runtime checking to ensure that the supplied argument is of acceptable type.
  /// </remarks>
  /// <param name="type">A type of handler. Must implement <seealso cref="ICustomValueHandler"/>
  /// </param>
  /// <exception cref="ArgumentException">If handler type doesn't implement the required interface.
  /// </exception>
  public CustomValueHandlerAttribute_OLD(Type type) {
    var instance = Activator.CreateInstance(type);
    if (!(instance is ICustomValueHandler)) {
      Logger.logError("Handler must be of type {0}", typeof(ICustomValueHandler));
      handler = null;
      return;
    }
    handler = (ICustomValueHandler) instance;
  }

  public string ToString(object value) {
    return handler.ToString(value);
  }

  public object ToValue(string strValue) {
    return handler.ToValue(strValue);
  }
}

}  // namespace
