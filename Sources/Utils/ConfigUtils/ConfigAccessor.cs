// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using KSPDev.LogUtils;

namespace KSPDev.ConfigUtils {

/// <summary>A service class that simplifies accessing configuration files.</summary>
/// <remarks>This class allows direct value reading as well as managing  </remarks>
public sealed class ConfigAccessor {
  private static readonly StandardOrdinaryTypesProto standardTypesProto =
      new StandardOrdinaryTypesProto();

  private readonly Type objectType;
  private readonly object targetObject;
  private readonly PersistentField[] persistentFields;

  /// <summary>Creates an accessor for the persitent fields of an instance of a type.</summary>
  /// <remarks>Both static and instance fields will be considered.</remarks>
  /// <param name="instance">An instance to manage fields for.</param>
  /// <param name="group">A group to consider fields for. Set group to <c>null</c> to have all the
  /// fields to be considered.</param>
  public ConfigAccessor(object instance, string group = "") {
    objectType = instance.GetType();
    targetObject = instance;
    persistentFields =
        PersistentFieldsFactory.GetPersistentFields(targetObject, group).ToArray();
  }
  
  /// <summary>Creates an accessor for the persitent fields of a type.</summary>
  /// <remarks>Only static fields will be considered.</remarks>
  /// <param name="type">A type manage fields for.</param>
  /// <param name="group">A group to consider fields for. Set group to <c>null</c> to have all the
  /// fields to be considered.</param>
  public ConfigAccessor(Type type, string group = "") {
    objectType = type;
    targetObject = null;
    persistentFields =
        PersistentFieldsFactory.GetPersistentFields(objectType, group).ToArray();
  }

  /// <summary>Reads values of the annotated persistent fields from a config file.</summary>
  /// <param name="filePath">A path to the file. For a relative path "current" location is
  /// determined by the game engine.</param>
  public void ReadConfigFromFile(string filePath) {
    Logger.logInfo("Loading settings for {0} from file {1}...", objectType, filePath);
    ConfigNode node = ConfigNode.Load(filePath);
    if (node != null) {
      ReadConfigFromNode(node);
    }
  }
  
  /// <summary>Reads values of the annotated persistent fields from a config node.</summary>
  /// <param name="node">A node to read values from.</param>
  public void ReadConfigFromNode(ConfigNode node) {
    Logger.logInfo("Reading {0} persistent fields for class {1}", persistentFields.Length, objectType);
    foreach (var persistentField in persistentFields) {
      persistentField.ReadFromConfig(node, targetObject);
    }
  }

  /// <summary>Writes values of the annotated persistent fields into a config file.</summary>
  /// <param name="filePath">A path to the file. For a relative path "current" location is
  /// determined by the game engine.</param>
  public void WriteConfigToFile(string filePath) {
    var node = new ConfigNode();
    WriteConfigToNode(node);
    Logger.logInfo("Saving settings for {0} into file {1}...", objectType, filePath);
    node.Save(filePath);
  }
  
  /// <summary>Writes values of the annotated persistent fields into a config node.</summary>
  /// <param name="node">A node to write values into.</param>
  public void WriteConfigToNode(ConfigNode node) {
    Logger.logInfo("Writing {0} persistent fields for class {1}", persistentFields.Length, objectType);
    foreach (var persistentField in persistentFields) {
      persistentField.WriteToConfig(node, targetObject);
    }
  }

  /// <summary>Reads a value from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="path">A string path to the value. Path components should be separated by '/'
  /// symbol.</param>
  /// <returns>String value or <c>null</c> if path or value is not present in the
  /// <paramref name="node"/>.</returns>
  public static string GetValueByPath(ConfigNode node, string path) {
    return GetValueByPath(node, path.Split('/'));
  }
  
  /// <summary>Reads a value from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <returns>String value or <c>null</c> if path or value is not present in the
  /// <paramref name="node"/>.</returns>
  public static string GetValueByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray());
    return valueNode != null ? valueNode.GetValue(pathKeys.Last()) : null;
  }

  /// <summary>Reads repeated values from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="path">A string path to the values. Path components should be separated by '/'
  /// symbol.</param>
  /// <returns>Array of string values or <c>null</c> if path is not present in the
  /// <paramref name="node"/>.</returns>
  public static string[] GetValuesByPath(ConfigNode node, string path) {
    return GetValuesByPath(node, path.Split('/'));
  }
  
  /// <summary>Reads repeated values from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <returns>Array of string values or <c>null</c> if path is not present in the
  /// <paramref name="node"/>.</returns>
  public static string[] GetValuesByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray());
    var values = valueNode != null ? valueNode.GetValues(pathKeys.Last()) : null;
    return values != null && values.Length > 0 ? values : null;
  }

  /// <summary>Reads a node from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <returns>Config node or <c>null</c> if path or node is not present in the
  /// <paramref name="node"/>.</returns>
  public static ConfigNode GetNodeByPath(ConfigNode node, string path) {
    return GetNodeByPath(node, path.Split('/'));
  }

  /// <summary>Reads a node from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="createMissingNodes">If <c>true</c> than unknown nodes in the path will be
  /// created.</param>
  /// <returns>Config node or <c>null</c> if path or node is not present in the
  /// <paramref name="node"/>. Returns <paramref name="node"/> if path is empty array.</returns>
  public static ConfigNode GetNodeByPath(ConfigNode node, string[] pathKeys,
                                          bool createMissingNodes = false) {
    if (pathKeys.Length == 0) {
      return node;
    }
    var nodeKey = pathKeys[0];
    if (!node.HasNode(nodeKey) && createMissingNodes) {
      node.AddNode(new ConfigNode(nodeKey));
    }
    return node.HasNode(nodeKey)
        ? GetNodeByPath(node.GetNode(nodeKey), pathKeys.Skip(1).ToArray(),
                        createMissingNodes: createMissingNodes)
        : null;
  }
  
  /// <summary>Reads repeated nodes from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="path">A string path to the nodes. Path components should be separated by '/'
  /// symbol.</param>
  /// <returns>Array of nodes or <c>null</c> if path is not present in the
  /// <paramref name="node"/>.</returns>
  public static ConfigNode[] GetNodesByPath(ConfigNode node, string path) {
    return GetNodesByPath(node, path.Split('/'));
  }

  /// <summary>Reads repeated nodes from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <returns>Array of nodes or <c>null</c> if path is not present in the
  /// <paramref name="node"/>.</returns>
  public static ConfigNode[] GetNodesByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray());
    var nodes = valueNode != null ? valueNode.GetNodes(pathKeys.Last()) : null;
    return nodes != null && nodes.Length > 0 ? nodes : null;
  }

  /// <summary>Sets a value in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A string value to store.</param>
  public static void SetValueByPath(ConfigNode node, string path, string value) {
    SetValueByPath(node, path.Split('/'), value);
  }
  
  /// <summary>Sets a value in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A string value to store.</param>
  public static void SetValueByPath(ConfigNode node, string[] pathKeys, string value) {
    var targetNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray(),
                                   createMissingNodes: true);
    targetNode.SetValue(pathKeys.Last(), value, createIfNotFound: true);
  }
  
  /// <summary>Sets a node in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A config node to store.</param>
  public static void SetNodeByPath(ConfigNode node, string path, ConfigNode value) {
    SetNodeByPath(node, path.Split('/'), value);
  }

  /// <summary>Sets a node in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A config node to store.</param>
  public static void SetNodeByPath(ConfigNode node, string[] pathKeys, ConfigNode value) {
    var targetNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray(),
                                   createMissingNodes: true);
    targetNode.SetNode(pathKeys.Last(), value, createIfNotFound: true);
  }

  /// <summary>Adds a repeated value in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the nodes. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A string value to add into the node.</param>
  public static void AddValueByPath(ConfigNode node, string path, string value) {
    AddValueByPath(node, path.Split('/'), value);
  }
  
  /// <summary>Adds a repeated value in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A string value to add into the node.</param>
  public static void AddValueByPath(ConfigNode node, string[] pathKeys, string value) {
    var targetNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray(),
                                   createMissingNodes: true);
    targetNode.AddValue(pathKeys.Last(), value);
  }

  /// <summary>Adds a repeated node in the config by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the nodes. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A config node to add.</param>
  public static void AddNodeByPath(ConfigNode node, string path, ConfigNode value) {
    AddNodeByPath(node, path.Split('/'), value);
  }
  
  /// <summary>Adds a repeated node in the config by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A config node to add.</param>
  public static void AddNodeByPath(ConfigNode node, string[] pathKeys, ConfigNode value) {
    var targetNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray(),
                                   createMissingNodes: true);
    targetNode.AddNode(pathKeys.Last(), value);
  }

  /// <summary>Stores a value into a config node.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A value to store. The <paramref name="typeProto"/> handler must know how
  /// to convert the value into string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  public static void SetValueByPath<T>(ConfigNode node, string path, T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    SetValueByPath(node, path.Split('/'), value, typeProto);
  }
  
  /// <summary>Stores a value into a config node.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A value to store. The <paramref name="typeProto"/> handler must know how
  /// to convert value's type into string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  public static void SetValueByPath<T>(ConfigNode node, string[] pathKeys, T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    if (typeProto == null) {
      typeProto = standardTypesProto;
    }
    var strValue = typeProto.SerializeToString(value);
    SetValueByPath(node, pathKeys, strValue);
  }

  /// <summary>Reads a value from a config node.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A variable to read value into. The <paramref name="typeProto"/> handler
  /// must know how to convert value's type from string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  /// <returns><c>true</c> if value was successfully read and stored.</returns>
  public static bool GetValueByPath<T>(ConfigNode node, string path, ref T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    return GetValueByPath(node, path.Split('/'), ref value, typeProto);
  }

  /// <summary>Reads a value from a config node.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A variable to read value into. The <paramref name="typeProto"/> handler
  /// must know how to convert value's type from string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  /// <returns><c>true</c> if value was successfully read and stored.</returns>
  public static bool GetValueByPath<T>(ConfigNode node, string[] pathKeys, ref T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    var strValue = GetValueByPath(node, pathKeys);
    if (strValue == null) {
      return false;
    }
    if (typeProto == null) {
      typeProto = standardTypesProto;
    }
    value = (T) typeProto.ParseFromString(strValue, typeof(T));
    return true;
  }
}
  
}  // namespace
