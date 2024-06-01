using System.Security.Cryptography;

namespace RResult.Test;

[TestClass]
public class RResultTest
{
    [TestMethod]
    public void TestRResult()
    {
        var resultOk = RResult<int, string>.Ok(1);
        Assert.AreEqual(resultOk, 1);
        var resultErr = RResult<int, string>.Err("error!");
        Assert.AreEqual(resultErr, "error!");
    }

    public static RResult<string, string> GetBothTypeStr(bool success) => success switch
    {
        // This needs type declarationbecause it cannot use implicit constructor when same types.
        true => RResult<string, string>.Ok("hoge"),
        _ => RResult<string, string>.Err("fail"),
    };

    [TestMethod]
    public void TestRResultBothSameTypes()
    {
        // This needs Unwrap because it cannot use implicit constructor when same name.
        var actual = GetBothTypeStr(true).Unwrap;
        Assert.AreEqual(actual, "hoge");
        var actual2 = GetBothTypeStr(false).UnwrapErr;
        Assert.AreEqual(actual2, "fail");
    }

    // Can use implicit constructor when deffenet types.
    // Can use Exception for Error type
    public static RResult<int, Exception> GetEx(bool success) => success switch
    {
        true => 1,
        _ => new Exception("fail"),
    };

    [TestMethod]
    public void TestRResultException()
    {
        var actual = GetEx(true).Unwrap;
        Assert.AreEqual(actual, 1);
        var actual2 = GetEx(false).UnwrapErr;
        Assert.AreEqual(actual2?.Message, "fail");
    }

    // Shorthands
    public static RResult<int, string> Ok(int v) => RResult<int, string>.Ok(v);
    public static RResult<string, string> Ok(string v) => RResult<string, string>.Ok(v);
    public static RResult<int, string> Err(string e) => RResult<int, string>.Err(e);

    [TestMethod]
    public void TestRResultAnd()
    {
        var actual = Ok(1)
                     .And(Ok(2));
        Assert.AreEqual(actual, 2);

        var actual2 = Ok(1)
                      .And(Err("failed"));
        Assert.AreEqual(actual2, "failed");

        var actual3 = Err("failed")
                      .And(Ok(1));
        Assert.AreEqual(actual3, "failed");
    }

    [TestMethod]
    public void TestRResultMatch()
    {
        // Happy path
        var actual_ok = Ok(1)
                        .Match(
                            v => Ok(v + 10), // 11
                            RResult<int, string>.Err
                        )
                        .Match(
                            v => Ok($"{v}"), // "11"
                            RResult<string, string>.Err
                        );
        Assert.AreEqual(actual_ok, Ok("11"));

        // Can convert Err message
        var actual_err = Err("failed at 1")
                         .Match(
                            Ok,
                            RResult<int, string>.Err
                         )
                         .Match(
                            Ok,
                            _ => RResult<int, string>.Err("failed at 3")
                         );
        Assert.AreEqual(actual_err, "failed at 3");


        var actual_err2 = Ok(1) // Ok(1)
                         .Match(
                            _ => Err("failed at 2"),
                            RResult<int, string>.Err
                         )
                         .Match(
                            Ok,
                            RResult<int, string>.Err
                         );
        Assert.AreEqual(actual_err2, "failed at 2");
    }

    public static void TestRResultAndThen()
    {
        // Happy path
        var actual_ok = Ok(1) // 1
                        .AndThen(v => Ok(v + 10)) // 11
                        .AndThen(v => Ok($"{v}")); // "11"
        Assert.AreEqual(actual_ok, Ok("11"));

        // UnHappy path
        //  - Use Exception type error
        //  - Forces first error
        var actual_err = RResult<int, Exception>.Err(new Exception("faild at 1"))
                         .AndThen(RResult<int, Exception>.Ok) // pass!
                         .AndThen(_ => RResult<int, Exception>.Err(new Exception("failed at 2"))); // pass!
        Assert.AreEqual(actual_err.UnwrapErr!.Message, "failed at 1");

        // UnHappy path
        //  - Use string type error
        var actual_err2 = Err("faild at 1")
                         .AndThen(Ok) // pass!
                         .AndThen(_ => RResult<string, string>.Err("failed at 2")); // pass!
        Assert.AreEqual(actual_err.UnwrapErr!.Message, "failed at 1");

        // UnHappy path
        //  - Thru when error occured
        var actual_err3 = Ok(1) // Ok(1)
                         .AndThen(_ => Err("failed at 2"))
                         .AndThen(Ok); // pass!
        Assert.AreEqual(actual_err3, "failed at 2");
    }

    [TestMethod]
    public void TestRResultAndThenBy()
    {
        // Can only use return type at first one.
        var actual = Ok(300) // Ok(300)
                     .AndThenBy(n => n + 10)// Ok(310))
                     .AndThenBy(a => a + 1); // Ok(311))
        Assert.AreEqual(actual, 311);

        var actual2 = Ok(300) // Ok(300)
                     .AndThenBy(_ => Err("error")) // Err(error))
                     .AndThenBy(n => n + 10); // pass
        Assert.AreEqual(actual2, "error");

        var actual3 = Ok(300) // Ok(300)
                     .AndThenBy(_ => Err("error")) // Err(error))
                     .Or(Ok(1)); // effective.
        Assert.AreEqual(actual3, 1);
    }

    [TestMethod]
    public void TestRResultOr()
    {
        var actual = Ok(1) // 1
                     .Or(Ok(2)) // pass
                     .Or(Err("failed"));  // pass
        Assert.AreEqual(actual, 1);

        var actual2 = Err("failed")
                      .Or(Ok(3))
                      .Or(Ok(4));  // pass
        Assert.AreEqual(actual2, 3);

        var actual3 = Err("failed")
                      .Or(Err("failed2"));
        Assert.AreEqual(actual3, "failed2");
    }
}