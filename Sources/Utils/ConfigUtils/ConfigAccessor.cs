// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using KSPDev.LogUtils;
using System.Linq;


namespace KSPDev.ConfigUtils {

/// <summary>A service class that simplifies accessing configuration files.</summary>
public static class ConfigAccessor {

  public static string GetValue(ConfigNode node, string[] path) {
    //UNDONE
    Logger.logWarning("GetValue: {0}", string.Join("/", path));
    //return null;
    //return GetValueByPath(node, path);
    var v = GetValueByPath(node, path);
    Logger.logWarning("GetValue: got value = {0}", v);
    return v;
  }

  public static string[] GetValues(ConfigNode node, string[] path) {
    //UNDONE
    Logger.logWarning("GetValues: {0}", string.Join("/", path));
    //return new string[0];
    //return null;
    return GetValuesByPath(node, path);
  }
  
  public static ConfigNode GetNode(ConfigNode node, string[] path) {
    //UNDONE
    Logger.logWarning("GetNode: {0}", string.Join("/", path));
    //return null;
    return GetNodeByPath(node, path);
  }

  public static ConfigNode[] GetNodes(ConfigNode node, string[] path) {
    //UNDONE
    Logger.logWarning("GetNodes: {0}", string.Join("/", path));
    //return new ConfigNode[0];
    //return null;
    return GetNodesByPath(node, path);
  }
  
  public static string GetValueByPath(ConfigNode node, string path) {
    return GetValueByPath(node, path.Split('/'));
  }
  
  public static string GetValueByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray());
    return valueNode != null ? valueNode.GetValue(pathKeys.Last()) : null;
  }

  public static string[] GetValuesByPath(ConfigNode node, string path) {
    return GetValuesByPath(node, path.Split('/'));
  }
  
  public static string[] GetValuesByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray());
    return valueNode != null ? valueNode.GetValues(pathKeys.Last()) : null;
  }

  public static ConfigNode[] GetNodesByPath(ConfigNode node, string path) {
    return GetNodesByPath(node, path.Split('/'));
  }

  public static ConfigNode[] GetNodesByPath(ConfigNode node, string[] pathKeys) {
    var valueNode = GetNodeByPath(node, pathKeys.Take(pathKeys.Length - 1).ToArray());
    return valueNode != null ? valueNode.GetNodes(pathKeys.Last()) : null;
  }

  public static ConfigNode GetNodeByPath(ConfigNode node, string path) {
    return GetNodeByPath(node, path.Split('/'));
  }

  public static ConfigNode GetNodeByPath(ConfigNode node, string[] pathKeys) {
    if (pathKeys.Length == 0) {
      return node;
    }
    var nodeKey = pathKeys[0];
    return node.HasNode(nodeKey)
        ? GetNodeByPath(node.GetNode(nodeKey), pathKeys.Skip(1).ToArray())
        : null;
  }
}
  
}  // namespace
