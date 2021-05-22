
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


// Use this as the number
class MagicNumber : IComparable<MagicNumber>
{
    List<long> data = new List<long>();
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

    public static MagicNumber operator +(MagicNumber a, MagicNumber b)
    {
        if (a.data.Count < b.data.Count)
        {
            var tmp = a;
            a = b;
            b = a;
        }
        var result = a.clone();

        for (var i = 0; i < a.data.Count; i++)
        {

        }

        return result;
    }

    public static MagicNumber operator *(MagicNumber a, MagicNumber b)
    {
        return a;
    }

    public override String ToString()
    {
        StringBuilder sb = new StringBuilder();
        List<String> tmp = new List<String>();
        var carry = 0;
        for (var i = 0; i < this.data.Count; i++)
        {
            long c = (long)this.data[i] + carry;

            // var tmp = c % STEP;
            // while (tmp > 0)
            // {
            //     sb.Append(tmp % 10);
            //     tmp = tmp / 10;
            // }

            var seg = c.ToString();
            var prefix = LENGTH - seg.Length;
            while(prefix-- > 0) {
                seg = "0" + seg;
            }

            tmp.Add(seg);
            carry = (int)(c / STEP);
        }

        tmp.Reverse();
        tmp.ForEach((item) => sb.Append(item));

        return sb.ToString();
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
    Queue<MagicNumber> seedValues = new Queue<MagicNumber>();

    // Use priority Queue to manage next posibile value
    PriorityQueue<MagicSeedWorker> pq = new PriorityQueue<MagicSeedWorker>();

    MagicNumber currentValue = new MagicNumber(1);
    public MagicContext()
    {
        pq.Enqueue(new MagicSeedWorker(new MagicNumber(1), this));
        pq.Enqueue(new MagicSeedWorker(new MagicNumber(2), this));
        pq.Enqueue(new MagicSeedWorker(new MagicNumber(3), this));
    }

    public MagicNumber getNextValue()
    {
        if (seedValues.Count > 0)
            return seedValues.Dequeue();
        else
            return new MagicNumber(0);
    }

    public MagicNumber match(int n)
    {
        int printStep = n / 10;
        if (n < 3) return new MagicNumber(n);

        int index = 1;
        MagicNumber result = null;
        MagicNumber maxValueInQueue;

        while (index < n)
        {
            MagicSeedWorker worker = pq.Dequeue();

            MagicNumber value = worker.currentValue;

            if (value > currentValue)
            {
                // check current max value is bigger than seedValues * 2, put that seed in the pq
                if (seedValues.Count > 0 && seedValues.Peek() * 2 < value)
                {
                    var peek = seedValues.Dequeue();
                    pq.Enqueue(new MagicSeedWorker(peek, this));
                    pq.Enqueue(worker);
                    continue;
                }

                index++;
                if (index % printStep == 0)
                {
                    Console.WriteLine("{0}, {1}, {2}, {3}", index, value, pq.Count(), seedValues.Count);
                }
                seedValues.Enqueue(value);
                currentValue = value;
            }

            worker.markUsed(value);
            maxValueInQueue = value;
            result = value;

            if (!worker.currentValue.isZero())
            {
                pq.Enqueue(worker);
            }
        }

        return result;
    }
}

class MagicSeedWorker : IComparable<MagicSeedWorker>
{
    static int[] NUMBER = new int[] { 2, 3, 5 };
    public MagicNumber value;
    MagicContext context;

    int seedIndex = 0;

    public MagicSeedWorker(MagicNumber v, MagicContext context)
    {
        this.value = v;
        this.context = context;
        this.currentValue = this.value * NUMBER[this.seedIndex];
    }

    public MagicNumber currentValue;

    public void markUsed(MagicNumber value)
    {
        // will mark the value as used if the value is smaller then the target
        if (this.currentValue <= value)
        {
            // increase the index
            this.seedIndex++;

            if (this.seedIndex >= 3)
            {
                seedIndex = 0;
                this.value = context.getNextValue();
                this.currentValue = this.value * NUMBER[this.seedIndex];
            }
            else
            {
                this.currentValue = this.value * NUMBER[this.seedIndex];
            }
        }
    }

    int IComparable<MagicSeedWorker>.CompareTo(MagicSeedWorker worker)
    {
        return this.currentValue < worker.currentValue ? -1 : 1;
    }

}


