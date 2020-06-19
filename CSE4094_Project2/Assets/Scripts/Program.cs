using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    class CuckooTable<T> : IEnumerable<T>
    {
        private KeyValuePair<int, T>[] table;
        public int Count { private set; get; }

        public CuckooTable()
        {
            table = new KeyValuePair<int, T>[16];
            Count = 0;
        }

        private int GetFirstIndex(T value, int length)
        {
            return Math.Abs(value.GetHashCode() % length);
        }

        private int GetFirstIndex(T value)
        {
            return Math.Abs(value.GetHashCode() % table.Length);
        }

        private int GetSecondIndex(T value, int length)
        {
            return Math.Abs((value.GetHashCode() * 83 + 7) % length);
        }

        private int GetSecondIndex(T value)
        {
            return Math.Abs((value.GetHashCode() * 83 + 7) % table.Length);
        }

        private int GetThirdIndex(T value)
        {
            return Math.Abs(value.GetHashCode() * (value.GetHashCode() + 19) % table.Length);
        }

        private int GetThirdIndex(T value, int length)
        {
            return Math.Abs(value.GetHashCode() * (value.GetHashCode() + 19) % length);
        }
        private KeyValuePair<int, T>[] Resize()
        {
            KeyValuePair<int, T>[] newTable = new KeyValuePair<int, T>[table.Length * 2];
            for (int i = 0; i < table.Length; i++)
            {
                newTable[i] = table[i];
            }
            return newTable;
        }

        public bool Contains(T value)
        {
            KeyValuePair<int, T> forComparison = new KeyValuePair<int, T>(value.GetHashCode(), value);
            for (int i = table.Length; i >= 16; i /= 2)
            {
                if (table[GetFirstIndex(value, i)].Equals(forComparison) || table[GetSecondIndex(value, i)].Equals(forComparison) || table[GetThirdIndex(value, i)].Equals(forComparison))
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(T value)
        {
            List<int> indexes = new List<int>();
            if (Contains(value))
            {
                throw new ArgumentException("This element is already present in the table!");
            }
            if (table[GetFirstIndex(value)].Equals(default(KeyValuePair<int, T>)))
            {
                table[GetFirstIndex(value)] = new KeyValuePair<int, T>(value.GetHashCode(), value);
                Count++;
                return;
            }
            else
            {
                if (!KickElement(value, indexes))
                {
                    table = Resize();
                    KickElement(value, indexes);
                }
                else
                {
                    Count++;
                }
            }
        }

        private bool KickElement(T value, List<int> indexes)
        {
            T exchange = table[GetSecondIndex(value)].Value;
            if (table[GetSecondIndex(value)].Equals(default(KeyValuePair<int, T>)))
            {
                table[GetSecondIndex(value)] = new KeyValuePair<int, T>(value.GetHashCode(), value);
                return true;
            }
            if (table[GetThirdIndex(value)].Equals(default(KeyValuePair<int, T>)))
            {
                table[GetThirdIndex(value)] = new KeyValuePair<int, T>(value.GetHashCode(), value);
                return true;
            }
            if (indexes.Contains(GetSecondIndex(value)))
            {
                return false;
            }
            indexes.Add(GetSecondIndex(value));
            table[GetSecondIndex(value)] = new KeyValuePair<int, T>(value.GetHashCode(), value);
            return KickElement(exchange, indexes);
        }

        public void Print()
        {
            string spaces = "__________________________________________________________________";
            Console.WriteLine(spaces);
            foreach (KeyValuePair<int, T> element in table)
            {
                Console.WriteLine(element.Key + "   " + element.Value);
            }
            Console.WriteLine(spaces);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (KeyValuePair<int, T> element in table)
            {
                if (!element.Equals(default(KeyValuePair<int, T>)))
                {
                    yield return element.Value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<int, T>>)this).GetEnumerator();
        }
    }
    static void Main(string[] args)
    {
        CuckooTable<string> cuckoo = new CuckooTable<string>();
        cuckoo.Add("bratan");
        cuckoo.Add("kak dela?");
        cuckoo.Add("che novogo?");
        cuckoo.Add("gde byl?");
        cuckoo.Add("che videl?");
        cuckoo.Add("abrakadabra");
        cuckoo.Add("rabotaiiii");
        cuckoo.Add("blin");
        cuckoo.Add("doge?");
        cuckoo.Add("mr cats?");
        cuckoo.Add("alloha?");
        cuckoo.Add("yoyo");
        cuckoo.Add("bandos");
        cuckoo.Add("hello?");
        cuckoo.Add("world?");
        cuckoo.Add("blin");
        cuckoo.Add("nichego sebe");
        cuckoo.Add("eta erunda rabotaet");
        cuckoo.Add("mlg");
        cuckoo.Add("noscope");
        cuckoo.Add("snipars");
        cuckoo.Add("trickshot");
        cuckoo.Add("zac");
        foreach (string str in cuckoo)
        {
            Console.WriteLine(str);
        }
        cuckoo.Print();
        Console.ReadLine();
    }
}

