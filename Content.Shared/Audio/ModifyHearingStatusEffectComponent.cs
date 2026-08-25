using Robust.Shared.GameStates;

namespace Content.Shared.Audio;

/// <summary>
/// Modifies the hearing of the entity, more when the effect is stronger.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ModifyHearingStatusEffectComponent : Component
{
    /// <summary>
    /// Max pitch modifier applied while the effect is active. Negative means decreased pitch.
    /// The actual effect gradually ramps up to this value when <see cref="DurationToMaxSeconds"/> is reached.
    /// </summary>
    [DataField(required: true)]
    public float MaxPitchModifier;

    /// <summary>
    /// The accumulated duration of the status effect at which <see cref="MaxPitchModifier"/> is reached.
    /// </summary>
    [DataField]
    public float DurationToMaxSeconds = 60f;

    /// <summary>
    /// How fast the effect ramps up based on its strength.
    /// </summary>
    [DataField]
    public float EffectRampUpSpeed = 1f;
}
