using NUnit.Framework;
using GemBlast.Core;

namespace GemBlast.Tests
{

    [TestFixture]
    public class RuleEngineTests
    {


        [Test]
        public void GetIconType_CountLessThanA_ReturnsDefault()
        {
            var ruleEngine = new RuleEngine(4, 7, 9);

            Assert.AreEqual(IconType.Default, ruleEngine.GetIconTypeForGroupSize(1));
            Assert.AreEqual(IconType.Default, ruleEngine.GetIconTypeForGroupSize(2));
            Assert.AreEqual(IconType.Default, ruleEngine.GetIconTypeForGroupSize(3));
        }

        [Test]
        public void GetIconType_CountEqualsA_ReturnsIconA()
        {
            var ruleEngine = new RuleEngine(4, 7, 9);


            Assert.AreEqual(IconType.IconA, ruleEngine.GetIconTypeForGroupSize(4));
        }

        [Test]
        public void GetIconType_CountBetweenAAndB_ReturnsIconA()
        {
            var ruleEngine = new RuleEngine(4, 7, 9);

            Assert.AreEqual(IconType.IconA, ruleEngine.GetIconTypeForGroupSize(5));
            Assert.AreEqual(IconType.IconA, ruleEngine.GetIconTypeForGroupSize(6));
        }

        [Test]
        public void GetIconType_CountEqualsB_ReturnsIconB()
        {
            var ruleEngine = new RuleEngine(4, 7, 9);


            Assert.AreEqual(IconType.IconB, ruleEngine.GetIconTypeForGroupSize(7));
        }

        [Test]
        public void GetIconType_CountBetweenBAndC_ReturnsIconB()
        {
            var ruleEngine = new RuleEngine(4, 7, 9);

            Assert.AreEqual(IconType.IconB, ruleEngine.GetIconTypeForGroupSize(8));
        }

        [Test]
        public void GetIconType_CountEqualsC_ReturnsIconC()
        {
            var ruleEngine = new RuleEngine(4, 7, 9);


            Assert.AreEqual(IconType.IconC, ruleEngine.GetIconTypeForGroupSize(9));
        }

        [Test]
        public void GetIconType_CountGreaterThanC_ReturnsIconC()
        {
            var ruleEngine = new RuleEngine(4, 7, 9);

            Assert.AreEqual(IconType.IconC, ruleEngine.GetIconTypeForGroupSize(10));
            Assert.AreEqual(IconType.IconC, ruleEngine.GetIconTypeForGroupSize(20));
            Assert.AreEqual(IconType.IconC, ruleEngine.GetIconTypeForGroupSize(100));
        }

        [Test]
        public void GetIconType_DifferentThresholds_WorksCorrectly()
        {

            var ruleEngine = new RuleEngine(4, 6, 8);

            Assert.AreEqual(IconType.Default, ruleEngine.GetIconTypeForGroupSize(3));
            Assert.AreEqual(IconType.IconA, ruleEngine.GetIconTypeForGroupSize(4));
            Assert.AreEqual(IconType.IconA, ruleEngine.GetIconTypeForGroupSize(5));
            Assert.AreEqual(IconType.IconB, ruleEngine.GetIconTypeForGroupSize(6));
            Assert.AreEqual(IconType.IconB, ruleEngine.GetIconTypeForGroupSize(7));
            Assert.AreEqual(IconType.IconC, ruleEngine.GetIconTypeForGroupSize(8));
            Assert.AreEqual(IconType.IconC, ruleEngine.GetIconTypeForGroupSize(9));
        }

        [Test]
        public void GetIconType_MinimumThresholds_EdgeCase()
        {

            var ruleEngine = new RuleEngine(2, 3, 4);

            Assert.AreEqual(IconType.Default, ruleEngine.GetIconTypeForGroupSize(1));
            Assert.AreEqual(IconType.IconA, ruleEngine.GetIconTypeForGroupSize(2));
            Assert.AreEqual(IconType.IconB, ruleEngine.GetIconTypeForGroupSize(3));
            Assert.AreEqual(IconType.IconC, ruleEngine.GetIconTypeForGroupSize(4));
        }
    }
}
