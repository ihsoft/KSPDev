using System;
using System.Reflection;
using System.Linq;
using KSPDev.LogUtils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace KSPDev.ConfigUtils {

//TODO: add help
public class PersistentField {
  // TODO: make all members readonly and implement a ctor.
  public readonly FieldInfo fieldInfo;
  public readonly string[] cfgPath;
  
  public CompoundValueHandler compoundValueHandler;
  public SimpleValueHandler simpleValueHandler;
  public RepeatableFieldHandler repeatableFieldHandler;
  public PersistentField[] compoundTypeFields;
  
  // FIXME: add isCOmpound/isRepeatable
  
  public PersistentField(FieldInfo fieldInfo, string[] cfgPath) {
    this.fieldInfo = fieldInfo;
    this.cfgPath = cfgPath;
  }
  
  public void ReadFromConfig(ConfigNode node, object obj) {
    object instance = null;
    if (repeatableFieldHandler != null) {
      instance = repeatableFieldHandler.CreateFromConfigNode(node);
    } else if (compoundValueHandler != null) {
      var compoundNode = ConfigAccessor.GetNodeByPath(node, cfgPath);
      instance = compoundValueHandler.CreateFromConfigNode(compoundNode, fieldInfo.FieldType);
    } else if (simpleValueHandler != null) {
      var value = ConfigAccessor.GetValue(node, cfgPath);
      if (value != null) {
        instance = simpleValueHandler.CreateFromString(fieldInfo.FieldType, value);
      }
    } else {
      Logger.logError("No loading field {0} due to unexpected attributes setup", fieldInfo.Name);
      return;
    }
    if (instance != null) {
      fieldInfo.SetValue(obj, instance);
    }
  }
}

}  // namespace
