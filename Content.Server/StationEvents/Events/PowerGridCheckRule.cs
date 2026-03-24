using System.Threading;
using Content.Server.Power.Components;
using Content.Server.StationEvents.Components;
using Content.Shared.GameTicking.Components;
using JetBrains.Annotations;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Server.StationEvents.Events
{
    [UsedImplicitly]
    public sealed class PowerGridCheckRule : StationEventSystem<PowerGridCheckRuleComponent>
    {
        protected override void Started(EntityUid uid,
            PowerGridCheckRuleComponent component,
            GameRuleComponent gameRule,
            GameRuleStartedEvent args)
        {
            base.Started(uid, component, gameRule, args);

            if (!TryGetRandomStation(out var chosenStation))
                return;

            component.AffectedStation = chosenStation.Value;

            var query = AllEntityQuery<PowerNetworkBatteryComponent, PowerMonitoringDeviceComponent>();
            while (query.MoveNext(out var substationUid, out _, out var powerMonitoringDevice))
            {
                if (powerMonitoringDevice.Group != component.TargetDeviceGroup)
                {
                    continue;
                }

                component.Powered.Add(substationUid);
            }

            RobustRandom.Shuffle(component.Powered);

            component.NumberPerSecond =
                Math.Max(1,
                    (int)(component.Powered.Count /
                          component.SecondsUntilOff)); // Number of targets to turn off every second. At least one.
        }

        protected override void Ended(EntityUid uid, PowerGridCheckRuleComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
        {
            base.Ended(uid, component, gameRule, args);

            foreach (var entity in component.Unpowered)
            {
                if (Deleted(entity))
                    continue;

                if (TryComp(entity, out PowerNetworkBatteryComponent? powerNetworkBattery))
                {
                    powerNetworkBattery.CanDischarge = true;
                }
            }

            // Can't use the default EndAudio
            component.AnnounceCancelToken?.Cancel();
            component.AnnounceCancelToken = new CancellationTokenSource();
            Timer.Spawn(3000, () =>
            {
                Audio.PlayGlobal(component.PowerOnSound, Filter.Broadcast(), true);
            }, component.AnnounceCancelToken.Token);
            component.Unpowered.Clear();
        }

        protected override void ActiveTick(EntityUid uid, PowerGridCheckRuleComponent component, GameRuleComponent gameRule, float frameTime)
        {
            base.ActiveTick(uid, component, gameRule, frameTime);

            var updates = 0;
            component.FrameTimeAccumulator += frameTime;
            if (component.FrameTimeAccumulator > component.UpdateRate)
            {
                updates = (int) (component.FrameTimeAccumulator / component.UpdateRate);
                component.FrameTimeAccumulator -= component.UpdateRate * updates;
            }

            for (var i = 0; i < updates; i++)
            {
                if (component.Powered.Count == 0)
                    break;

                var selected = component.Powered.Pop();
                if (Deleted(selected))
                    continue;
                if (TryComp<PowerNetworkBatteryComponent>(selected, out var powerNetworkBattery))
                {
                    powerNetworkBattery.CanDischarge = false;
                }

                component.Unpowered.Add(selected);
            }
        }
    }
}
