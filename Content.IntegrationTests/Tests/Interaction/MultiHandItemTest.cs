using System.Threading.Tasks;
using Content.Shared.Hands.Components;
using Content.Shared.Item;
using NUnit.Framework;
using Robust.Shared.GameObjects;
using Robust.UnitTesting.Pool;

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

    [Test]
    [TestOf(typeof(MultiHandedItemComponent))]
    [Description(
        "Ensures that multi-handed items properly block hands from picking up other items, and that they require enough hands to pick up.")]
    public async Task BlockedHandsTest()
    {
        await Server.WaitAssertion(() =>
        {
            var item1 = SEntMan.SpawnEntity(TestMultiHandItemPrototypeId, SEntMan.GetCoordinates(TargetCoords));

            Assert.That(!HandSys.TryPickupAnyHand(SPlayer, item1),
                "Picked up a multi-handed item while having only one hand.");
        });

        await Server.WaitAssertion(() =>
        {
            HandSys.AddHand(SPlayer, HandLeft, HandLocation.Left);
            var item2 = SEntMan.SpawnEntity(TestMultiHandItemPrototypeId, SEntMan.GetCoordinates(TargetCoords));

            Assert.Multiple(() =>
            {
                Assert.That(HandSys.TryPickup(SPlayer, item2, HandLeft),
                    "Could not pick up a two-handed item with two free hands.");

                Assert.That(HandSys.CountFreeHands(SPlayer),
                    Is.Zero,
                    "Free hands remained after using all hands to pick up a multi-handed item.");
            });
        });

        await Server.WaitAssertion(() =>
        {
            var item3 = SEntMan.SpawnEntity(TestItemPrototypeId, SEntMan.GetCoordinates(TargetCoords));

            Assert.That(!HandSys.TryPickup(SPlayer, item3, HandRight),
                "Picked up an item with a hand blocked by a multi-handed item.");
        });
    }
}
