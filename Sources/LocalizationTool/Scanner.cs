// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.Localization;
using KSPDev.FSUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KSPDev.LocalizationTool {

[KSPAddon(KSPAddon.Startup.MainMenu, true /*once*/)]
class Scanner : MonoBehaviour {
  /// <summary>
  /// List of the part's fields that need localization.
  /// </summary>
  string[] localizablePartFields = {"title", "manufacturer", "description", "tags"};

  /// <summary>A container for an item that needs localization.</summary>
  struct LocItem {
    /// <summary>The physical file path contains the entity declaration.</summary>
    public string fullFilePath;

    /// <summary>A string to use to group the similar items together.</summary>
    /// <remarks>It can be anything, but usually it's something meaningful.</remarks>
    public string groupKey;

    /// <summary>A string to use to sort the items within the group.</summary>
    /// <remarks>
    /// The items are always sorted by this value first. Then, an extra sorting can be applied
    /// depening on the writing preferences.
    /// </remarks>
    public string sortKey;

    /// <summary>The tag to use for resolution of the localized content.</summary>
    public string locTag;

    /// <summary>A value to use when the localization is not available.</summary>
    /// <remarks>
    /// In some cases it can be the localized value for the current game language. E.g. the KSP
    /// annotated fields only give <c>guiName</c> in a localized form, i.e. if the value has been
    /// set to a tag, and the tag is known to the game, then the localized value will be returned
    /// instead of the tag (which was the original value).
    /// </remarks>
    public string locDefaultValue;

    /// <summary>Optional description of the item.</summary>
    /// <remarks>
    /// It can be different in every particular case. The general rule is that this field should
    /// give a context which is absent otherwsie (e.g. via the tag name or via the default value). 
    /// </remarks>
    public string locDescription;

    /// <summary>Optional example usage of the template.</summary>
    /// <remarks>
    /// The example must be provided by the localization class. E.g.
    /// <see cref="KSPDev.GUIUtils.LocalizableMessage"/>.
    /// </remarks>
    public string locExample;
  }

  //FIXME
  void Awake() {
    var locItems = EmitAllItemsForPrefix("KAS-1.0/");
    WriteLocItems(locItems,
                  Localizer.CurrentLanguage,
                  KspPaths.GetModsDataFilePath(this, "agg-localization.cfg"));
  }

  /// <summary>
  /// Finds all the entities that macthes the prefix, and extracts the items for them.
  /// </summary>
  /// <param name="prefix">
  /// The file path prefix. It's relative to the game's <c>GameData</c> directory.
  /// </param>
  /// <returns>The all items for the prefix.</returns>
  List<LocItem> EmitAllItemsForPrefix(string prefix) {
    var res = new List<LocItem>();

    // Extract strings for the parts.    
    var parts = PartLoader.LoadedPartsList
        .Where(x => x.partUrl.StartsWith(prefix, StringComparison.CurrentCulture));
    foreach (var part in parts) {
      res.AddRange(EmitItemForPart(part));
    }

    // Extract strings from the assembliy(ies). 
    var types = AssemblyLoader.loadedAssemblies
        .Where(x => x.url.StartsWith(prefix, StringComparison.CurrentCulture))
        .SelectMany(x => x.types)
        .SelectMany(x => x.Value);
    foreach (var type in types) {
      res.AddRange(EmitItemsForType(type));
    }

    return res;
  }

  /// <summary>Extracts strings from the specified type.</summary>
  /// <remarks>
  /// The method can extract strings from the members annotated by the KSP attributes:
  /// <see cref="KSPField"/>, <see cref="KSPEvent"/>, and <see cref="KSPAction"/>. It also supports
  /// <c>KSPDev</c> localization class <see cref="KSPDev.GUIUtils.LocalizableMessage"/>.
  /// </remarks>
  /// <param name="type"></param>
  /// <returns></returns>
  List<LocItem> EmitItemsForType(Type type) {
    var res = new List<LocItem>();
    var members = type.GetMembers(
        BindingFlags.Public | BindingFlags.NonPublic
        | BindingFlags.Instance | BindingFlags.Static
        | BindingFlags.DeclaredOnly);
    foreach (var member in members) {
      var memberItems = new List<LocItem>();
      if (memberItems.Count == 0) {
        memberItems = EmitItemsForLocalizableMessage(member);
      }
      if (memberItems.Count == 0) {
        memberItems = EmitItemsForKSPField(member);
      }
      if (memberItems.Count == 0) {
        memberItems = EmitItemsForKSPEvent(member);
      }
      if (memberItems.Count == 0) {
        memberItems = EmitItemsForKSPAction(member);
      }
      res.AddRange(memberItems);
    }
    return res;
  }

  /// <summary>Extracts localization items from the <c>[KSPField]</c> annotated fields.</summary>
  /// <param name="info">The type member to extract the strings for.</param>
  /// <returns>All the localization items for the member.</returns>
  List<LocItem> EmitItemsForKSPField(MemberInfo info) {
    var res = new List<LocItem>();
    var attrObj = info.GetCustomAttributes(typeof(KSPField), false).FirstOrDefault() as KSPField;
    if (attrObj != null && !string.IsNullOrEmpty(attrObj.guiName)) {
      res.Add(new LocItem() {
          groupKey = "Type: " + info.DeclaringType.FullName,
          sortKey = "000",
          fullFilePath = info.DeclaringType.Assembly.Location,
          locTag = MakeTypeMemberLocalizationTag(info, "_Field_"),
          locDefaultValue = attrObj.guiName,
      });
      if (!string.IsNullOrEmpty(attrObj.guiUnits)) {
        res.Add(new LocItem() {
            groupKey = "Type: " + info.DeclaringType.FullName,
            sortKey = "000",
            fullFilePath = info.DeclaringType.Assembly.Location,
            locTag = MakeTypeMemberLocalizationTag(info, "_FieldUnits_"),
            locDefaultValue = attrObj.guiUnits,
        });
      }
    }
    return res;
  }

  /// <summary>Extracts localization items from the <c>[KSPEvent]</c> annotated fields.</summary>
  /// <param name="info">The type member to extract the strings for.</param>
  /// <returns>All the localization items for the member.</returns>
  List<LocItem> EmitItemsForKSPEvent(MemberInfo info) {
    var res = new List<LocItem>();
    var attrObj = info.GetCustomAttributes(typeof(KSPEvent), false).FirstOrDefault() as KSPEvent;
    if (attrObj != null && !string.IsNullOrEmpty(attrObj.guiName)) {
      res.Add(new LocItem() {
          groupKey = "Type: " + info.DeclaringType.FullName,
          sortKey = "001",
          fullFilePath = info.DeclaringType.Assembly.Location,
          locTag = MakeTypeMemberLocalizationTag(info, "_Event_"),
          locDefaultValue = attrObj.guiName,
      });
    }
    return res;
  }

  /// <summary>Extracts localization items from the <c>[KSPAction]</c> annotated fields.</summary>
  /// <param name="info">The type member to extract the strings for.</param>
  /// <returns>All the localization items for the member.</returns>
  List<LocItem> EmitItemsForKSPAction(MemberInfo info) {
    var res = new List<LocItem>();
    var attrObj = info.GetCustomAttributes(typeof(KSPAction), false).FirstOrDefault() as KSPAction;
    if (attrObj != null && !string.IsNullOrEmpty(attrObj.guiName)) {
      res.Add(new LocItem() {
          groupKey = "Type: " + info.DeclaringType.FullName,
          sortKey = "002",
          fullFilePath = info.DeclaringType.Assembly.Location,
          locTag = MakeTypeMemberLocalizationTag(info, "_Action_"),
          locDefaultValue = attrObj.guiName,
      });
    }
    return res;
  }

  /// <summary>
  /// Extracts localization info from a <see cref="KSPDev.GUIUtils.LocalizableMessage"/> classes.
  /// </summary>
  /// <param name="info">The type member to extract the strings for.</param>
  /// <returns>All the localization items for the member.</returns>
  List<LocItem> EmitItemsForLocalizableMessage(MemberInfo info) {
    var res = new List<LocItem>();
    var field = info as FieldInfo;
    if (field == null) {
      return res;
    }
    // Different modules may use own versions of the KSPDev Utils library. So access the data
    // through the reflections rather than casting to a type (which would be very convinient).
    var fieldType = field.FieldType;
    while (fieldType != null
           && (fieldType.Namespace != "KSPDev.GUIUtils"
               || !fieldType.Name.StartsWith("LocalizableMessage", StringComparison.Ordinal))) {
      fieldType = fieldType.BaseType;
    }
    if (fieldType == null) {
      return res; // It's not a LocalizableMessage instance. 
    }
    if ((field.Attributes | FieldAttributes.Static) == 0) {
      Debug.LogWarningFormat("Skipping a non-static message field: {0}.{1}",
                             field.DeclaringType.Namespace, field.Name);
      return res;
    }
    var value = field.GetValue(null);
    if (value == null) {
      Debug.LogErrorFormat("The message field is NULL: {0}.{1}",
                           field.DeclaringType.Namespace, field.Name);
      return res;
    }
    var msgTag = GetFieldOrPropertyString(value, "tag");
    var defaultTemplate = GetFieldOrPropertyString(value, "defaultTemplate");
    var description = GetFieldOrPropertyString(value, "description");
    var locExample = GetFieldOrPropertyString(value, "example");
    if (string.IsNullOrEmpty(msgTag) || string.IsNullOrEmpty(defaultTemplate)) {
      Debug.LogErrorFormat("Failed to read a message from {0} in {1}.{2}",
                           field.FieldType.FullName,
                           field.DeclaringType.FullName, field.Name);
      return res;
    }
    if (msgTag[0] != '#') {
      msgTag = MakeTypeMemberLocalizationTag(info);
      Debug.LogFormat("Auto generate a tag {0}", msgTag);
    }
    res.Add(new LocItem() {
        groupKey = "Type: " + info.DeclaringType.FullName,
        sortKey = "003",
        fullFilePath = field.FieldType.Assembly.Location,
        locTag = msgTag,
        locDefaultValue = defaultTemplate,
        locDescription = description,
        locExample = locExample,
    });
    return res;
  }

  /// <summary>Gets a string value from a filed of paroperty via reflection.</summary>
  /// <param name="obj">The object to read value from.</param>
  /// <param name="memberName">The field or property name to read.</param>
  /// <returns>A string value or <c>null</c> if the value cannot be obtained.</returns>
  string GetFieldOrPropertyString(object obj, string memberName) {
    var fieldInfo = obj.GetType().GetField(memberName);
    if (fieldInfo != null) {
      return (fieldInfo != null ? fieldInfo.GetValue(obj) : null) as string;
    }
    var propertyInfo = obj.GetType().GetProperty(memberName);
    return (propertyInfo != null ? propertyInfo.GetValue(obj, null) : null) as string;
  }

  /// <summary>Makes a tag for the type member.</summary>
  /// <param name="info">The member to make a tag for.</param>
  /// <param name="fieldTypeStr">A string to distinguish the dafferent field types.</param>
  /// <returns>A complete and correct localization tag.</returns>
  string MakeTypeMemberLocalizationTag(MemberInfo info, string fieldTypeStr = "_") {
    return "#" + info.DeclaringType.FullName.Replace(".", "_") + fieldTypeStr + info.Name;
  }

  /// <summary>Extracts the localization items from the part's config.</summary>
  /// <param name="part">The part to extract items for.</param>
  /// <returns>All the localization items for the part.</returns>
  List<LocItem> EmitItemForPart(AvailablePart part) {
    var res = new List<LocItem>();
    // The part's config in the AvailablePart doesn't have all the fields and lacks the comments.
    // We do need the comments to resolve the field tag names, so load via a custom method.
    // In case of the custom loading method fails, use the stock one without the comments support.
    var config = (LoadConfigWithComments(part.configFileFullName)
                  ?? ConfigNode.Load(part.configFileFullName)).GetNode("PART");
    if (config == null) {
      Debug.LogErrorFormat("Failed to load part's config: partName={0}, configUrl={1}",
                           part.name, part.configFileFullName);
      return res;
    }
    foreach (var fieldName in localizablePartFields) {
      var field = config.values.Cast<ConfigNode.Value>()
          .FirstOrDefault(x => x.name == fieldName);
      if (field == null) {
        Debug.LogWarningFormat("Field '{0}' is not found in the part {1} config",
                               fieldName, part.name);
        continue;
      }
      string locTag = null;
      string locDefaultValue = null;
      if (!string.IsNullOrEmpty(field.comment)
          && field.comment.StartsWith("#", StringComparison.Ordinal)) {
        var match = Regex.Match(field.comment, @"^(#[a-zA-Z0-9_-]+)\s*=\s*(.+?)$");
        if (match.Success) {
          locTag = match.Groups[1].Value;
          if (field.value == locTag) {
            // Part's tag localization failed, use the default text.
            locDefaultValue = match.Groups[2].Value; 
          }
        } else {
          Debug.LogWarningFormat(
              "Cannot resolve defult localization tag in field {0} for part {1}: {2}",
              fieldName, config.GetValue("name"), field.comment);
        }
      }
      var item = new LocItem() {
          groupKey = "Part: " + part.name,
          fullFilePath = part.configFileFullName,
          locTag = locTag ?? MakePartFieldLocalizationTag(config.GetValue("name"), fieldName),
          locDefaultValue = locDefaultValue ?? field.value,
      };
      res.Add(item);
    }
    return res;
  }

  /// <summary>Makes a tag for the part field.</summary>
  /// <param name="partName">The name of the part to make a tag for.</param>
  /// <param name="fieldName">The field name in the config.</param>
  /// <returns>A complete and correct localization tag.</returns>
  string MakePartFieldLocalizationTag(string partName, string fieldName) {
    return "#" + partName.Replace(".", "_") + "_Part_" + fieldName;
  }

  /// <summary>Writes the localization items into file.</summary>
  /// <remarks>
  /// <list type="bullet">
  /// <item>All items are grouped by <see cref="LocItem.groupKey"/>.</item>
  /// <item>The groups are sorted in the ascending order.</item>
  /// <item>The items within a group are sorted in the ascending order.</item>
  /// <item>
  /// If item has <see cref="LocItem.locDescription"/>, then its written as a comment that precedes
  /// the tag-to-value line.
  /// </item>
  /// </list>
  /// </remarks>
  /// <param name="items">The items to write.</param>
  /// <param name="lang">
  /// The language of the file. It will be used for the language block in the file.
  /// </param>
  /// <param name="filePath">The file path to write the data into.</param>
  void WriteLocItems(List<LocItem> items, string lang, string filePath) {
    using (var file = new StreamWriter(filePath)) {
      file.WriteLine("// Auto generated by KSPDev Locaziation tool at: " + DateTime.Now);
      file.Write("Localization\n{\n\t" + lang + "\n\t{\n");
      var grouped = items
          .OrderBy(x => x.groupKey)
          .ThenBy(x => x.sortKey)
          .ThenBy(x => x.locTag)
          .GroupBy(x => x.groupKey);
      foreach (var groupItems in grouped) {
        file.WriteLine(MakeMultilineComment(2, groupItems.Key));
        foreach (var item in groupItems) {
          if (!string.IsNullOrEmpty(item.locDescription)) {
            file.WriteLine(MakeMultilineComment(2, item.locDescription));
          }
          if (!string.IsNullOrEmpty(item.locExample)) {
            file.WriteLine(MakeMultilineComment(2, "Example usage:"));
            file.WriteLine(MakeMultilineComment(2, item.locExample));
          }
          file.WriteLine(MakeConfigNodeLine(2, item.locTag, item.locDefaultValue ?? item.locTag));
        }
        file.WriteLine("");
      }
      file.Write("\t}\n}\n");
    }
  }

  /// <summary>Formats a comment with the proper indentation.</summary>
  /// <param name="indentation">The indentation in tabs. Each tab is 8 spaces.</param>
  /// <param name="comment">
  /// The comment to format. It can contain multiple lines separated by a "\n" symbols.
  /// </param>
  /// <returns>A properly formatted comment block.</returns>
  string MakeMultilineComment(int indentation, string comment) {
    return string.Join(
        "\n",
        comment.Split('\n').Select(x => new string('\t', indentation) + "// " + x).ToArray());
  }

  /// <summary>Formats a config node key/value line.</summary>
  /// <param name="indentation">The indentation in tabs. Each tab is 8 spaces.</param>
  /// <param name="key">The key string.</param>
  /// <param name="value">
  /// The value string. It can contain multiple lines separated by a "\n" symbols.
  /// </param>
  /// <returns>A properly formatted line.</returns>
  string MakeConfigNodeLine(int indentation, string key, string value) {
    return new string('\t', indentation) + key + " = " + value.Replace("\n", "\\n");
  }

  /// <summary>Loads a config file preserving the value comments.</summary>
  /// <remarks>
  /// <para>
  /// Unfortunately, the stock class can save the comments, but not loading them. This method loads
  /// a simple config file while keeping the comments. Note, that only a simple well formated config
  /// can be handled.
  /// </para>
  /// <para>
  /// The method keeps all the comments in the file including the empty lines which are treated as
  /// empty comments. When a comment is placed on the same line as a node or a value declaration,
  /// then it's added to the node or value. If the comment is placed on its own line, then it's
  /// added as a value of a special field named <c>__commentField</c>.   
  /// </para>
  /// </remarks>
  /// <param name="fileFullName">The file to load.</param>
  /// <returns>A loaded config node.</returns>
  ConfigNode LoadConfigWithComments(string fileFullName) {
    if (!File.Exists(fileFullName)) {
      return ConfigNode.Load(fileFullName);  // Just for the sake of the logs.
    }

    // $1 - key, $2 - other data
    var nodeMultiLinePrefixDeclRe = new Regex(@"^\s*([a-zA-Z_]+[a-zA-Z0-9_]*)$");
    // $1 - key, $2 - other data
    var nodeSameLineDeclRe = new Regex(@"^\s*([a-zA-Z_]+[a-zA-Z0-9_]*)\s*{\s*(.*?)?\s*$");
    // $1 - key, $2 - value
    var keyValueLineDeclRe = new Regex(@"^\s*([a-zA-Z_]+[a-zA-Z0-9_]*)\s*=\s*(.*?)$");

    var lines = File.ReadAllLines(fileFullName)
        .Select(x => x.Trim())
        .ToList();
    var nodesStack = new List<ConfigNode>() { new ConfigNode() };
    var node = nodesStack[0];
    var lineNum = 1;
    while (lines.Count > 0) {
      var line = lines[0];
      if (line.Length == 0) {
        lines.RemoveAt(0);
        lineNum++;
        node.AddValue("__commentField", "");
        continue;
      }
      
      // Check for the node section close.
      if (line.StartsWith("}", StringComparison.Ordinal)) {
        nodesStack.RemoveAt(nodesStack.Count - 1);
        if (nodesStack.Count == 0) {
          Debug.LogErrorFormat("Unexpected node close statement found at line {0} in {1}",
                               lineNum, fileFullName);
          return null;
        }
        node = nodesStack[nodesStack.Count - 1];
        if (line.Length == 1) {
          lines.RemoveAt(0);
          lineNum++;
        } else {
          lines[0] = line.Substring(1);
        }
        continue;
      }

      // Chop off the comment.
      string comment = null;
      var commentPos = line.IndexOf("//", StringComparison.Ordinal);
      if (commentPos != -1) {
        comment = line.Substring(commentPos + 2).TrimStart();
        line = line.Substring(0, commentPos).TrimEnd();
        if (line.Length == 0) {
          lines.RemoveAt(0);
          lineNum++;
          node.AddValue("__commentField", comment);
          continue;
        }
      }
      
      // Try handling the simples case: a key value pair (with an optional comment).
      var keyValueMatch = keyValueLineDeclRe.Match(line);
      if (keyValueMatch.Success) {
        // Localize the value if it starts from "#". There can be false positives.
        var value = keyValueMatch.Groups[2].Value;
        if (value.StartsWith("#", StringComparison.Ordinal)) {
          value = Localizer.Format(value);
        }
        node.AddValue(keyValueMatch.Groups[1].Value, value, comment);
        lines.RemoveAt(0);
        lineNum++;
        continue;
      }

      // Here it has to be a subnode. Check the otehr conditions it.
      string nodeName = null;
      string lineLeftOff = null;
      if (nodeSameLineDeclRe.IsMatch(line)) {
        // The node declaration starts on the same line. The can be more data in the same line!
        var sameLineMatch = nodeSameLineDeclRe.Match(line);
        nodeName = sameLineMatch.Groups[1].Value;
        lineLeftOff = sameLineMatch.Groups[2].Value;
        lines.RemoveAt(0);
        lineNum++;
      } else if (nodeMultiLinePrefixDeclRe.IsMatch(line)
                 && lines.Count > 0 && lines[1].StartsWith("{", StringComparison.Ordinal)) {
        // The node declaration starts on the next line.
        var multiLineMatch = nodeMultiLinePrefixDeclRe.Match(line);
        nodeName = multiLineMatch.Groups[1].Value;
        lines.RemoveAt(0);
        lineNum++;
        lineLeftOff = lines[0].Substring(1);  // Chop off "{"
        if (lineLeftOff.Length == 0) {
          lines.RemoveAt(0);
          lineNum++;
        } else {
          lines[0] = lineLeftOff;
        }
      }
      if (nodeName == null) {
        Debug.LogErrorFormat("Cannot parse node at line {0} in {1}", lineNum, fileFullName);
        return null;
      }
      var newNode = node.AddNode(nodeName, comment);
      nodesStack.Add(newNode);
      node = newNode;
    }
    
    return nodesStack[0];
  }
}

}  // namesapce
