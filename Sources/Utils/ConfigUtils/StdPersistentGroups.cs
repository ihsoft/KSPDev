// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

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
  public const string Default = "";

  /// <summary>
  /// A public group that designates the fields that are saved/loaded durint the vessel persistense.
  /// </summary>
  /// <remarks>
  /// The fields of this group are expected to be loaded/saved from the related <c>PartModule</c>
  /// methods: <c>OnSave</c> and <c>OnLoad</c>. Dealing with them outside of this logic is highly
  /// discouraged.
  /// </remarks>
  public const string PartPersistant = "PartPersistant";
}

