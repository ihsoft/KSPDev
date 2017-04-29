// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.ConfigUtils {

/// <summary>Interface for the simple types that need custom (de)serialization logic.</summary>
/// <remarks>
/// It's similar to <c>IConfigNode</c> interface in the compound types but with the following
/// differences:
/// <list type="bullet">
/// <item>The value is (de)serialized from/to a simple string.</item>
/// <item>
/// If the field is initialized to an instance of the type, then this instance will be used to
/// deserialize the value. If the field is not initialized but there is a value in the config file,
/// then a new instance will be created. For this reason the type must implement a default
/// constructor.
/// </item>
/// </list>
/// <para>
/// Note that the types that implement this interface will <i>never</i> be treated as compound. I.e.
/// <see cref="ConfigAccessor"/> will not try to persist the members of such types even though there
/// may be fields attributed with <see cref="PersistentFieldAttribute"/>.
/// </para>
/// </remarks>
/// <example>
/// Here is how a simple vector serialization may look like:
/// <code><![CDATA[
/// public class MyVector : IPeristentField {
///   float x;
///   float y;
///
///   /// <inheritdoc/>
///   public string SerializeToString() {
///     return string.Format("{0},{1}", x ,y);
///   }
///   /// <inheritdoc/>
///   public void ParseFromString(string value) {
///     var elements = value.Split(',');
///     x = float.Parse(elements[0]);
///     y = float.Parse(elements[1]);
///   }
/// }
/// ]]></code>
/// <para>
/// This example doesn't do any checking when parsing the string, but in general it's a good idea to
/// do a sanity check of the string. It's OK to throw an exception from the parsing method when the
/// data is invalid.
/// </para>
/// </example>
/// <seealso cref="ConfigAccessor"/>
/// <seealso cref="PersistentFieldAttribute"/>
public interface IPersistentField {
  /// <summary>Returns the object's state as a plain string.</summary>
  /// <returns>Object's state.</returns>
  string SerializeToString();

  /// <summary>Restores the object's state from a plain string.</summary>
  /// <param name="value">String value to restore from.</param>
  /// <remarks>It's OK to throw exceptions if the value cannot be parsed.</remarks>
  void ParseFromString(string value);
}

}  // namespace

