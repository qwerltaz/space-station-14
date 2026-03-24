using System.Threading;
using Content.Server.Power.Components;
using Content.Server.StationEvents.Events;
using Content.Shared.Power;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(PowerGridCheckRule))]
public sealed partial class PowerGridCheckRuleComponent : Component
{
    /// <summary>
    /// Default sound for power restoration announcement.
    /// </summary>
    private static readonly ProtoId<SoundCollectionPrototype> DefaultPowerOn = new("PowerOn");

    /// <summary>
    /// Sound to play when power is restored.
    /// </summary>
    [DataField]
    public SoundSpecifier PowerOnSound = new SoundCollectionSpecifier(DefaultPowerOn, AudioParams.Default.WithVolume(-4f));

    /// <summary>
    /// Token source for cancelling the power restoration announcement.
    /// </summary>
    public CancellationTokenSource? AnnounceCancelToken;

    /// <summary>
    /// Power device group by which to identify target devices to turn off.
    /// The targets must also have a <see cref="PowerNetworkBatteryComponent"/>.
    /// </summary>
    public PowerMonitoringConsoleGroup TargetDeviceGroup = PowerMonitoringConsoleGroup.Substation;

    /// <summary>
    /// Station affected by the power grid event.
    /// </summary>
    [DataField]
    public EntityUid AffectedStation;

    /// <summary>
    /// List of battery entities that will be sequentially turned off during the event.
    /// </summary>
    [DataField]
    public List<EntityUid> Powered = new();

    /// <summary>
    /// List of battery entities that have been turned off.
    /// </summary>
    [DataField]
    public List<EntityUid> Unpowered = new();

    /// <summary>
    /// Time delay in seconds before starting to turn off targets.
    /// </summary>
    [DataField]
    public float SecondsUntilOff = 15.0f;

    /// <summary>
    /// Number of toggles to process per second during the shutdown phase.
    /// Dynamically calculated based on total count and <see cref="SecondsUntilOff"/>.
    /// </summary>
    public int NumberPerSecond = 0;

    /// <summary>
    /// Computed time interval between toggles.
    /// </summary>
    public float UpdateRate => 1.0f / NumberPerSecond;

    /// <summary>
    /// Accumulated frame time to track when to process the next toggle.
    /// </summary>
    [DataField]
    public float FrameTimeAccumulator = 0.0f;
}
