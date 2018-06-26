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

#region LocalizationLoaderDemo2
public class LocalizationLoaderDemo2 : PartModule, IsLocalizableModule {
  [KSPField(guiActive = true)]
  [LocalizableItem(tag = "#tag1", defaultTemplate = "Field1")]
  [LocalizableItem(tag = "#tag2", defaultTemplate = "units",
                   spec = StdSpecTags.Units)]
  public string field1 = "";

  #region IsLocalizableModule implementation
  public void LocalizeModule() {
    LocalizationLoader.LoadItemsInModule(this);
  }
  #endregion

  public override void OnAwake() {
    base.OnAwake();
    LocalizeModule();
  }
}
#endregion

}  // namespace
