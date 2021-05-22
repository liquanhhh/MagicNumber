
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


class Context
{
    Queue<long> seedValues = new Queue<long>();

    PriorityQueue<SeedWorker> pq = new PriorityQueue<SeedWorker>();

    long currentValue = 0;
    public Context()
    {
        pq.Enqueue(new SeedWorker(1, this));
        pq.Enqueue(new SeedWorker(2, this));
        pq.Enqueue(new SeedWorker(3, this));
    }

    public long getNextValue()
    {
        if (seedValues.Count > 0)
            return seedValues.Dequeue();
        else
            return 0;
    }

    public long match(int n)
    {
        if (n < 3) return n;

        int index = 1;
        long result = 0;
        long maxValueInQueue = 0;

        while (index < n)
        {
            SeedWorker worker = pq.Dequeue();

            long value = worker.currentValue;

            if (value > currentValue)
            {
                if (seedValues.Count > 0 && seedValues.Peek() * 2 < value)
                {
                    var peek = seedValues.Dequeue();
                    pq.Enqueue(new SeedWorker(peek, this));
                    pq.Enqueue(worker);
                    continue;
                }

                index++;
                Console.WriteLine("{0}, {1}, {2}, {3}", index, value, pq.Count(), seedValues.Count);
                seedValues.Enqueue(value);
                currentValue = value;
            }

            worker.markUsed(value);
            maxValueInQueue = value;

            if (worker.currentValue > 0)
            {
                pq.Enqueue(worker);
            }
        }

        return result;
    }
}

class SeedWorker : IComparable<SeedWorker>
{
    long[] NUMBER = new long[] { 2, 3, 5 };
    long value;
    Context context;

    int seedIndex = 0;

    public SeedWorker(long v, Context context)
    {
        this.value = v;
        this.context = context;
    }

    public long currentValue
    {
        get
        {
            return this.value * NUMBER[seedIndex];
        }
    }

    public void markUsed(long value)
    {
        if (this.currentValue == value)
        {
            this.seedIndex++;

            if (this.seedIndex >= 3)
            {
                seedIndex = 0;
                this.value = context.getNextValue();
            }
        }
    }

    int IComparable<SeedWorker>.CompareTo(SeedWorker worker)
    {
        return this.currentValue < worker.currentValue ? -1 : 1;
    }

}


