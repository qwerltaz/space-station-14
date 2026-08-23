using Robust.Shared.GameStates;

namespace Content.Shared.Audio;

/// <summary>
/// Modifies the hearing of the entity.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ModifyHearingStatusEffectComponent : Component
{
    /// <summary>
    /// Base pitch multiplier applied while the effect is active.
    /// </summary>
    [DataField(required: true)]
    public float Pitch;

    /// <summary>
    /// Max strength by which the effect is multiplied
    /// when the status effect duration reaches <see cref="DurationToMaxPower"/>.
    /// </summary>
    [DataField]
    public float MaxEffectMultiplier = 2.5f;

    /// <summary>
    /// The duration of the status effect after which max effect strength is reached.
    /// </summary>
    [DataField]
    public float DurationToMaxPower = 60f;
}
