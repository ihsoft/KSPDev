// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;

namespace Examples {

#region LocalizationLoaderDemo1
public class LocalizationLoaderDemo1 : PartModule {
  public class OtherModule : PartModule {
    [KSPField(guiName = "just-in-case text", guiActive = true)]
    [LocalizableItem(tag = "#tag1")]
    public string field1 = "";
  }

  public void AddCustomModule() {
    var newModule = gameObject.AddComponent<OtherModule>();
    LocalizationLoader.LoadItemsInModule(newModule);
  }
}
#endregion

}  // namespace
