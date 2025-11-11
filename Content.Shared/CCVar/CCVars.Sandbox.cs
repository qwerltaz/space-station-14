using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Max entities the player can spawn within one <see cref="SandboxEntitySpawnTimeFrameLengthSeconds"/>
    /// in sandbox mode.
    /// </summary>
    public static readonly CVarDef<int>
        SandboxMaxEntitySpawnsPerTimeFrame = CVarDef.Create("sandbox.max_entity_spawns_per_time_frame",
            20,
            CVar.ARCHIVE | CVar.CLIENTONLY);

    /// <summary>
    /// Length of the time frame in which to count the number of the player's recent entity spawns
    /// and compare against the allowed maximum.
    /// </summary>
    public static readonly CVarDef<float>
        SandboxEntitySpawnTimeFrameLengthSeconds = CVarDef.Create("sandbox.entity_spawn_time_frame_length_seconds",
            5.0f,
            CVar.ARCHIVE | CVar.CLIENTONLY);
}
