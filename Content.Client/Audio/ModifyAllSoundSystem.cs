using Content.Shared.Audio;
using Content.Shared.StatusEffectNew;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared.Audio.Components;
using Robust.Shared.Player;

namespace Content.Client.Audio;

public sealed class ModifyAllSoundSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;

    private bool _enabled;
    private float _volumeMultiplier = 1f;
    private float _pitchMultiplier = 1f;

    public override void Initialize()
    {
        base.Initialize();

        UpdatesAfter.Add(typeof(AudioSystem));

        SubscribeLocalEvent<ModifyAllSoundStatusEffectComponent, StatusEffectAppliedEvent>(OnModifyAudioParamsApply);
        SubscribeLocalEvent<ModifyAllSoundStatusEffectComponent, StatusEffectRemovedEvent>(OnModifyAudioParamsShutdown);
        SubscribeLocalEvent<ModifyAllSoundStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>(
            OnPlayerAttached);
        SubscribeLocalEvent<ModifyAllSoundStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>(
            OnPlayerDetached);
    }

    public override void FrameUpdate(float frameTime)
    {
        if (!_enabled)
            return;

        var query = AllEntityQuery<AudioComponent>();
        while (query.MoveNext(out _, out var audio))
        {
            if (!audio.Started)
                continue;

            audio.Volume = audio.Params.Volume * _volumeMultiplier;
            audio.Pitch = audio.Params.Pitch * _pitchMultiplier;
        }
    }

    private void OnModifyAudioParamsApply(Entity<ModifyAllSoundStatusEffectComponent> ent,
        ref StatusEffectAppliedEvent args)
    {
        if (_player.LocalEntity != args.Target)
            return;

        _enabled = true;
        _volumeMultiplier = ent.Comp.Volume;
        _pitchMultiplier = ent.Comp.Pitch;
    }

    private void OnModifyAudioParamsShutdown(Entity<ModifyAllSoundStatusEffectComponent> ent,
        ref StatusEffectRemovedEvent args)
    {
        if (_player.LocalEntity != args.Target)
            return;

        _enabled = false;
    }

    private void OnPlayerAttached(Entity<ModifyAllSoundStatusEffectComponent> ent,
        ref StatusEffectRelayedEvent<LocalPlayerAttachedEvent> args)
    {
        _enabled = true;
    }

    private void OnPlayerDetached(Entity<ModifyAllSoundStatusEffectComponent> ent,
        ref StatusEffectRelayedEvent<LocalPlayerDetachedEvent> args)
    {
        _enabled = false;
    }
}
