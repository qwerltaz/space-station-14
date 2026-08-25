using Content.Shared.Audio;
using Content.Shared.CCVar;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Client.Audio;

public sealed partial class ModifyHearingSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private AudioSystem _audioSystem = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private Shared.StatusEffectNew.StatusEffectsSystem _statusEffectsSystem = default!;

    private bool _disableModifyHearingEffect;

    public override void Initialize()
    {
        base.Initialize();

        _cfg.OnValueChanged(CCVars.DisableSoundDistortions, value => _disableModifyHearingEffect = value, true);
    }

    [SubscribeLocalEvent]
    private void OnSound(ref AudioStartupEvent args)
    {
        if (_disableModifyHearingEffect)
            return;

        var localEntity = _playerManager.LocalEntity;
        if (localEntity is null)
            return;

        if (!_statusEffectsSystem.TryGetMaxTime<ModifyHearingStatusEffectComponent>(localEntity.Value,
                out var effectTime))
            return;

        var effectComponent = Comp<ModifyHearingStatusEffectComponent>(effectTime.EffectEnt);
        var pitchModifier = GetPitchModifier(effectComponent, effectTime.EndEffectTime);

        var newAudioParams = args.Ent.Comp.Params;
        var newPitch = args.Ent.Comp.Params.Pitch + pitchModifier;
        newAudioParams = newAudioParams.WithPitchScale(newPitch);

        _audioSystem.SetAudioParams(args.Ent.Comp, newAudioParams);
    }

    private float GetPitchModifier(ModifyHearingStatusEffectComponent comp, TimeSpan? endEffectTime)
    {
        if (comp.DurationToMaxSeconds <= 0f)
            return comp.MaxPitchModifier;

        var remainingSeconds = endEffectTime == null
            ? comp.DurationToMaxSeconds
            : Math.Max(0f, (float)(endEffectTime.Value - _timing.CurTime).TotalSeconds) * comp.EffectRampUpSpeed;

        var normalizedDuration = Math.Clamp(remainingSeconds / comp.DurationToMaxSeconds, 0f, 1f);

        return comp.MaxPitchModifier * normalizedDuration;
    }
}
