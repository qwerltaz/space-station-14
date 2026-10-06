using Robust.Shared.Serialization;

namespace Content.Shared.DeviceLinking.Events;

[Serializable, NetSerializable]
public sealed class DeviceLinkDebugOverlayToggledEvent(bool enabled) : EntityEventArgs
{
    public readonly bool Enabled = enabled;
}

[Serializable, NetSerializable]
public sealed class DeviceLinkDebugOverlayUpdateEvent(List<DeviceLinkDebugLink> links) : EntityEventArgs
{
    public readonly List<DeviceLinkDebugLink> Links = links;
}

[Serializable, NetSerializable]
public readonly record struct DeviceLinkDebugLink(NetEntity Source, NetEntity Sink);
