using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static TElement Find<TElement>(this IEnumerable<TElement> collection, Predicate<TElement> comparator)
    {
        if (collection is null)
            throw new ArgumentNullException("Input collection cannot be null");

        if (comparator is null)
            throw new NullReferenceException("Comparator cannot be null");

        foreach (TElement element in collection)
            if (comparator(element))
                return element;

        return default;
    }

    public static bool Contains<TElement>(this IEnumerable<TElement> collection, Predicate<TElement> comparator)
    {
        if (collection is null)
            throw new ArgumentNullException("Input collection cannot be null");

        if (comparator is null)
            throw new NullReferenceException("Comparator cannot be null");

        foreach (TElement element in collection)
            if (comparator(element))
                return true;

        return false;
    }

    public static int IndexOf<TElement>(this IEnumerable<TElement> collection, Predicate<TElement> comparison)
    {
        int index = 0;

        foreach (TElement e in collection)
        {
            if (comparison(e))
                return index;

            index++;
        }

        return -1;
    }

    public static int LastIndexOf<TElement>(this IEnumerable<TElement> collection, Predicate<TElement> comparison)
    {
        int index = 0;
        int result = -1;

        foreach (TElement e in collection)
        {
            if (comparison(e))
                result = index;

            index++;
        }

        return result;
    }

    public static TElement FindMin<TElement>(this IEnumerable<TElement> collection,  
        Func<TElement, TElement, bool> nextElementMore)
    {
        TElement last = collection.FirstOrDefault();

        foreach (TElement e in collection)
        {
            if (nextElementMore(last, e))
                last = e;
        }

        return last;
    }
    public static bool Exist<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, bool> comparer)
    {
        foreach (TValue value in enumerable)
            if (comparer(value))
                return true;

        return false;
    }

    public static int IndexOfMin<TElement>(this IEnumerable<TElement> collection, Comparison<TElement> comparison)
    {
        return collection.IndexOfMaxOrMin(comparison, comparisonResult => comparisonResult > 0);
    }

    public static int IndexOfMax<TElement>(this IEnumerable<TElement> collection, Comparison<TElement> comparison)
    {
        return collection.IndexOfMaxOrMin<TElement>(comparison, comparisonResult => comparisonResult < 0);
    }

    private static int IndexOfMaxOrMin<TElement>(this IEnumerable<TElement> collection,
        Comparison<TElement> comparison, Predicate<int> comparisonComparer)
    {
        if (collection.Count() is 0)
            return -1;

        int currentIndex = 0;
        int resultIndex = 0;
        TElement element = collection.First();

        foreach (TElement item in collection)
        {
            if (comparisonComparer(comparison(element, item)))
            {
                resultIndex = currentIndex;
                element = item;
            }

            currentIndex++;
        }

        return resultIndex;
    }
}
