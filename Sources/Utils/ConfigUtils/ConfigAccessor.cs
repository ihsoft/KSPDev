// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using KSPDev.LogUtils;
using KSPDev.FSUtils;

namespace KSPDev.ConfigUtils {

/// <summary>Group names that have special meaning.</summary>
/// <seealso cref="ConfigAccessor"/>
/// <seealso cref="PersistentFieldAttribute"/>
public static class StdPersistentGroups {
  /// <summary>A public group that can be saved/loaded on every game scene.</summary>
  /// <remarks>By the contract any caller can save/load this group at any time. If class declares
  /// persistent fields with specific save/load logic then they need to have a group different from
  /// the default.</remarks>
  public const string Default = "";
}

/// <summary>A service class that simplifies accessing configuration files.</summary>
/// <remarks>This class allows direct value reading as well as managing  </remarks>
/// <seealso cref="PersistentFieldAttribute"/>
public static class ConfigAccessor {
  private static readonly StandardOrdinaryTypesProto standardTypesProto =
      new StandardOrdinaryTypesProto();

  /// <summary>Reads values of the annotated persistent fields from a config file.</summary>
  /// <param name="filePath">A relative or an absolute path to the file. It's resolved via
  /// <see cref="KspPaths.makePluginPath"/>.</param>
  /// <param name="type">A type to load fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be loaded.</param>
  /// <param name="group">A group tag (see <see cref="AbstractPersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void ReadFieldsFromFile(string filePath, Type type = null, object instance = null,
                                        string group = StdPersistentGroups.Default) {
    Logger.logInfo("Loading persistent fields: file={0}, group=\"{1}\"",
                   filePath, group ?? "<ALL>");
    var node = ConfigNode.Load(KspPaths.makePluginPath(filePath));
    if (node != null) {
      ReadFieldsFromNode(node, type: type, instance: instance, group: group);
    }
  }

  /// <summary>Reads values of the annotated persistent fields from a config node.</summary>
  /// <param name="node">A config node to read data from.</param>
  /// <param name="type">A type to load fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be loaded.</param>
  /// <param name="group">A group tag (see <see cref="AbstractPersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void ReadFieldsFromNode(ConfigNode node, Type type = null, object instance = null,
                                        string group = StdPersistentGroups.Default) {
    var fields = GetPersistentFields(ref type, instance, group);
    Logger.logInfo("Loading {0} persistent fields: group=\"{1}\", node={2}", 
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
  public static void ReadFieldsInType(Type type = null, object instance = null,
                                      string group = StdPersistentGroups.Default) {
    var attributes = GetPersistentFieldsFiles(ref type, instance, group);
    Logger.logInfo("Loading persistent fields: type={0}, group=\"{1}\"",
                   type, group ?? "<ALL>");
    foreach (var attr in attributes) {
      var filePath = KspPaths.makePluginPath(attr.configFilePath);
      Logger.logInfo("Loading persistent fields: file={0}, group=\"{1}\"",
                     filePath, group ?? "<ALL>");
      var node = ConfigNode.Load(filePath);
      if (node != null) {
        ReadFieldsFromNode(GetNodeByPath(node, attr.nodePath),
                           type: type, instance: instance, group: attr.group);
      }
    }
  }

  /// <summary>Writes values of the annotated persistent fields into a file.</summary>
  /// <remarks>All persitent values are <b>added</b> into the file provided. I.e. if node had
  /// already had a value being persited then it either overwritten (ordinary fields) or extended
  /// (collection fields).</remarks>
  /// <param name="filePath">A relative or an absolute path to the file. It's resolved via
  /// <see cref="KspPaths.makePluginPath"/>.</param>
  /// <param name="rootNodePathKeys">A path to the node in the file where the daata should be
  /// written. If the node already exsist it will be deleted.</param>
  /// <param name="mergeMode">If <c>true</c> and the file already exists then only
  /// <paramref name="rootNodePathKeys"/> node will be updated in that file. Otherwise, a new file
  /// will be created.</param>
  /// <param name="type">A type to write fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be written.</param>
  /// <param name="group">A group tag (see <see cref="AbstractPersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void WriteFieldsIntoFile(string filePath,
                                         string[] rootNodePathKeys = null, bool mergeMode = true,
                                         Type type = null, object instance = null,
                                         string group = StdPersistentGroups.Default) {
    Logger.logInfo("Writing persistent fields: file={0}, group=\"{1}\", isMerging={2}, root={3}",
                   filePath, group ?? "<ALL>", mergeMode,
                   string.Join("/", rootNodePathKeys ?? new[] {"/"}));
    var node = mergeMode
        ? ConfigNode.Load(filePath) ?? new ConfigNode()  // Make empty node if file doesn't exist.
        : new ConfigNode();
    var tagetNode = node;
    if (rootNodePathKeys != null) {
      tagetNode = GetNodeByPath(node, rootNodePathKeys, createIfMissing: true);
      tagetNode.ClearData();  // In case of it's an existing node.
    }
    WriteFieldsIntoNode(tagetNode, type: type, instance: instance, group: group);
    node.Save(KspPaths.makePluginPath(filePath));
  }
  
  /// <summary>Writes values of the annotated persistent fields into a config node.</summary>
  /// <remarks>All persitent values are <b>added</b> into the node provided. I.e. if node had
  /// already had a value being persited then it either overwritten (ordinary fields) or extended
  /// (collection fields).</remarks>
  /// <param name="node">A config node to write data into.</param>
  /// <param name="type">A type to write fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be written.</param>
  /// <param name="group">A group tag (see <see cref="AbstractPersistentFieldAttribute"/>).</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  public static void WriteFieldsIntoNode(ConfigNode node, Type type = null, object instance = null,
                                         string group = StdPersistentGroups.Default) {
    var fields = GetPersistentFields(ref type, instance, group);
    Logger.logInfo("Writing {0} persistent fields: group=\"{1}\", node={2}", 
                   fields.Length, group ?? "<ALL>", node.name);
    foreach (var field in fields) {
      field.WriteToConfig(node, instance);
    }
  }
  
  /// <summary>
  /// Writes persistent fields into the config files specified by the class annotation.
  /// </summary>
  /// <remarks>Method updates the config file(s) by preserving top level nodes that are not
  /// specified as targets for the requested group.</remarks>
  /// <param name="type">A type to write fields for.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be written.</param>
  /// <param name="group">A group to write fields for. If <c>null</c> then all groups that are
  /// defined in the class annotation via <see cref="PersistentFieldsFileAttribute"/> will be
  /// written.</param>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <seealso cref="PersistentFieldsFileAttribute"/>
  public static void WriteFieldsFromType(Type type = null, object instance = null,
                                         string group = StdPersistentGroups.Default) {
    var attributes = GetPersistentFieldsFiles(ref type, instance, group);
    Logger.logInfo("Writing persistent fields: type={0}, group=\"{1}\"",
                   type, group ?? "<ALL>");
    foreach (var attr in attributes) {
      WriteFieldsIntoFile(KspPaths.makePluginPath(attr.configFilePath),
                          rootNodePathKeys: attr.nodePath, mergeMode: true,
                          type: type, instance: instance, group: attr.group);
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

  /// <summary>Gathers and returns persistent field annotations.</summary>
  /// <param name="type">[OUT] A type to lookup for the field annotations. Can be <c>null</c> in
  /// which case the type is resolved from <paramref name="instance"/>.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. If it's <c>null</c> then
  /// only static fields will be loaded.</param>
  /// <param name="group">A group tag (see <see cref="AbstractPersistentFieldAttribute"/>). If
  /// <c>null</c> then annotated fields in any group are returned.</param>
  /// <returns>Array of persistent fields.</returns>
  private static PersistentField[] GetPersistentFields(
      ref Type type, object instance, string group) {
    if (type == null && instance == null) {
      throw new ArgumentException("Either type or instance must not be null");
    }
    if (type == null) {
      type = instance.GetType();
    }
    return PersistentFieldsFactory.GetPersistentFields(
        type, true /* needStatic */, instance != null /* needInstance */, group).ToArray();
  }
  
  /// <summary>Gathers and returns persistent field fields annotations.</summary>
  /// <param name="type">[OUT] A type to lookup for the field annotations. Can be <c>null</c> in
  /// which case the type is resolved from <paramref name="instance"/>.</param>
  /// <param name="instance">An instance of type <paramref name="type"/>. Only used to resolve
  /// <paramref name="type"/> when it's <c>null</c>.</param>
  /// <param name="group">A group tag (see <see cref="PersistentFieldsFileAttribute"/>). If
  /// <c>null</c> then all files defined in the type are returned.</param>
  /// <returns>Array of persistent fields.</returns>
  private static PersistentFieldsFileAttribute[] GetPersistentFieldsFiles(
      ref Type type, object instance, string group) {
    if (type == null && instance == null) {
      throw new ArgumentException("Either type or instance must not be null");
    }
    if (type == null) {
      type = instance.GetType();
    }
    // Sort by config path to ensure the most top level nodes are handled before the children.
    return (type.GetCustomAttributes(typeof(PersistentFieldsFileAttribute), true /* inherit */)
        as PersistentFieldsFileAttribute[])
        .Where(x => group == null || x.group.ToLowerInvariant().Equals(group.ToLowerInvariant()))
        .OrderBy(x => string.Join("/", x.nodePath))
        .ToArray();
  }
}
  
}  // namespace
