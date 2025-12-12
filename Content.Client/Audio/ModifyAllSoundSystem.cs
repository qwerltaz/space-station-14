using Content.Shared.Audio;
using Content.Shared.StatusEffectNew;
using Robust.Client.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;

namespace Content.Client.Audio;

public sealed class ModifyAllSoundSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audioSystem = default!;

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

        ent.Comp.Params = _params.Value;
    }
}
