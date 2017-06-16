// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;

namespace Examples {

#region LocalizationLoaderDemo1
public class LocalizationLoaderDemo1 : PartModule {
  [KSPField(guiName = "just-in-case text", guiActive = true)]
  [LocalizableItem(
      tag = "#tag1",
      defaultTemplate = "English text 1",
      description = "A field which demonstrates a localizable GUI string")]
  public string field1 = "";

  public override void OnAwake() {
    base.OnAwake();
    // The loader will overwrite guiName in field1.
    LocalizationLoader.LoadItemsInModule(this);
  }
}
#endregion

}  // namespace
