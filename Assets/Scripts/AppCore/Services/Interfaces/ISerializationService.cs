using Newtonsoft.Json;

/// <summary>
/// Interface used for JSON serialization/deserialization.
/// </summary>
public interface ISerializationService
{
    /// <summary>
    /// Deserializes the JSON to a .NET object using <see cref="JsonSerializerSettings"/>.
    /// </summary>
    /// <param name="json">The JSON to deserialize.</param>
    /// <param name="settings">The <see cref="JsonSerializerSettings"/> instance.
    /// If this is <c>null</c>, default serialization settings will be used.
    /// </param>
    /// <typeparam name="T">The type of object.</typeparam>
    /// <returns>The deserialized object from the JSON string.</returns>
    T Deserialize<T>(string json, JsonSerializerSettings settings = null);

    /// <summary>
    /// Serializes the specified object to a JSON string using <see cref="JsonSerializerSettings"/>.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
    /// If this is <c>null</c>, default serialization settings will be used.</param>
    /// <returns>
    /// A JSON string representation of the object.
    /// </returns>
    string Serialize<T>(T value, JsonSerializerSettings settings = null);

}
