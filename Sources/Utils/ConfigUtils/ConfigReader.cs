using System;
using System.Reflection;
using System.Linq;
using KSPDev.LogUtils;
using System.Collections.Generic;
using System.ComponentModel;

namespace KSPDev.ConfigUtils {
  
/// <summary>
/// Description of Class1.
/// </summary>
/// TODO: add extensive help
public class ConfigReader {
  private readonly Type objectType;
  private readonly object targetObject;
  //private readonly PersistentFieldsContainer annotations;
  private readonly PersistentField[] persistentFields;
  
  public ConfigReader(object obj, string group = "") {
    objectType = obj.GetType();
    targetObject = obj;
    persistentFields =
        PersistentFieldsFactory.GetPersistentFields(targetObject, group).ToArray();
  }
  
  public ConfigReader(Type objType, string group = "") {
    objectType = objType;
    targetObject = null;
    persistentFields =
        PersistentFieldsFactory.GetPersistentFields(objectType, group).ToArray();
  }

  /// <summary>Reads values of the annotated persitent fields from a CFG file.</summary>
  /// <param name="filePath">A path to the file. For a relative path "current" location is
  /// determined by the game engine.</param>
  public void ReadConfigFromFile(string filePath) {
    Logger.logInfo("Loading settings for {0} from file {1}...", objectType, filePath);
    ConfigNode node = ConfigNode.Load(filePath);
    if (node != null) {
      ReadConfigFromNode(node);
    }
  }
  
  public void ReadConfigFromNode(ConfigNode node) {
    Logger.logInfo("Reading {0} config fields for class {1}",
                   persistentFields.Length, objectType);
    foreach (var persistentField in persistentFields) {
      persistentField.ReadFromConfig(node, targetObject);
    }
  }

  public static bool ReadValueByPath<T>(ConfigNode node, string path, ref T target,
                                        ICustomValueHandler customValueHandler = null) {
    return false;
  }
  
  public static bool ReadValuesByPath<T>(ConfigNode node, string path, ICollection<T> target,
                                         ICustomValueHandler customValueHandler = null) {
    return false;
  }
  
  public static T ParseCfgValue<T>(string value, ICustomValueHandler customValueHandler = null) {
    return customValueHandler != null
        ? (T) customValueHandler.ToValue(value)
        : (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
  }

  public static T[] ParseCfgValues<T>(string value, ICustomValueHandler customValueHandler = null) {
    return new T[0];
  }

  public static string GetValueByPath(ConfigNode node, string path) {
    return GetValueByPath(node, path.Split('/'));
  }
  
  public static string GetValueByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = FindNode(node, pathKeys);
    return valueNode != null ? valueNode.GetValue(pathKeys.Last()) : null;
  }

  public static string[] GetValuesByPath(ConfigNode node, string path) {
    return GetValuesByPath(node, path.Split('/'));
  }
  
  public static string[] GetValuesByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = FindNode(node, pathKeys);
    return valueNode != null ? valueNode.GetValues(pathKeys.Last()) : null;
  }

  public static ConfigNode FindNode(ConfigNode node, string path) {
    return FindNode(node, path.Split('/'));
  }

  public static ConfigNode FindNode(ConfigNode node, string[] pathKeys) {
    if (pathKeys.Length <= 1) {
      return node;
    }
    var nodeKey = pathKeys[0];
    return node.HasNode(nodeKey)
        ? FindNode(node.GetNode(nodeKey), pathKeys.Skip(1).ToArray())
        : null;
  }
}

}  // namespace
