/// <summary>
/// Constants for Odin Inspector FoldoutGroup and BoxGroup attributes./>
/// </summary>
public static class FoldoutGroupNames
{

    /// <summary>
    /// For fields that are set in inspector.
    /// </summary>
    public const string SetInInspector = "Set In Inspector";

    /// <summary>
    /// For fields that are dynamic or used for debugging purposes.
    /// </summary>
    public const string DynamicDebug = "Debug-Dynamic";

    /// <summary>
    /// For fields that are editor only or used for debugging purposes.
    /// </summary>
    public const string EditorOnly = "Editor Only";

    /// <summary>
    /// For fields that are set in inspector.
    /// </summary>
    public const string BroadcastingOn = "Broadcasting On";

    /// <summary>
    /// For fields that are set in inspector.
    /// </summary>
    public const string ListeningOn = "Listening On";

    /// <summary>
    /// For fields that are set in inspector but marked obsolete.
    /// </summary>
    public const string SetInInspectorObsolete = SetInInspector + "/Obsolete";

    /// <summary>
    /// For fields that will be removed at later stage.
    /// </summary>
    public const string PendingRemoval = "Pending Removal";

    /// <summary>
    /// Used for grouping fields, event broadcasting.
    /// </summary>
    public const string SetInInspectorBroadcastingOn = SetInInspector + "/" + BroadcastingOn;

    /// <summary>
    /// For fields that represents notes for developer.
    /// </summary>
    public const string DeveloperDescription = "Developer Description";

}