namespace SpellingCorrector.Algorithms;

public static class EditDistanceAlgorithms
{
    public static int DamerauLevenshteinDistance(string source, string target, int threshold)
    {
        var length1 = source.Length;
        var length2 = target.Length;

        if (Math.Abs(length1 - length2) > threshold) { return int.MaxValue; }

        if (length1 > length2)
        {
            (target, source) = (source, target);
        }

        var dCurrent = new int[length1 + 1];
        var dMinus1 = new int[length1 + 1];
        var dMinus2 = new int[length1 + 1];

        for (var i = 0; i <= length1; i++)
        {
            dCurrent[i] = i;
        }

        for (var i = 1; i <= length2; i++)
        {
            var dSwap = dMinus2;
            dMinus2 = dMinus1;
            dMinus1 = dCurrent;
            dCurrent = dSwap;

            var minDistance = int.MaxValue;
            dCurrent[0] = i;
            
            var iMinus1 = 0;
            var iMinus2 = -1;

            for (var j = 1; j <= length1; j++)
            {
                var cost = source[iMinus1] == target[i - 1] ? 0 : 1;

                var del = dCurrent[iMinus1] + 1;
                var ins = dMinus1[j] + 1;
                var sub = dMinus1[iMinus1] + cost;

                var min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

                if (i > 1 && j > 1 && source[iMinus2] == target[i - 1] && source[iMinus1] == target[i - 2])
                    min = Math.Min(min, dMinus2[iMinus2] + cost);

                dCurrent[j] = min;
                if (min < minDistance) { minDistance = min; }
                iMinus1++;
                iMinus2++;
            }

            if (minDistance > threshold) { return int.MaxValue; }
        }

        var result = dCurrent[length1];
        return (result > threshold) ? int.MaxValue : result;
    }
}