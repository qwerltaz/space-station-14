using System.Linq;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.DeviceLinking;
using Content.Shared.DeviceLinking.Events;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.Player;

namespace Content.Server.DeviceLinking.Systems;

public sealed partial class DeviceLinkSystem
{
    private readonly HashSet<ICommonSession> _debugSessions = new();

    public void ToggleDebugView(ICommonSession session)
    {
        var enabled = !_debugSessions.Remove(session);
        if (enabled)
            _debugSessions.Add(session);

        RaiseNetworkEvent(new DeviceLinkDebugOverlayToggledEvent(enabled), session.Channel);
        if (enabled)
            SendDebugUpdate(session);
    }

    protected override void OnLinksChanged()
    {
        base.OnLinksChanged();

        if (_debugSessions.Count == 0)
            return;

        var update = new DeviceLinkDebugOverlayUpdateEvent(GetDebugLinks());
        foreach (var session in _debugSessions.ToArray())
        {
            if (session.Status != SessionStatus.InGame)
            {
                _debugSessions.Remove(session);
                continue;
            }

            RaiseNetworkEvent(update, session.Channel);
        }
    }

    private void SendDebugUpdate(ICommonSession session)
    {
        RaiseNetworkEvent(new DeviceLinkDebugOverlayUpdateEvent(GetDebugLinks()), session.Channel);
    }

    private List<DeviceLinkDebugLink> GetDebugLinks()
    {
        var links = new HashSet<DeviceLinkDebugLink>();
        var query = EntityQueryEnumerator<DeviceLinkSourceComponent>();
        while (query.MoveNext(out var source, out var sourceComponent))
        {
            foreach (var sink in sourceComponent.LinkedPorts.Keys)
            {
                if (EntityManager.Deleted(source) || EntityManager.Deleted(sink))
                    continue;

                links.Add(new DeviceLinkDebugLink(GetNetEntity(source), GetNetEntity(sink)));
            }
        }

        return links.ToList();
    }
}

[AdminCommand(AdminFlags.Admin)]
public sealed partial class DeviceLinkViewCommand : LocalizedEntityCommands
{
    [Dependency] private DeviceLinkSystem _deviceLink = default!;

    public override string Command => "showdevicelinks";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is { } session)
            _deviceLink.ToggleDebugView(session);
    }
}
