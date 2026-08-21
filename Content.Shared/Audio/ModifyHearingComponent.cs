using Robust.Shared.GameStates;

namespace Content.Shared.Audio;

/// <summary>
///
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ModifyHearingComponent : Component
{
        [DataField]
        public float Volume = 1f;

        [DataField]
        public float Pitch = 1f;
}
