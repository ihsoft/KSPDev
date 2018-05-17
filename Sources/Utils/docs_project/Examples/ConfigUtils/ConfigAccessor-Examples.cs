// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;

namespace ConfigUtils.Examples {

#region ReadPartConfigExample
public class ReadPartConfigExample : PartModule {
  public override void OnAwake() {
    base.OnAwake();
    // get hthe custom fields from the prefab.
    ConfigAccessor.CopyPartConfigFromPrefab(this);
  }

  public override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    // Load part's config custom fields in the scenes that clone the parts.
    ConfigAccessor.ReadPartConfig(this, node);
  }
}
#endregion

}  // namespace
