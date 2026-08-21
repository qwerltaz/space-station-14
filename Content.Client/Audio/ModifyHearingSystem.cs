using Content.Shared.Audio;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;

namespace Content.Client.Audio;

public sealed partial class ModifyHearingSystem : SharedModifyHearingSystem
{
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private SharedAudioSystem _sharedAudioSystem = default!;

    public override void FrameUpdate(float frameTime)
    {
        if (_playerManager.LocalEntity is null)
        {
            return;
        }

        if (!TryComp<ModifyHearingComponent>(_playerManager.LocalEntity, out var comp))
        {
            return;
        }

        var query = AllEntityQuery<AudioComponent>();
        while (query.MoveNext(out _, out var audio))
        {
            if (!audio.Started)
                continue;

            var newAudioParams = new AudioParams
            {
                Volume = audio.Volume * comp.Volume,
                Pitch = audio.Pitch * comp.Pitch,
            };
            _sharedAudioSystem.SetAudioParams(audio, newAudioParams);
        }
    }
}
