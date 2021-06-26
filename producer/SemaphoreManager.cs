using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace kafka_poc
{
    static class SemaphoreManager
    {
        static readonly IDictionary<Keys, SemaphoreSlim> _semaphores = new Dictionary<Keys, SemaphoreSlim>();

        public enum Keys
        {
            Preferences = 0
        }

        static SemaphoreSlim GetSemaphore(Keys key)
        {
            if (!_semaphores.ContainsKey(key))
                _semaphores.Add(key, new SemaphoreSlim(1, 1));
            return _semaphores[key];
        }

        public static async Task Lock(Keys key) => await GetSemaphore(key).WaitAsync();
        public static void Release(Keys key) => GetSemaphore(key).Release();
    }
}
