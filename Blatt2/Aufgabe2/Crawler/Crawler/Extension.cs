using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public static class Extension
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                queue.Enqueue(item);
            }
        }
        public static void EnqueueRange<T>(this ConcurrentQueue<T> queue, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                queue.Enqueue(item);
            }
        }
    }
}
