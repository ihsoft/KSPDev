// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>
/// A simple annotation to associate a persistent group with a game's database key.
/// </summary>
/// <remarks>
/// <para>This annotation is basically a shortcut to <see cref="PersistentFieldsFileAttribute"/>.
/// You may use the latter with an empty config file path if you find it more convinient.</para>
/// <para>Each <c>.cfg</c> file in the mod's folders is scanned and indexed by the game on start. The
/// data is stored in the database where it can be accessed from the game. The access is read-only,
/// any changes to the node returned from the database won't affect the database state.</para>
/// <para>Config file in the database is indentified by a key which is made of three major parts:
/// <list type="">
/// <item>A file path relative to <c>GameData</c>.</item>
/// <item>The config filename without extension.</item>
/// <item>Node path inside the file starting from the root. I.e. the root node should have name as
/// well.</item>
/// </list>
/// </para>
/// <para>E.g. key <c>KIS/settings/KISConfig/Global</c> addresses a node <c>KISConfig/Global</c> in file
/// stored at <c>GameData/KIS/settings.cfg</c>.
/// </para>
/// <para>Special case is subfolders <c>PluginData</c>, they are ignored during database scan. Put
/// there configs that can change during the gameplay. Remember, that even a tiny change in the
/// config will trigger database re-compilation on the next start which may significantly impact
/// game loading time.</para>
/// <para>Database is actively used by
/// <see href="https://github.com/sarbian/ModuleManager">ModuleManager</see> (a.k.a. MM). Be wise
/// when choosing if fields should be read from a file or from the database. Module manager
/// patches are only applied on the database, they don't affect the config files. E.g. part configs
/// are frequently updated by the patches but never updated from the gameplay so, reading them via
/// database is the best choice. Various mod's settings, on the other hand, can change during the
/// game but will unlikely be a target for a MM patch, put them into <c>PluginData</c> and
/// read/write via file methods.</para>
/// </remarks>
/// <seealso cref="PersistentFieldsFileAttribute"/>
/// <seealso cref="ConfigAccessor.ReadFieldsInType"/>
/// <seealso cref="ConfigAccessor.WriteFieldsFromType"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class PersistentFieldsDatabaseAttribute : AbstractPersistentFieldsFileAttribute {
  /// <param name="nodePath">An absolute path to the node in the game's database.</param>
  /// <param name="group">A group of the annotation. When saving or loading persistent fields only
  /// the fields of this group will be considered. Must not be <c>null</c>.</param>
  public PersistentFieldsDatabaseAttribute(string nodePath,
                                           string group = StdPersistentGroups.Default)
      : base("", nodePath, group) {
  }
}

}  // namespace
