using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Utils
{
    public class RandomTable<T>
    {
        Dictionary<T, int> _entries;
        int _totalWeight;

        public RandomTable()
        {
            _entries = new Dictionary<T, int>();
            _totalWeight = 0;
        }

        public int NumberOfEntries { get { return _entries.Count; } }

        public RandomTable<T> Add(T name, int weight)
        {
            if (weight > 0)
            {
                _entries[name] = weight;
                _totalWeight = _entries.Values.Sum();
            }
            return this;
        }

        public void Clear()
        {
            _entries.Clear();
            _totalWeight = 0;
        }

        public T Roll(Random random)
        {
            T retVal = default;

            if (_totalWeight > 0)
            {
                int roll = random.Next(0, _totalWeight + 1);

                int index = 0;
                do
                {
                    if (roll < _entries.ElementAt(index).Value)
                    {
                        retVal = _entries.ElementAt(index).Key;
                        roll = 0;
                    }
                    else
                    {
                        roll -= _entries.ElementAt(index).Value;
                        if (roll == 0)
                        {
                            retVal = _entries.ElementAt(index).Key;
                        }
                        else
                        {
                            index++;
                        }
                    }
                } while (roll > 0);
            }

            return retVal;
        }
    }
}
