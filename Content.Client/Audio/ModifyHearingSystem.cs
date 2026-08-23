using System.Linq;
using Content.Shared.Audio;
using Content.Shared.StatusEffectNew;
using Robust.Client.Player;
using Robust.Shared.Audio.Systems;

namespace Content.Client.Audio;

public sealed partial class ModifyHearingSystem : EntitySystem
{
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private SharedAudioSystem _sharedAudioSystem = default!;
    [Dependency] private StatusEffectsSystem _statusEffectsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AudioStartupEvent>(OnAudioStartup);
    }

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
        var outputVolume = args.Ent.Comp.Params.Volume * comp.Volume;

        var newAudioParams = args.Ent.Comp.Params
            .WithPitchScale(outputPitch)
            .WithVolume(outputVolume);

        _sharedAudioSystem.SetAudioParams(args.Ent.Comp, newAudioParams);
    }
}
