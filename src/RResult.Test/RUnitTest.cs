namespace RResult.Test;

[TestClass]
public class RUnitTest
{
    [TestMethod]
    public void TestRUnitEquality()
    {
        {
            RUnit unit = new();
            var unit2 = new RUnit();
            RUnit unit3 = default;
            Assert.AreEqual(unit, default);
            Assert.AreEqual(unit.GetHashCode(), 0);
            Assert.AreEqual(unit.ToString(), "()");
            Assert.AreEqual(unit, unit2);
            Assert.AreEqual(unit2, unit3);
            Assert.AreEqual(unit3, new RUnit());
        }
    }
}
