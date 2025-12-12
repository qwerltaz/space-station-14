using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Audio;

/// <summary>
///
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ModifyAllSoundStatusEffectComponent : Component
{
    [DataField]
    public AudioParams ModifiedAudioParams = AudioParams.Default.WithPitchScale(2.0f);
}
