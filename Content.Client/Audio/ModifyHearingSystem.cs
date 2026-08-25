using Content.Shared.Audio;
using Content.Shared.StatusEffectNew;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Client.Audio;

public sealed partial class ModifyHearingSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private AudioSystem _audioSystem = default!;
    [Dependency] private StatusEffectsSystem _statusEffectsSystem = default!;

    [SubscribeLocalEvent]
    private void OnSound(ref AudioStartupEvent args)
    {
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

    private float GetPitchModifier(ModifyHearingStatusEffectComponent effect, TimeSpan? endEffectTime)
    {
        if (effect.DurationToMaxPower <= 0f)
            return effect.BasePitchModifier;

        var remainingSeconds = endEffectTime == null
            ? effect.DurationToMaxPower
            : Math.Max(0f, (float)(endEffectTime.Value - _timing.CurTime).TotalSeconds);

        var normalizedDuration = Math.Clamp(remainingSeconds / effect.DurationToMaxPower, 0f, 1f);
        var maxPitchModifier = Math.Max(1f, effect.MaxEffectMultiplier);
        var durationMultiplier = MathHelper.Lerp(1f, maxPitchModifier, normalizedDuration);

        return effect.BasePitchModifier * durationMultiplier;
    }
}
