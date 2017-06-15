// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using KSPDev.FSUtils;
using UnityEngine;

namespace KSPDev.ConfigUtils {

/// <summary>Group names that have special meaning.</summary>
/// <seealso cref="ConfigAccessor"/>
/// <seealso cref="PersistentFieldAttribute"/>
public static class StdPersistentGroups {
  /// <summary>A public group that can be saved/loaded on every game scene.</summary>
  /// <remarks>
  /// By the contract any caller can save/load this group at any time. If the class declares
  /// persistent fields with a specific save/load logic then they need to have a group different
  /// from the default.
  /// </remarks>
  public const string Default = "";
}

/// <summary>A service class that simplifies accessing configuration files.</summary>
/// <remarks>This class provides a lot of useful methods to deal with values in game's configuration
/// files. There are low level methods that deal with nodes and values, and there are high level
/// methods that use metadata from the annotated fields.</remarks>
/// <seealso cref="PersistentFieldAttribute"/>
/// <seealso cref="PersistentFieldsFileAttribute"/>
/// <seealso cref="PersistentFieldsDatabaseAttribute"/>
public static class ConfigAccessor {
  static readonly StandardOrdinaryTypesProto standardTypesProto =
      new StandardOrdinaryTypesProto();

  /// <summary>Reads values of the annotated persistent fields from a config file.</summary>
  /// <param name="filePath">
  /// A relative or an absolute path to the file. It's resolved via
  /// <see cref="KspPaths.MakeAbsPathForGameData"/>.
  /// </param>
  /// <param name="type">A type to load fields for.</param>
  /// <param name="instance">
  /// An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be loaded.
  /// </param>
  /// <param name="nodePath">
  /// An optional path in the file. All type's field will be read relative to this part.
  /// </param>
  /// <param name="group">A group tag (see <see cref="BasePersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void ReadFieldsFromFile(string filePath, Type type, object instance,
                                        string nodePath = null,
                                        string group = StdPersistentGroups.Default) {
    Debug.LogFormat("Loading persistent fields: file={0}, group=\"{1}\"",
                    KspPaths.MakeRelativePathToGameData(filePath), group ?? "<ALL>");
    var node = ConfigNode.Load(KspPaths.MakeAbsPathForGameData(filePath));
    if (node != null && nodePath.Length > 0) {
      node = node.GetNode(nodePath);
    }
    if (node != null) {
      ReadFieldsFromNode(node, type, instance, group: group);
    }
  }

  /// <summary>Reads values of the annotated persistent fields from a config file.</summary>
  /// <param name="nodePath">An absolute path in the database. No leading "/".</param>
  /// <param name="type">A type to load fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be loaded.</param>
  /// <param name="group">A group tag (see <see cref="BasePersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void ReadFieldsFromDatabase(string nodePath, Type type, object instance,
                                            string group = StdPersistentGroups.Default) {
    Debug.LogFormat("Loading persistent fileds: db path={0}, group=\"{1}\"",
                    nodePath, group ?? "<ALL>");
    var node = GameDatabase.Instance.GetConfigNode(nodePath);
    if (node != null) {
      ReadFieldsFromNode(node, type, instance, group: group);
    }
  }

  /// <summary>Reads values of the annotated persistent fields from a config node.</summary>
  /// <param name="node">A config node to read data from.</param>
  /// <param name="type">A type to load fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be loaded.</param>
  /// <param name="group">A group tag (see <see cref="BasePersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void ReadFieldsFromNode(ConfigNode node, Type type, object instance,
                                        string group = StdPersistentGroups.Default) {
    var fields = PersistentFieldsFactory.GetPersistentFields(
        type, true /* needStatic */, instance != null /* needInstance */, group).ToArray();
    Debug.LogFormat("Loading {0} persistent fields: group=\"{1}\", node={2}", 
                    fields.Length, group ?? "<ALL>", node.name);
    foreach (var field in fields) {
      field.ReadFromConfig(node, instance);
    }
  }
  
  /// <summary>
  /// Reads persistent fields from the config files specified by the class annotation.
  /// </summary>
  /// <param name="type">A type to load fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be loaded.</param>
  /// <param name="group">A group to load fields for. If <c>null</c> then all groups that are
  /// defined in the class annotation via <see cref="PersistentFieldsFileAttribute"/> will be
  /// loaded.</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <seealso cref="PersistentFieldsFileAttribute"/>
  /// <seealso cref="PersistentFieldsDatabaseAttribute"/>
  public static void ReadFieldsInType(Type type, object instance,
                                      string group = StdPersistentGroups.Default) {
    var attributes = GetPersistentFieldsFiles(type, group);
    Debug.LogFormat("Loading persistent fields: type={0}, group=\"{1}\"", type, group ?? "<ALL>");
    foreach (var attr in attributes) {
      if (attr.configFilePath.Length > 0) {
        ReadFieldsFromFile(
            attr.configFilePath, type, instance, nodePath: attr.nodePath, group: group);
      } else {
        ReadFieldsFromDatabase(attr.nodePath, type, instance, group);
      }
    }
  }

  /// <summary>Writes values of the annotated persistent fields into a file.</summary>
  /// <remarks>
  /// All persitent values are <b>added</b> into the file provided. I.e. if node had already had a
  /// value being persited then it either overwritten (ordinary fields) or extended (collection
  /// fields).
  /// </remarks>
  /// <param name="filePath">
  /// A relative or an absolute path to the file. It's resolved via
  /// <see cref="KspPaths.MakeAbsPathForGameData"/>.
  /// </param>
  /// <param name="rootNodePath">
  /// A path to the node in the file where the data should be written. If the node already exsists
  /// it will be deleted.
  /// </param>
  /// <param name="type">A type to write fields for.</param>
  /// <param name="instance">
  /// An instance of type <paramref name="type"/>. If it's <c>null</c> then only static fields will
  /// be written.
  /// </param>
  /// <param name="mergeMode">
  /// If <c>true</c> and the file already exists then only will be created.
  /// </param>
  /// <param name="group">A group tag (see <see cref="BasePersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void WriteFieldsIntoFile(string filePath,
                                         Type type, object instance,
                                         string rootNodePath = null, bool mergeMode = true,
                                         string group = StdPersistentGroups.Default) {
    Debug.LogFormat("Writing persistent fields: file={0}, group=\"{1}\", isMerging={2}, root={3}",
                    KspPaths.MakeRelativePathToGameData(filePath),
                    group ?? "<ALL>", mergeMode, rootNodePath ?? "/");
    var node = mergeMode
        ? ConfigNode.Load(filePath) ?? new ConfigNode()  // Make empty node if file doesn't exist.
        : new ConfigNode();
    var tagetNode = node;
    if (rootNodePath != null) {
      tagetNode = GetNodeByPath(node, rootNodePath, createIfMissing: true);
      tagetNode.ClearData();  // In case of it's an existing node.
    }
    WriteFieldsIntoNode(tagetNode, type, instance, group: group);
    node.Save(KspPaths.MakeAbsPathForGameData(filePath));
  }
  
  /// <summary>Writes values of the annotated persistent fields into a config node.</summary>
  /// <remarks>All persitent values are <b>added</b> into the node provided. I.e. if node had
  /// already had a value being persited then it either overwritten (ordinary fields) or extended
  /// (collection fields).</remarks>
  /// <param name="node">A config node to write data into.</param>
  /// <param name="type">A type to write fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be written.</param>
  /// <param name="group">A group tag (see <see cref="BasePersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void WriteFieldsIntoNode(ConfigNode node, Type type, object instance,
                                         string group = StdPersistentGroups.Default) {
    var fields = PersistentFieldsFactory.GetPersistentFields(
        type, true /* needStatic */, instance != null /* needInstance */, group).ToArray();
    Debug.LogFormat("Writing {0} persistent fields: group=\"{1}\", node={2}", 
                    fields.Length, group ?? "<ALL>", node.name);
    foreach (var field in fields) {
      field.WriteToConfig(node, instance);
    }
  }
  
  /// <summary>
  /// Writes persistent fields into the config files specified by the class annotation.
  /// </summary>
  /// <remarks>Method updates the config file(s) by preserving top level nodes that are not
  /// specified as targets for the requested group.
  /// <para>Note, that fields cannot be writtent into database. Such annotations will be skipped
  /// during the save.</para>
  /// </remarks>
  /// <param name="type">A type to write fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be written.</param>
  /// <param name="group">A group to write fields for. If <c>null</c> then all groups that are
  /// defined in the class annotation via <see cref="PersistentFieldsFileAttribute"/> will be
  /// written.</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <seealso cref="PersistentFieldsFileAttribute"/>
  public static void WriteFieldsFromType(Type type, object instance,
                                         string group = StdPersistentGroups.Default) {
    var attributes = GetPersistentFieldsFiles(type, group);
    Debug.LogFormat("Writing persistent fields: type={0}, group=\"{1}\"", type, group ?? "<ALL>");
    foreach (var attr in attributes) {
      if (attr.configFilePath.Length > 0) {
        WriteFieldsIntoFile(KspPaths.MakeAbsPathForGameData(attr.configFilePath), type, instance,
                            rootNodePath: attr.nodePath, mergeMode: true, group: attr.group);
      } else {
        Debug.LogFormat("Not saving database group: {0}", attr.nodePath);
      }
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
  /// <param name="createIfMissing">If <c>true</c> than unknown nodes in the path will be
  /// created.</param>
  /// <returns>Config node or <c>null</c> if path or node is not present in the
  /// <paramref name="node"/>.</returns>
  public static ConfigNode GetNodeByPath(ConfigNode node, string path,
                                         bool createIfMissing = false) {
    var pathKeys = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
    return GetNodeByPath(node, pathKeys, createIfMissing: createIfMissing);
  }

  /// <summary>Reads a node from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="createIfMissing">If <c>true</c> than unknown nodes in the path will be
  /// created.</param>
  /// <returns>Config node or <c>null</c> if path or node is not present in the
  /// <paramref name="node"/>. Returns <paramref name="node"/> if path is empty array.</returns>
  public static ConfigNode GetNodeByPath(ConfigNode node, string[] pathKeys,
                                         bool createIfMissing = false) {
    if (pathKeys.Length == 0) {
      return node;
    }
    var nodeKey = pathKeys[0];
    if (!node.HasNode(nodeKey) && createIfMissing) {
      node.AddNode(new ConfigNode(nodeKey));
    }
    return node.HasNode(nodeKey)
        ? GetNodeByPath(node.GetNode(nodeKey), pathKeys.Skip(1).ToArray(),
                        createIfMissing: createIfMissing)
        : null;
  }
  
  /// <summary>Reads repeated nodes from config node by a path.</summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="path">A string path to the nodes. Path components should be separated by '/'
  /// symbol.</param>
  /// <returns>Array of nodes or <c>null</c> if path is not present in the
  /// <paramref name="node"/>.</returns>
  public static ConfigNode[] GetNodesByPath(ConfigNode node, string path) {
    var pathKeys = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
    return GetNodesByPath(node, pathKeys);
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
                                   createIfMissing: true);
    targetNode.SetValue(pathKeys.Last(), value, createIfNotFound: true);
  }
  
  /// <summary>Sets a node in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A config node to store.</param>
  public static void SetNodeByPath(ConfigNode node, string path, ConfigNode value) {
    var pathKeys = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
    SetNodeByPath(node, pathKeys, value);
  }

  /// <summary>Sets a node in config node by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A config node to store.</param>
  public static void SetNodeByPath(ConfigNode node, string[] pathKeys, ConfigNode value) {
    var targetNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray(),
                                   createIfMissing: true);
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
                                   createIfMissing: true);
    targetNode.AddValue(pathKeys.Last(), value);
  }

  /// <summary>Adds a repeated node in the config by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the nodes. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A config node to add.</param>
  public static void AddNodeByPath(ConfigNode node, string path, ConfigNode value) {
    var pathKeys = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
    AddNodeByPath(node, pathKeys, value);
  }
  
  /// <summary>Adds a repeated node in the config by a path.</summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A config node to add.</param>
  public static void AddNodeByPath(ConfigNode node, string[] pathKeys, ConfigNode value) {
    var targetNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray(),
                                   createIfMissing: true);
    targetNode.AddNode(pathKeys.Last(), value);
  }

  /// <summary>
  /// Stores a value of arbitrary type <typeparamref name="T"/> into a config node.
  /// </summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A value to store. The <paramref name="typeProto"/> handler must know how
  /// to convert the value into string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  /// <typeparam name="T">The value type to store. Type proto must be able to handle it.
  /// </typeparam>
  /// <exception cref="ArgumentException">If type cannot be handled by the proto.</exception>
  public static void SetValueByPath<T>(ConfigNode node, string path, T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    SetValueByPath(node, path.Split('/'), value, typeProto);
  }
  
  /// <summary>
  /// Stores a value of arbitrary type <typeparamref name="T"/> into a config node.
  /// </summary>
  /// <param name="node">A node to set data in.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A value to store. The <paramref name="typeProto"/> handler must know how
  /// to convert value's type into string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  /// <typeparam name="T">The value type to store. Type proto must be able to handle it.
  /// </typeparam>
  /// <exception cref="ArgumentException">If type cannot be handled by the proto.</exception>
  public static void SetValueByPath<T>(ConfigNode node, string[] pathKeys, T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    if (typeProto == null) {
      typeProto = standardTypesProto;
    }
    if (!typeProto.CanHandle(typeof(T))) {
      throw new ArgumentException(string.Format(
          "Proto {0} cannot handle type {1}", typeProto.GetType(), typeof(T)));
    }
    var strValue = typeProto.SerializeToString(value);
    SetValueByPath(node, pathKeys, strValue);
  }

  /// <summary>
  /// Reads a value of arbitrary type <typeparamref name="T"/> from a config node.
  /// </summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="path">A string path to the node. Path components should be separated by '/'
  /// symbol.</param>
  /// <param name="value">A variable to read value into. The <paramref name="typeProto"/> handler
  /// must know how to convert value's type from string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  /// <returns><c>true</c> if value was successfully read and stored.</returns>
  /// <typeparam name="T">The value type to read. Type proto must be able to handle it.
  /// </typeparam>
  /// <exception cref="ArgumentException">If type cannot be handled by the proto.</exception>
  public static bool GetValueByPath<T>(ConfigNode node, string path, ref T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    return GetValueByPath(node, path.Split('/'), ref value, typeProto);
  }

  /// <summary>
  /// Reads a value of arbitrary type <typeparamref name="T"/> from a config node.
  /// </summary>
  /// <param name="node">A node to read data from.</param>
  /// <param name="pathKeys">An array of values that makes the full path. First node in the array is
  /// the top most component of the path.</param>
  /// <param name="value">A variable to read value into. The <paramref name="typeProto"/> handler
  /// must know how to convert value's type from string.</param>
  /// <param name="typeProto">A proto capable to handle the type of <paramref name="value"/>. If not
  /// set then <see cref="StandardOrdinaryTypesProto"/> is used.</param>
  /// <returns><c>true</c> if value was successfully read and stored.</returns>
  /// <typeparam name="T">The value type to read. Type proto must be able to handle it.
  /// </typeparam>
  /// <exception cref="ArgumentException">If type cannot be handled by the proto.</exception>
  public static bool GetValueByPath<T>(ConfigNode node, string[] pathKeys, ref T value,
                                       AbstractOrdinaryValueTypeProto typeProto = null) {
    if (typeProto == null) {
      typeProto = standardTypesProto;
    }
    if (!typeProto.CanHandle(typeof(T))) {
      throw new ArgumentException(string.Format(
          "Proto {0} cannot handle type {1}", typeProto.GetType(), typeof(T)));
    }
    var strValue = GetValueByPath(node, pathKeys);
    if (strValue == null) {
      return false;
    }
    value = (T) typeProto.ParseFromString(strValue, typeof(T));
    return true;
  }

  /// <summary>Gathers and returns persistent field fields annotations.</summary>
  /// <param name="type">A type to lookup for the field annotations.</param>
  /// <param name="group">A group tag (see <see cref="PersistentFieldsFileAttribute"/>). If
  /// <c>null</c> then all files defined in the type are returned.</param>
  /// <returns>Array of persistent fields.</returns>
  static AbstractPersistentFieldsFileAttribute[] GetPersistentFieldsFiles(Type type, string group) {
    // Sort by config path to ensure the most top level nodes are handled before the children.
    var attributes = type.GetCustomAttributes(
        typeof(AbstractPersistentFieldsFileAttribute), true /* inherit */);
    return (attributes as AbstractPersistentFieldsFileAttribute[])
        .Where(x => group == null || x.group.ToLowerInvariant().Equals(group.ToLowerInvariant()))
        .OrderBy(x => x.nodePath)
        .ToArray();
  }
}
  
}  // namespace
