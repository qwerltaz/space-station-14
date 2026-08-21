using Content.Shared.StatusEffectNew;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;

namespace Content.Shared.Audio;

public abstract partial class SharedModifyHearingSystem : EntitySystem
{
    [Dependency] private ISharedPlayerManager _playerManager = default!;
    [Dependency] private SharedAudioSystem _sharedAudioSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        UpdatesAfter.Add(typeof(SharedAudioSystem));

        SubscribeLocalEvent<ModifyHearingStatusEffectComponent, StatusEffectAppliedEvent>(OnModifyAudioParamsApply);
        SubscribeLocalEvent<ModifyHearingStatusEffectComponent, StatusEffectRemovedEvent>(OnModifyAudioParamsRemoved);
        // SubscribeLocalEvent<ModifyHearingStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>(
        // OnPlayerAttached);
        // SubscribeLocalEvent<ModifyHearingStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>(
        // OnPlayerDetached);
    }

    private void OnModifyAudioParamsApply(Entity<ModifyHearingStatusEffectComponent> ent,
        ref StatusEffectAppliedEvent args)
    {
        // if (_playerManager.LocalEntity != args.Target)
        //     return;

        EnsureComp<ModifyHearingComponent>(args.Target, out var comp);
        comp.Volume = ent.Comp.Volume;
        comp.Pitch = ent.Comp.Pitch;
    }

    private void OnModifyAudioParamsRemoved(Entity<ModifyHearingStatusEffectComponent> ent,
        ref StatusEffectRemovedEvent args)
    {
        // if (_playerManager.LocalEntity != args.Target)
        //     return;

        RemComp<ModifyHearingComponent>(args.Target);
    }

    private void OnPlayerAttached(Entity<ModifyHearingStatusEffectComponent> ent,
        ref StatusEffectRelayedEvent<LocalPlayerAttachedEvent> args)
    {
    }

    private void OnPlayerDetached(Entity<ModifyHearingStatusEffectComponent> ent,
        ref StatusEffectRelayedEvent<LocalPlayerDetachedEvent> args)
    {
    }
}
