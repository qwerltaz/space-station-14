using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server;

[AnyCommand]
public sealed class SpawnMany : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    public string Command => "spawnmany";
    public string Description => "spawn many entities of given prototype on the user";
    public string Help => $"{Command} <prototype> <count>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length is > 2 or < 1)
            shell.WriteLine("Usage: spawnmany <prototype>");

        if (shell.Player?.AttachedEntity is null)
            shell.WriteLine("No entity found attached to you.");

        if (!_protoManager.Resolve(args[0], out var proto))
        {
            shell.WriteLine($"No prototype found with name {args[0]}");
            return;
        }

        var count = 1;
        if (args.Length == 2 && !int.TryParse(args[1], out count))
        {
            shell.WriteLine($"Invalid entity count: {args[1]}");
            return;
        }

        var uid = shell.Player!.AttachedEntity!.Value;
        var transformSystem = _entManager.System<SharedTransformSystem>();
        var pos = transformSystem.GetMapCoordinates(uid);

        for (var i = 0; i < count; i++)
        {
            _entManager.Spawn(proto.ID, pos);
        }
    }
}
