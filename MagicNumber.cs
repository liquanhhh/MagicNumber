
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


// Use this as the number
class MagicNumber : IComparable<MagicNumber>
{
    List<int> data = new List<int>();
    public MagicNumber clone()
    {
        var number = new MagicNumber();
        this.data.ForEach((item) => number.data.Add(item));

        return number;
    }

    public MagicNumber()
    {

    }
    public MagicNumber(int value)
    {
        this.data.Add(value);
    }

    public static MagicNumber operator *(MagicNumber magic, int number)
    {
        var number1 = new MagicNumber();

        var length = magic.data.Count;
        var carry = 0;
        for (var i = 0; i < length; i++)
        {
            var c = magic.data[i] * number + carry;
            number1.data.Add(c % 10);
            carry = c / 10;
        }

        if(carry > 0) {
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
        for (var i = this.data.Count - 1; i >= 0; i--)
        {
            sb.Append(this.data[i]);
        }
        return sb.ToString();
    }

    int IComparable<MagicNumber>.CompareTo(MagicNumber number)
    {
        return this.Compare(number);
    }

    public int Compare(MagicNumber number)
    {
        if(number == null) return 0;

        if (this.data.Count == number.data.Count)
        {
            for (var i = this.data.Count - 1; i >= 0; i--)
            {
                if (this.data[i] == number.data[i])
                {

                }
                else
                {
                    return this.data[i] - number.data[i];
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
                if(index % 1000000 == 0) {
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
            } else {
                this.currentValue = this.value * NUMBER[this.seedIndex];
            }
        }
    }

    int IComparable<MagicSeedWorker>.CompareTo(MagicSeedWorker worker)
    {
        return this.currentValue < worker.currentValue ? -1 : 1;
    }

}


