using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Audio;

/// <summary>
/// <see cref="ModifyHearingComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ModifyHearingStatusEffectComponent : Component
{
    [DataField]
    public float Volume = 1f;

    [DataField]
    public float Pitch = 1f;
}
