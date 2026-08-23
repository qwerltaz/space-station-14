using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Audio;

/// <summary>
///
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ModifyHearingStatusEffectComponent : Component
{
    [DataField]
    public float Pitch = 1f;
}
