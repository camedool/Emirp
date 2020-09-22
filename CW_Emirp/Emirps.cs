using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

public class Emirps
{
    private static SortedSet<int> _primes = new SortedSet<int>();

    /// <summary>
    /// Find the emirps for the given limit.
    /// </summary>
    /// <param name="n">Limit number</param>
    /// <returns>Array of long with number of emirps, max emirp and sum of all emirps below the given limit</returns>
    public static long[] FindEmirp(long n)
    {
        var primes = GetPrimeNumbers(n);
        var emirps = new ConcurrentBag<long>();

        Parallel.ForEach(primes, (prime) =>
        {
            if (prime > n)
                return;

            var reverse = GetReverse(prime);
            if (prime != reverse && _primes.Contains((int)reverse))
                emirps.Add(prime);
        });

        return new long[] { emirps.Count(), emirps.Max(), emirps.Sum() };
    }

    /// <summary>
    /// Update the primeCache to full digit (e.g. limit is 47 -> update to exponent of 2 -> 100)
    /// </summary>
    /// <param name="limit">number limit to update</param>
    private static void UpdatePrimesCache(long limit)
    {
        var exponentValue = limit.ToString().Length;
        // get the all numbers within the same number of digits (e.g. 50 -> 100, 450 -> 1000)
        limit = (long)Math.Pow(10, exponentValue);
        var primeLimit = Math.Sqrt(limit);

        if (_primes.Count == 0)
        {
            // initial 200 primes
            var intArray = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199 };
            _primes.UnionWith(intArray);
        }

        var maxCurrentNumber = _primes.Max;
        // if limit is less then our maxNumber in cache, finish
        if (maxCurrentNumber > limit)
            return;

        var leftSide = new ConcurrentBag<int>();
        var rightSide = new ConcurrentBag<int>();

        // get the range of numbers to evaluate a new primes
        var range = Enumerable.Range(maxCurrentNumber, (int)(limit - maxCurrentNumber) + 1)
        .AsParallel()
        .Where(n => !_primes.Any(a => n % a == 0));

        // split range set into 2: leftSide (all numbers which are less than Math.sqrt(limit) and right side all other numbers in the range)
        foreach (var num in range)
        {
            if (num <= primeLimit)
                leftSide.Add(num);
            else
                rightSide.Add(num);
        }

        // remove the not prime numbers from right side, based on the number from the left side
        var cleanedRightSide = rightSide.Where(a => leftSide.All(b => a % b != 0)).AsParallel();

        // add to cache
        _primes.UnionWith(leftSide);
        _primes.UnionWith(cleanedRightSide);
    }


    /// <summary>
    /// Gets the prime numbers to evaluate the emirps (removes the numbers starting with even or 5, they are not emirps by default)
    /// </summary>
    /// <param name="limit">limit to evaluate</param>
    /// <returns>Array of primes numbers that potentially can be emirps</returns>
    private static int[] GetPrimeNumbers(long limit)
    {
        if (_primes?.Count == 0 || _primes?.Max() < limit)
            UpdatePrimesCache(limit);

        var notEmirpNumbers = new char[] { '2', '4', '6', '8', '5' };
        return _primes.Where(a => a > 11 && a <= limit && !notEmirpNumbers.Contains(a.ToString()[0])).ToArray();
    }

    /// <summary>
    /// Reverse the given number using string (e.g. 1024 -> 4201)
    /// </summary>
    /// <param name="number">Number to revers</param>
    /// <returns>Reversed number</returns>
    private static long GetReverse(long number)
    {
        return long.Parse(new string(number.ToString().Reverse().ToArray()));
    }
}