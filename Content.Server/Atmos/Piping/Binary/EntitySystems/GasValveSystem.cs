using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Binary.Systems;
using Content.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems;

public sealed class GasValveSystem : SharedGasValveSystem
{
    [Dependency] private readonly SharedAmbientSoundSystem _ambientSoundSystem = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var gasValveQuery = EntityQueryEnumerator<GasValveComponent>();
        while (gasValveQuery.MoveNext(out var uid, out var component))
        {
            if (!_nodeContainer.TryGetNodes(uid,
                    component.InletName,
                    component.OutletName,
                    out PipeNode? inlet,
                    out PipeNode? outlet))
                continue;

            if (!component.Open)
                continue;

            var pressure = inlet.Air.Pressure;
            var volume = MathF.Asinh(pressure);
            _ambientSoundSystem.SetVolume(uid, volume);

        }
    }

    public override void Set(EntityUid uid, GasValveComponent component, bool value)
    {
        base.Set(uid, component, value);

        if (_nodeContainer.TryGetNodes(uid, component.InletName, component.OutletName, out PipeNode? inlet, out PipeNode? outlet))
        {
            if (component.Open)
            {
                inlet.AddAlwaysReachable(outlet);
                outlet.AddAlwaysReachable(inlet);
                _ambientSoundSystem.SetAmbience(uid, true);

                var pressure = inlet.Air.Pressure;
                var volume = MathF.Asinh(pressure);

                _ambientSoundSystem.SetVolume(uid, volume);
            }
            else
            {
                inlet.RemoveAlwaysReachable(outlet);
                outlet.RemoveAlwaysReachable(inlet);
                _ambientSoundSystem.SetAmbience(uid, false);
            }
        }
    }
}
