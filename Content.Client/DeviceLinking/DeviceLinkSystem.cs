using Content.Client.DeviceLinking.Overlays;
using Content.Shared.DeviceLinking;
using Content.Shared.DeviceLinking.Events;
using Robust.Client.Graphics;

namespace Content.Client.DeviceLinking;

public sealed partial class DeviceLinkSystem : SharedDeviceLinkSystem
{
    [Dependency] private IOverlayManager _overlayManager = default!;

    public List<DeviceLinkDebugLink> DebugLinks { get; private set; } = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<DeviceLinkDebugOverlayToggledEvent>(OnDebugOverlayToggled);
        SubscribeNetworkEvent<DeviceLinkDebugOverlayUpdateEvent>(OnDebugOverlayUpdate);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlayManager.RemoveOverlay<DeviceLinkDebugOverlay>();
    }

    private void OnDebugOverlayToggled(DeviceLinkDebugOverlayToggledEvent ev)
    {
        if (ev.Enabled)
            _overlayManager.AddOverlay(new DeviceLinkDebugOverlay());
        else
            _overlayManager.RemoveOverlay<DeviceLinkDebugOverlay>();
    }

    private void OnDebugOverlayUpdate(DeviceLinkDebugOverlayUpdateEvent ev)
    {
        DebugLinks = ev.Links;
    }
}
