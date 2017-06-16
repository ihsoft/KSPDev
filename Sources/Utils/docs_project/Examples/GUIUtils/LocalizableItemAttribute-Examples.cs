// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

public class LocalizableItemAttributeDemo1 : PartModule {
  #region ItemField
  [KSPField(guiName = "just-in-case text", guiActive = true)]
  [LocalizableItem(
      tag = "#tag1",
      defaultTemplate = "English text 1",
      description = "A field which demonstrates a localizable GUI string")]
  public string field1 = "";

  // This field doesn't have guiName and it would, normally, be treated as a non-GUI field. However,
  // due to the localization attribute, the guiName value will be assigned when the appropriate
  // method is called.
  [KSPField(guiActive = true)]
  [LocalizableItem(
      tag = "#tag2",
      defaultTemplate = "English text 2",
      description = "A field which demonstrates a localizable GUI string without guiName")]
  public string field2 = "";
  #endregion

  #region ItemField_WithUnits
  [KSPField(guiName = "just-in-case text", guiUnits = "just-in-case units", guiActive = true)]
  [LocalizableItem(
      tag = "#tag1",
      defaultTemplate = "English text 1",
      description = "A field which demonstrates a localizable GUI string")]
  [LocalizableItem(
      tag = "#tag1_units",
      spec = LocalizableItemAttribute.Spec.KspFieldUnits,
      defaultTemplate = "meters",
      description = "Units for the field which demonstrates a localizable GUI string")]
  public string field3 = "";
  #endregion

  #region ItemEvent
  [KSPEvent(guiName = "just-in-case text", guiActive = true)]
  [LocalizableItem(
      tag = "#tag1",
      defaultTemplate = "English text",
      description = "A field which demonstrates a localizable event")]
  public void MyEvent1() {
    Debug.Log("MyEvent1 invoked");
  }
  #endregion

  #region ItemAction
  [KSPEvent(guiName = "just-in-case text", guiActive = true)]
  [LocalizableItem(
      tag = "#tag1",
      defaultTemplate = "English text",
      description = "A field which demonstrates a localizable action")]
  public void MyAction1() {
    Debug.Log("MyAction1 invoked");
  }
  #endregion
}

}  // namespace
