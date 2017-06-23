// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A utility class to localize annotated members</summary>
/// <seealso cref="LocalizableItemAttribute"/>
/// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo1"/></example>
public static class LocalizationLoader {
  /// <summary>
  /// Specification for the <see cref="KSPField"/> <c>guiUnits</c> localization. 
  /// </summary>
  /// <remarks>
  /// Use it when specifying a <see cref="LocalizableItemAttribute"/> for a field with the units.
  /// </remarks>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField_WithUnits"/></example>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public const string KspFieldUnitsSpec = "units";
  
  /// <summary>Localizes the <see cref="PartModule"/> items.</summary>
  /// <remarks>
  /// <para>
  /// The localizable items must be declared as non-static public members. The following items are
  /// supported:
  /// <list type="bullet">
  /// <item>
  /// <see cref="KSPField"/>. This type may have multiple localization items: for <c>guiName</c>
  /// (spec=<c>null</c>) and for <c>guiUnits</c> (spec=<see cref="KspFieldUnitsSpec"/>).
  /// </item>
  /// <item><see cref="KSPEvent"/>.</item>
  /// <item><see cref="KSPAction"/>.</item>
  /// </list>
  /// </para>
  /// <para>
  /// The original KSP attributes don't need to specify <c>guiName</c> field since it will be
  /// overwritten anyways. However, it's a good idea to give a default value just in case.
  /// </para>
  /// <para>
  /// This method can be called at any time during the module's life. However, the
  /// <see cref="PartModule.OnAwake"/> method looks the most appropriate since it's called each time
  /// the module is created. The other methods may be called differently depending on the loaded
  /// scene.
  /// </para>
  /// </remarks>
  /// <param name="module">The module instance to localize.</param>
  /// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo1"/></example>
  /// <seealso cref="LocalizableItemAttribute"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPEvent']"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPAction']"/>
  public static void LoadItemsInModule(PartModule module) {
    module.Fields.Cast<BaseField>().ToList()
        .ForEach(LocalizeKSPField);

    // Hash the KSP annotated methods by the name. We only take the methods with zero arguments. 
    var methodItemTypes = new[] { typeof(KSPEvent), typeof(KSPAction) };
    var methodsByName = module.GetType()
        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .Where(m =>
             m.GetParameters().Length == 0
             && m.GetCustomAttributes(true).Any(o => methodItemTypes.Contains(o.GetType())))
        .ToDictionary(k => k.Name);

    module.Events
        .Where(e => methodsByName.ContainsKey(e.name))
        .ToList()
        .ForEach(x => LocalizeKSPEvent(methodsByName[x.name], x));
    module.Actions
        .Where(e => methodsByName.ContainsKey(e.name))
        .ToList()
        .ForEach(x => LocalizeKSPAction(methodsByName[x.name], x));
  }

  #region Local utility methods
  static void LocalizeKSPField(BaseField kspField) {
    var locItems = kspField.FieldInfo.GetCustomAttributes(false)
        .OfType<LocalizableItemAttribute>();
    foreach (var locItem in locItems) {
      if (string.IsNullOrEmpty(locItem.tag)) {
        continue;  // Localization is disabled for the item.
      }
      if (string.IsNullOrEmpty(locItem.spec)) {
        kspField.guiName = locItem.GetLocalizedString();
      } else if (locItem.spec == KspFieldUnitsSpec) {
        kspField.guiUnits = locItem.GetLocalizedString();
      } else {
        Debug.LogWarningFormat("Bad specialization tag for field {0}.{1}: {2}",
                               kspField.FieldInfo.FieldType.FullName,
                               kspField.FieldInfo.Name,
                               locItem.spec);
      }
    }
  }

  static void LocalizeKSPEvent(ICustomAttributeProvider info, BaseEvent item) {
    var locItem = info.GetCustomAttributes(false)
        .OfType<LocalizableItemAttribute>()
        .FirstOrDefault();
    if (locItem != null && !string.IsNullOrEmpty(locItem.tag)) {
      item.guiName = locItem.GetLocalizedString();
    }
  }

  static void LocalizeKSPAction(ICustomAttributeProvider info, BaseAction item) {
    var locItem = info.GetCustomAttributes(false)
        .OfType<LocalizableItemAttribute>()
        .FirstOrDefault();
    if (locItem != null && !string.IsNullOrEmpty(locItem.tag)) {
      item.guiName = locItem.GetLocalizedString();
    }
  }
  #endregion
}

}  // namespace
