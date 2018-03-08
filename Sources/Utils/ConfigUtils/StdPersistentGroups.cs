// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.ConfigUtils {

/// <summary>Group names that have special meaning.</summary>
/// <seealso cref="KSPDev.ConfigUtils.ConfigAccessor"/>
/// <seealso cref="KSPDev.ConfigUtils.PersistentFieldAttribute"/>
public static class StdPersistentGroups {
  /// <summary>A public group that can be saved/loaded on every game scene.</summary>
  /// <remarks>
  /// By the contract any caller can save/load this group at any time. If the class declares the
  /// persistent fields with a specific save/load logic, then they need to have a group different
  /// from the default.
  /// </remarks>
  /// <seealso cref="PersistentFieldAttribute"/>
  public const string Default = "";

  /// <summary>
  /// A public group that designates the fields that are saved/loaded during the vessel persistense.
  /// </summary>
  /// <remarks>
  /// The fields of this group are expected to be loaded/saved from the related <c>PartModule</c>
  /// methods: <c>OnSave</c> and <c>OnLoad</c>. Dealing with them outside of this logic is highly
  /// discouraged.
  /// </remarks>
  /// <seealso cref="PersistentFieldAttribute"/>
  public const string PartPersistant = "PartPersistant";

  /// <summary>A public group for the feilds that needs to be loaded from a part config.</summary>
  /// <remarks>
  /// It's a very special group, never deal with it directly. The consumer code is only allowed to
  /// use this group when defining the persistent fields via annotations.  
  /// </remarks>
  /// <seealso cref="PersistentFieldAttribute"/>
  /// <seealso cref="ConfigAccessor.ReadPartConfig"/>
  public const string PartConfigLoadGroup = "PartConfig";
}

}  // namespace
