using System.Linq;
using Content.Client.Items;
using Content.Client.Storage.Systems;
using Content.Shared.Stacks;
using Content.Shared.Storage.Components;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Stack
{
    [UsedImplicitly]
    public sealed class StackSystem : SharedStackSystem
    {
        [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly ItemCounterSystem _counterSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<StackComponent, AppearanceChangeEvent>(OnAppearanceChange);
            Subs.ItemStatus<StackComponent>(ent => new StackStatusControl(ent));
        }

        public override void SetCount(EntityUid uid, int amount, StackComponent? component = null)
        {
            if (!Resolve(uid, ref component))
                return;

            base.SetCount(uid, amount, component);

            if (component.Lingering &&
                TryComp<SpriteComponent>(uid, out var sprite))
            {
                // tint the stack gray and make it transparent if it's lingering.
                var color = component.Count == 0 && component.Lingering
                    ? Color.DarkGray.WithAlpha(0.65f)
                    : Color.White;

                for (var i = 0; i < sprite.AllLayers.Count(); i++)
                {
                    sprite.LayerSetColor(i, color);
                }
            }

            // TODO PREDICT ENTITY DELETION: This should really just be a normal entity deletion call.
            if (component.Count <= 0 && !component.Lingering)
            {
                Xform.DetachEntity(uid, Transform(uid));
                return;
            }

            component.UiUpdateNeeded = true;
        }

        private void OnAppearanceChange(EntityUid uid, StackComponent comp, ref AppearanceChangeEvent args)
        {
            var itemCounter = EnsureComp<ItemCounterComponent>(uid);
            _counterSystem.OnAppearanceChange(uid, itemCounter, ref args);
        }
    }
}
