using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests.Interaction;

[TestFixture]
public sealed class MultiHandItemTest : InteractionTest
{
    private const string HandLeft = "hand_left";
    private const string HandRight = "hand_right";

    private const string TestItemPrototypeId = "MultiHandItemTestItem";
    private const string TestMultiHandItemPrototypeId = "MultiHandItemTestMultiHandItem";

    [TestPrototypes]
    private const string TestItemPrototype = $"""
        - type: entity
          id: {TestItemPrototypeId}
          parent: BaseItem
          components:
          - type: Item
        """;

    [TestPrototypes]
    private const string TestMultiHandItemPrototype = $"""
        - type: entity
          id: {TestMultiHandItemPrototypeId}
          parent: BaseItem
          components:
          - type: Item
          - type: MultiHandedItem
        """;

    private EntityUid SpawnTestItem(string prototypeId)
    {
        var item = SEntMan.SpawnEntity(prototypeId,
            SEntMan.GetCoordinates(PlayerCoords));
        return item;
    }

    [Test]
    public async Task BlockedHandsTest()
    {
        await Server.WaitAssertion(() =>
        {
            var item1 = SpawnTestItem(TestMultiHandItemPrototypeId);
            Assert.That(!HandSys.TryPickupAnyHand(SPlayer, item1),
                "Picked up a multi-handed item while having only one hand.");

            HandSys.AddHand(SPlayer, HandLeft, HandLocation.Left);
            var item2 = SpawnTestItem(TestMultiHandItemPrototypeId);
            Assert.That(HandSys.TryPickup(SPlayer, item2, HandLeft),
                "Could not pick up a two-handed item with two free hands.");

            Assert.That(HandSys.CountFreeHands(SPlayer),
                Is.Zero,
                "Free hands remained after using all hands to pick up a multi-handed item.");

            var item3 = SpawnTestItem(TestItemPrototypeId);
            Assert.That(!HandSys.TryPickup(SPlayer, item3, HandRight),
                "Picked up an item with a hand blocked by a multi-handed item.");
        });
    }
}
