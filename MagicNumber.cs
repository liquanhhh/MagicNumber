
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


// Use this as the number
class MagicNumber : IComparable<MagicNumber>
{
    public List<long> data = new List<long>();
    static readonly long STEP = 100000000000000000;
    static readonly int LENGTH = 17;
    public MagicNumber clone()
    {
        var number = new MagicNumber();
        this.data.ForEach((item) => number.data.Add(item));

        return number;
    }

    public MagicNumber()
    {

    }
    public MagicNumber(long value)
    {
        this.data.Add(value);
    }

    public static MagicNumber operator *(MagicNumber magic, int number)
    {
        var number1 = new MagicNumber();

        var length = magic.data.Count;
        long carry = 0;
        for (var i = 0; i < length; i++)
        {
            var c = magic.data[i] * number + carry;
            number1.data.Add(c % STEP);
            carry = c / STEP;
        }

        if (carry > 0)
        {
            number1.data.Add(carry);
        }

        return number1;
    }

    public bool isZero()
    {
        return data.Count == 0 || (data.Count == 1 && data[0] == 0);
    }

    public static bool operator <(MagicNumber a, MagicNumber b)
    {
        var compare = a.Compare(b);
        return compare <= -1;
    }


    public static bool operator >(MagicNumber a, MagicNumber b)
    {
        var compare = a.Compare(b);
        return compare >= 1;
    }

    public static bool operator <=(MagicNumber a, MagicNumber b)
    {
        var compare = a.Compare(b);
        return compare < 1;
    }

    public static bool operator >=(MagicNumber a, MagicNumber b)
    {
        var compare = a.Compare(b);
        return compare > -1;
    }


    public override String ToString()
    {
        StringBuilder sb = new StringBuilder();
        List<String> tmp = new List<String>();
        var carry = 0;
        for (var i = 0; i < this.data.Count; i++)
        {
            long c = (long)this.data[i] + carry;

            var seg = c.ToString();
            var prefix = LENGTH - seg.Length;
            while (prefix-- > 0)
            {
                seg = "0" + seg;
            }

            tmp.Add(seg);
            carry = (int)(c / STEP);
        }

        tmp.Reverse();
        tmp.ForEach((item) => sb.Append(item));

        return sb.ToString().TrimStart('0');
    }

    int IComparable<MagicNumber>.CompareTo(MagicNumber number)
    {
        return this.Compare(number);
    }

    public int Compare(MagicNumber number)
    {
        if (number == null) return 0;

        if (this.data.Count == number.data.Count)
        {
            for (var i = this.data.Count - 1; i >= 0; i--)
            {
                if (this.data[i] == number.data[i])
                {

                }
                else
                {
                    return this.data[i] < number.data[i] ? -1 : 1;
                }
            }
            return 0;
        }
        else
        {
            return this.data.Count - number.data.Count;
        }
    }
}




class MagicContext
{

    List<MagicWaitingList> waitingList = new List<MagicWaitingList>(3);

    MagicNumber currentValue = new MagicNumber(1);
    public MagicContext()
    {
        this.waitingList.Add(new MagicWaitingList(0, this));
        this.waitingList.Add(new MagicWaitingList(1, this));
        this.waitingList.Add(new MagicWaitingList(2, this));


        this.addWorker(new MagicSeedWorker(new MagicNumber(1), this));
    }


    public MagicNumber Find(int n)
    {
        int printStep = n/10;
        if (n < 3) return new MagicNumber(n);

        int index = 1;
        MagicNumber result = null;
        int printCounter = 1;

        while (index < n)
        {
            var value1 = this.waitingList[0].value;
            var value2 = this.waitingList[1].value;
            var value3 = this.waitingList[2].value;

            var minValue = value1 <= value2 ? value1 : value2;
            minValue = minValue <= value3 ? minValue : value3;

            this.waitingList[0].markUsed(minValue);
            this.waitingList[1].markUsed(minValue);
            this.waitingList[2].markUsed(minValue);

            this.addWorker(new MagicSeedWorker(minValue, this));
            index++;
            result = minValue;

            if(printCounter++ == printStep) {
                printCounter = 0;
                Console.WriteLine("{0} {1} {2} {3} {4}",index, minValue, this.waitingList[0].Count, this.waitingList[1].Count, this.waitingList[2].Count);
            }
        }

        return result;
    }

    public void addWorker(MagicSeedWorker worker)
    {
        if (worker.index < 3 && worker.index >= 0)
        {
            this.waitingList[worker.index].AddLast(worker);
        }
    }

}

class MagicSeedWorker : IComparable<MagicSeedWorker>
{
    static int[] NUMBER = new int[] { 2, 3, 5 };
    public MagicNumber seedValue;
    MagicContext context;

    int seedIndex = 0;

    public MagicSeedWorker(MagicNumber v, MagicContext context)
    {
        this.seedValue = v;
        this.context = context;
        this.currentValue = this.seedValue * NUMBER[this.seedIndex];
    }

    public MagicNumber currentValue;

    public void markUsed(MagicNumber value)
    {
        // will mark the value as used if the value is smaller then the target
        if (this.currentValue <= value)
        {
            // increase the index
            this.seedIndex++;

            if (this.seedIndex < 3)
            {
                this.currentValue = this.seedValue * NUMBER[this.seedIndex];
            }
        }
    }

    int IComparable<MagicSeedWorker>.CompareTo(MagicSeedWorker worker)
    {
        return this.currentValue < worker.currentValue ? -1 : 1;
    }

    public int index
    {
        get
        {
            return seedIndex;
        }
    }

}


class MagicWaitingList : LinkedList<MagicSeedWorker>
{

    int channel;
    MagicContext context;

    public MagicWaitingList(int channel, MagicContext context)
    {
        this.channel = channel;
        this.context = context;
    }

    public MagicNumber value
    {
        get
        {
            if (this.Count > 0)
            {
                return this.First.Value.currentValue;
            }
            else
            {
                return null;
            }
        }
    }

    public void markUsed(MagicNumber lastValue)
    {
        if (this.Count > 0)
        {
            var worker = this.First.Value;

            worker.markUsed(lastValue);

            if (worker.index != this.channel)
            {
                this.RemoveFirst();

                if (worker.index < 3)
                {
                    this.context.addWorker(worker);
                }
            }
        }
    }
}


