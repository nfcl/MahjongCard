using System;

namespace Data
{
    public struct CardKind :IComparable<CardKind>
    {
        public int value;

        public CardKind(int kind)
        {
            value = kind;
        }

        public static bool operator ==(CardKind self, CardKind other)
        {
            return self.value == other.value;
        }
        public static bool operator !=(CardKind self, CardKind other)
        {
            return self.value != other.value;
        }
        public override bool Equals(object obj)
        {
            return value == ((CardKind)obj).value;
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override string ToString()
        {
            if (value < 0 || value >= 37)
            {
                throw new System.Exception($"Error CardKind{value}");
            }
            return value switch
            {
                < 10 => $"{value - 00}m",
                < 20 => $"{value - 10}p",
                < 30 => $"{value - 20}s",
                _ => $"{value - 30}z",
            };
        }

        public static CardKind GetRandomKind()
        {
            return new CardKind(UnityEngine.Random.Range(0, 37));
        }
        public int CompareTo(CardKind other)
        {
            return value.CompareTo(other.value);
        }
        public class LogicComparer
        {
            public static int[] order =
            {
                 4, 0, 1, 2, 3, 5, 6, 7, 8, 9,
                14,10,11,12,13,15,16,17,18,19,
                24,20,21,22,23,25,26,27,28,29,
                30,31,32,33,34,35,36
            };
            public static int Compare(CardKind x, CardKind y)
            {
                return order[x.value] - order[y.value];
            }
        }
    }
}