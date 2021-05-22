
using System;
class MagicNumberTest {
    public static void Test() {
        var n1 = new MagicNumber(1);
        var n2 = new MagicNumber(2);
        var v3 = new MagicNumber(3) * 5;

        Console.WriteLine("n1 > n2, {0}  --> false", n1 > n2);
        Console.WriteLine("n1 < n2, {0}  --> true", n1 < n2);
        Console.WriteLine("n1 >= n2, {0}  --> false", n1 >= n2);
        Console.WriteLine("n1 <= n2, {0}  --> true", n1 <= n2);

        Console.WriteLine("15 == {0}", v3.ToString());
    }
}


