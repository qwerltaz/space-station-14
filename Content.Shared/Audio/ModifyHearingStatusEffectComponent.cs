using Robust.Shared.GameStates;

namespace Content.Shared.Audio;

/// <summary>
/// Modifies the hearing of the entity, more when the effect is stronger.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ModifyHearingStatusEffectComponent : Component
{
    /// <summary>
    /// Base pitch modifier applied while the effect is active. Negative means decreased pitch.
    /// </summary>
    [DataField(required: true)]
    public float BasePitchModifier;

    /// <summary>
    /// Max strength by which the base effect is multiplied
    /// when the status effect duration reaches <see cref="DurationToMaxPower"/>.
    /// </summary>
    [DataField]
    public float MaxEffectMultiplier = 2f;

    /// <summary>
    /// The duration of the status effect after which max effect strength is reached.
    /// </summary>
    [DataField]
    public float DurationToMaxPower = 60f;
}
