// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {
  
/// <summary>An annotation for fields that needs (de)serialization.</summary>
/// <remarks>See help topics and examples in <seealso cref="ConfigReader"/>.</remarks>
[AttributeUsage(AttributeTargets.Field)]
public class PersistentFieldAttribute : Attribute {
  public readonly string[] path;
  public string group = "";
  public Type valueHandler;
  public Type repetableHandler;
  
  public bool isRepeatable {
    set {
      repetableHandler = value ? typeof(GenericContainerHandler) : null;
    }
    get {
      return repetableHandler != null;
    }
  }
  
  public bool isCompound {
    set {
      valueHandler = value ? typeof(CompoundValueHandler) : typeof(TrivialValueHandler);      
    }
    get {
      // FIXME: Check for subclasses
      return valueHandler == typeof(CompoundValueHandler)
          || valueHandler.IsSubclassOf(typeof(CompoundValueHandler));
    }
  }
  
  /// <summary>
  /// 
  /// </summary>
  /// <param name="cfgPath">A relative path to the the value in the config file. The config root
  /// depends on the annotation context.</param>
  /// <param name="group">A name of the group to consider annotaned fields for.</param>
  /// <param name="repeatable">If <c>true</c> than field's value is treated as a generic container
  /// which single argument is the item's type. The container must have exactly one template
  /// parameter.</param>
  /// <param name="compound">TBD</param>
  public PersistentFieldAttribute(string cfgPath) {
    this.path = cfgPath.Split('/');
    isRepeatable = false;
    isCompound = false;
  }
}
  
}  // namespace
