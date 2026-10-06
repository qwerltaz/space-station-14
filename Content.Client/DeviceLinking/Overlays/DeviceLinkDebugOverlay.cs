using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Map;

namespace Content.Client.DeviceLinking.Overlays;

public sealed partial class DeviceLinkDebugOverlay : Overlay
{
    [Dependency] private IEntityManager _entityManager = default!;

    private readonly SharedTransformSystem _transform;
    private readonly DeviceLinkSystem _deviceLink;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    public DeviceLinkDebugOverlay()
    {
        IoCManager.InjectDependencies(this);
        _deviceLink = _entityManager.System<DeviceLinkSystem>();
        _transform = _entityManager.System<SharedTransformSystem>();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        foreach (var link in _deviceLink.DebugLinks)
        {
            var source = _entityManager.GetEntity(link.Source);
            var sink = _entityManager.GetEntity(link.Sink);

            if (!_entityManager.EntityExists(source) || !_entityManager.EntityExists(sink))
                continue;
            if (!_entityManager.TryGetComponent(source, out TransformComponent? sourceTransform)
                || !_entityManager.TryGetComponent(sink, out TransformComponent? sinkTransform))
                continue;
            if (sourceTransform.MapID != args.MapId
                || sinkTransform.MapID != args.MapId
                || sourceTransform.MapID == MapId.Nullspace
                || sinkTransform.MapID == MapId.Nullspace)
                continue;

            args.WorldHandle.DrawLine(
                _transform.GetWorldPosition(sourceTransform),
                _transform.GetWorldPosition(sinkTransform),
                Color.Cyan);
        }
    }
}
