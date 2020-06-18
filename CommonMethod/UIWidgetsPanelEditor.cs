using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.UIWidgets.CommonMethod {
    public static partial class Enumerable {
        public static List<TResult> Repeat<TResult>(TResult element, int count) {
            List<TResult> results = new List<TResult>();
            for (int i = 0; i < count; i++) {
                results.Add(element);
            }

            return results;
        }

        public static bool Contains<TSource>(IEnumerable<TSource> source, TSource value) {
            if (source == null) {
                return false;
            }

            foreach (TSource element in source) {
                if (EqualityComparer<TSource>.Default.Equals(element, value)) {
                    return true;
                }
            }

            return false;
        }

        public static List<T> ToList<T>(this IEnumerable<T> source) {
            List<T> result = new List<T>();
            foreach (var VARIABLE in source) {
                result.Add(VARIABLE);
            }

            return result;
        }

        public static T[] ToArray<T>(this IEnumerable<T> source) {
            if (source is ICollection<T> ic) {
                int count = ic.Count;
                if (count != 0) {
                    T[] arr = new T[count];
                    ic.CopyTo(arr, 0);
                    return arr;
                }
            }
            else {
                using (var en = source.GetEnumerator()) {
                    if (en.MoveNext()) {
                        const int DefaultCapacity = 4;
                        T[] arr = new T[DefaultCapacity];
                        arr[0] = en.Current;
                        int count = 1;

                        while (en.MoveNext()) {
                            if (count == arr.Length) {
                                const int MaxArrayLength = 0x7FEFFFFF;
                                int newLength = count << 1;
                                if ((uint) newLength > MaxArrayLength) {
                                    newLength = MaxArrayLength <= count ? count + 1 : MaxArrayLength;
                                }

                                Array.Resize(ref arr, newLength);
                            }

                            arr[count++] = en.Current;
                        }

                        return arr;
                    }
                }
            }
            return Array.Empty<T>();
        }

        public static TResult[] Select<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector) {
            if (!check(source, selector)) {
                return new TResult[0];
            }

            List<TResult> result = new List<TResult>();
            foreach (var VARIABLE in source) {
                result.Add(selector(VARIABLE));
            }


            return result.ToArray();
        }

        static bool check<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector) {
            return source == null || selector == null;
        }

        public static TSource[] Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate) {
            if (!check(source, predicate)) {
                return new TSource[0];
            }
            List<TSource> result = new List<TSource>();
            foreach (var VARIABLE in source) {
                if (predicate(VARIABLE)) {
                   result.Add(VARIABLE);
                }
            }

            return result.ToArray();
        }
        
        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            if (source is IEnumerable<TResult> typedSource)
            {
                return typedSource;
            }

            if (source == null) {
                return null;
            }
            return CastIterator<TResult>(source);
        }
        
        private static IEnumerable<TResult> CastIterator<TResult>(IEnumerable source)
        {
            foreach (object obj in source)
            {
                yield return (TResult)obj;
            }
        }
        
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            var comparer = EqualityComparer<TSource>.Default;

            if (first == null) {
                return false;
            }

            if (second == null) {
                return false;
            }

            if (first is ICollection<TSource> firstCol && second is ICollection<TSource> secondCol)
            {
                if (firstCol.Count != secondCol.Count)
                {
                    return false;
                }

                if (firstCol is IList<TSource> firstList && secondCol is IList<TSource> secondList)
                {
                    int count = firstCol.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (!comparer.Equals(firstList[i], secondList[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            using (IEnumerator<TSource> e1 = first.GetEnumerator())
            using (IEnumerator<TSource> e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && comparer.Equals(e1.Current, e2.Current)))
                    {
                        return false;
                    }
                }

                return !e2.MoveNext();
            }
        }
        
        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null)
            {
                throw new  Exception("source null");
            }

            if (func == null)
            {
                throw new  Exception("func null");
            }

            TAccumulate result = seed;
            foreach (TSource element in source)
            {
                result = func(result, element);
            }

            return result;
        }
        
        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new  Exception("source null");
            }

            if (func == null)
            {
                throw new  Exception("func null");
            }

            if (resultSelector == null)
            {
                throw new  Exception("result selector null");
            }

            TAccumulate result = seed;
            foreach (TSource element in source)
            {
                result = func(result, element);
            }

            return resultSelector(result);
        }

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source) {
            List<TSource> list = new List<TSource>();
            foreach (var VARIABLE in source) {
                list.Add(VARIABLE);
            }

            return list;
        }
        
        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) {
                throw new Exception("source");
            }

            if (predicate == null)
            {
                throw new Exception("predicate");
            }

            foreach (TSource element in source)
            {
                if (!predicate(element))
                {
                    return false;
                }
            }

            return true;
        }

        public static float Sum(this IEnumerable<float> source) {
            if (source == null) {
                return 0;
            }

            float answer = 0;
            foreach (var VARIABLE in source) {
                answer += VARIABLE;
            }

            return answer;
        }
        
        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new Exception("wrong");
            }

            if (source is IList<TSource> list)
            {
                switch (list.Count)
                {
                    case 0:
                        throw new Exception("wrong");
                        return default;
                    case 1:
                        return list[0];
                }
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                    {
                        throw new Exception("wrong");
                    }

                    TSource result = e.Current;
                    if (!e.MoveNext())
                    {
                        return result;
                    }
                }
            }

            throw new Exception("wrong");
            return default;
        }
        
        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new Exception("wrong");
            }

            if (source is IList<TSource> list)
            {
                switch (list.Count)
                {
                    case 0:
                        throw new Exception("wrong");
                        return default;
                    case 1:
                        foreach (var VARIABLE in list) {
                            if (predicate(VARIABLE)) {
                                return VARIABLE;
                            }
                        }
                        break;
                }
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                    {
                        throw new Exception("wrong");
                    }

                    TSource result = e.Current;
                    if (predicate(result) && !e.MoveNext())
                    {
                        return result;
                    }
                }
            }

            throw new Exception("wrong");
            return default;
        }
        
        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new Exception("source null");
            }

            if (source is ICollection<TSource> collectionoft)
            {
                return collectionoft.Count != 0;
            }
            else if (source is ICollection collection)
            {
                return collection.Count != 0;
            }

            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                return e.MoveNext();
            }
        }
        
        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw new Exception("null");
            }

            if (predicate == null)
            {
                throw new Exception("null");
            }

            int count = 0;
            foreach (TSource element in source)
            {
                checked
                {
                    if (predicate(element))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
        
        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            TSource first = source.TryGetFirst(null,out bool found);
            if (!found)
            {
                throw new Exception();
            }

            return first;
        }
        
        private static TSource TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
        {
            if (source == null)
            {
                throw new Exception();
            }

            if (predicate == null)
            {
                foreach (TSource element in source)
                {
                    found = true;
                    return element;
                }
            }
            else {
                foreach (TSource element in source) {
                    if (predicate(element)) {
                        found = true;
                        return element;
                    }
                }
            }

            found = false;
            return default;
        }
    }
}