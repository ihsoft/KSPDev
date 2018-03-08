// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;

namespace ConfigUtils.Examples {

#region ReadPartConfigExample
public class ReadPartConfigExample : PartModule {
  public override void OnAwake() {
    base.OnAwake();
    // Load part's config custom fields in teh scenes that create the parts.
    ConfigAccessor.ReadPartConfig(this);
  }

  public override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    // Load part's config custom fields in teh scenes that clone the parts.
    ConfigAccessor.ReadPartConfig(this);
  }
}
#endregion

}  // namespace
