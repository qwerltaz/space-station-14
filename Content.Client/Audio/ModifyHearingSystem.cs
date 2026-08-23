using System.Linq;
using Content.Shared.Audio;
using Content.Shared.StatusEffectNew;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared.Audio.Systems;

namespace Content.Client.Audio;

public sealed partial class ModifyHearingSystem : EntitySystem
{
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private AudioSystem _audioSystem = default!;
    [Dependency] private StatusEffectsSystem _statusEffectsSystem = default!;

    [SubscribeLocalEvent]
    private void OnAudioStartup(ref AudioStartupEvent args)
    {
        if (_playerManager.LocalEntity is null)
        {
            return;
        }

        if (!_statusEffectsSystem.TryEffectsWithComp<ModifyHearingStatusEffectComponent>(
                _playerManager.LocalEntity.Value,
                out var comps))
        {
            return;
        }

        var comp = (ModifyHearingStatusEffectComponent)comps.First();

        var outputPitch = args.Ent.Comp.Params.Pitch * comp.Pitch;

        var newAudioParams = args.Ent.Comp.Params;
        newAudioParams = newAudioParams
            .WithPitchScale(outputPitch);

        _audioSystem.SetAudioParams(args.Ent.Comp, newAudioParams);
    }
}
