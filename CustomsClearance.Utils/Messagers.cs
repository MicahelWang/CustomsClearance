using System.Collections.Generic;

namespace CustomsClearance.Utils
{
    public class Messagers
    {
        private static Dictionary<string, IMessager> _dictionary;

        private static readonly Messagers _Default;

        static Messagers()
        {
            _Default = new Messagers();
        }

        public static Messagers Default => _Default;
        private static object locker = new object();
        private Messagers()
        {
            _dictionary = new Dictionary<string, IMessager>();;
        }

        public void Register(string key,IMessager messager)
        {
            lock (locker)
            {
                if (!_dictionary.ContainsKey(key))
                {
                    _dictionary.Add(key, messager);
                }
            }
           
        }
        public IMessager this[string key] {
            get
            {
                lock (locker)
                {
                    if (_dictionary.ContainsKey(key))
                    {
                        return _dictionary[key];
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
            }
        }
    }
}