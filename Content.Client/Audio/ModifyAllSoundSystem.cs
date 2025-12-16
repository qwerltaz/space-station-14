using Content.Shared.Audio;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;

namespace Content.Client.Audio;

public sealed class ModifyAllSoundSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _sharedAudioSystem = default!;

    private bool _enabled;
    private AudioParams? _params;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ModifyAllSoundStatusEffectComponent, StatusEffectAppliedEvent>(OnModifyAudioParamsApply);
        SubscribeLocalEvent<ModifyAllSoundStatusEffectComponent, StatusEffectRemovedEvent>(OnModifyAudioParamsShutdown);
        SubscribeLocalEvent<AudioComponent, ComponentInit>(OnAudioComponentInit);
    }

    private void OnModifyAudioParamsApply(Entity<ModifyAllSoundStatusEffectComponent> ent,
        ref StatusEffectAppliedEvent args)
    {
        _enabled = true;
        _params = ent.Comp.ModifiedAudioParams;

        var query = AllEntityQuery<AudioComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            _sharedAudioSystem.SetAudioParams(comp, _params.Value);
        }
    }

    private void OnModifyAudioParamsShutdown(Entity<ModifyAllSoundStatusEffectComponent> ent,
        ref StatusEffectRemovedEvent args)
    {
        _enabled = false;
        _params = null;
    }

    private void OnAudioComponentInit(Entity<AudioComponent> ent, ref ComponentInit args)
    {
        if (!_enabled || _params is null)
            return;

        _sharedAudioSystem.SetAudioParams(ent.Comp, _params.Value);
    }

    private void OnAudioState(Entity<AudioComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (!_enabled || _params is null || ent.Comp.State != AudioState.Playing)
            return;

        _sharedAudioSystem.SetAudioParams(ent.Comp, _params.Value);
    }
}
