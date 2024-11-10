using System;
using System.Collections;

namespace Data
{
    public class CardKind :IComparable<CardKind>
    {
        public int value = 0;

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

        public int CompareTo(CardKind other)
        {
            return value.CompareTo(other.value);
        }
    }
}