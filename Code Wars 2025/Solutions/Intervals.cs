namespace Code_Wars_2025.Solutions
{
    public class Intervals
    {
        public static int SumIntervals((int, int)[] intervals)
        {
            if (intervals.Length == 0)
            {
                return 0;
            }

            var orderedIntervals = intervals.OrderBy(interval => interval.Item1).ToList();

            List<(int, int)> mergedIntervals = new();
            int tempStart = orderedIntervals[0].Item1;
            int tempEnd = orderedIntervals[0].Item2;

            for (int i = 1; i < orderedIntervals.Count; i++)
            {
                if (orderedIntervals[i].Item1 <= tempEnd)
                {
                    tempEnd = Math.Max(tempEnd, orderedIntervals[i].Item2);
                }
                else
                {
                    mergedIntervals.Add((tempStart, tempEnd));
                    tempStart = orderedIntervals[i].Item1;
                    tempEnd = orderedIntervals[i].Item2;
                }
            }

            mergedIntervals.Add((tempStart, tempEnd));

            int sum = 0;
            foreach ((int, int) interval in mergedIntervals)
            {
                sum += interval.Item2 - interval.Item1;
            }
            return sum;
        }
    }
}