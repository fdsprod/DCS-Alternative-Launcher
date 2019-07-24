using System;
using System.Collections.Generic;
using System.Linq;

namespace DCS.Alternative.Launcher.Collections
{
    public class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly object _padlock = new object();

        public TValue this[TKey key]
        {
            set
            {
                lock (_padlock)
                {
                    TValue current;
                    if (_dictionary.TryGetValue(key, out current))
                    {
                        var disposable = current as IDisposable;

                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    _dictionary[key] = value;
                }
            }
        }

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public void Dispose()
        {
            lock (_padlock)
            {
                var disposableItems = from item in _dictionary.Values
                    where item is IDisposable
                    select item as IDisposable;

                foreach (var item in disposableItems)
                {
                    item.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_padlock)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        public bool Remove(TKey key)
        {
            lock (_padlock)
            {
                return _dictionary.Remove(key);
            }
        }

        public void Clear()
        {
            lock (_padlock)
            {
                _dictionary.Clear();
            }
        }
    }
}