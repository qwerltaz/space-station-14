using System.Linq;
using Content.Shared.Audio;
using Content.Shared.StatusEffectNew;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
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
        _sharedAudioSystem.SetAudioParams(args.Ent.Comp, new AudioParams().WithPitchScale(0.5f));
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


        var newAudioParams = new AudioParams
        {
            Volume = args.Ent.Comp.Volume * comp.Volume,
            Pitch = args.Ent.Comp.Pitch * comp.Pitch,
        };
        _sharedAudioSystem.SetAudioParams(args.Ent.Comp, newAudioParams);
    }
}
