// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using KSPDev.LogUtils;

namespace KSPDev.ConfigUtils {

/// <summary>A unified interface to convert compound values from/into serialized form.</summary>
/// <remarks>Used for handling classes and structs serialization.</remarks>
public class CompoundValueHandler {
  private readonly PersistentField persistentField;
  
  public CompoundValueHandler(PersistentField field) {
    this.persistentField = field;
  }
  
  /// <summary>Converts a serialized confg node into an actual type value.</summary>
  /// <param name="compoundNode">A node that describes the value.</param>
  /// <returns>A deserialized value.</returns>
  public virtual object CreateFromConfigNode(ConfigNode compoundNode, Type type) {
    // When compound field is a part of repeated field don't use field's path.
    if (compoundNode == null) {
      return null;
    }
    var instance = Activator.CreateInstance(type);
    foreach (var compoundTypeField in persistentField.compoundTypeFields) {
      compoundTypeField.ReadFromConfig(compoundNode, instance);
    }
    //UNDONE
    Logger.logWarning("Constructed compound value: {0}", instance);
    return instance;
  }

  /// <summary>Saves value into a config node.</summary>
  public virtual void WriteToConfigNode(ConfigNode node, string[] path, object obj) {
    //TODO: implement
  }
}

}  // namespace
