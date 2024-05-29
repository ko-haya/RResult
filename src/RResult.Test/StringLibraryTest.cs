namespace RResult.Test;

[TestClass]
public class StringLibraryTest
{
    [TestMethod]
    public void TestStartsWithUppder()
    {
        string[] words = { "Alphabet", "Zebra", "ABC", "Αθήνα", "Москва" };
        foreach (var w in words)
            Assert.IsTrue(StringLibrary.StartsWithUpper(w));
    }

    [TestMethod]
    public void TestDoesNotStartWithUpper()
    {
        string[] words = { "alphabet", "zebra", "abc", "αυτοκινητοβιομηχανία", "государство",
                               "1234", ".", ";", " " };
        foreach (var w in words)
            Assert.IsFalse(StringLibrary.StartsWithUpper(w));
    }

    [TestMethod]
    public void DirectCallWithNullOrEmpty()
    {
        string?[] words = { string.Empty, null };
        foreach (var w in words)
            Assert.IsFalse(StringLibrary.StartsWithUpper(w));
    }
}
