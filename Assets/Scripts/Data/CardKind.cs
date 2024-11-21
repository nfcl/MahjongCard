using Data;
using GameLogic;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public enum HuaSe
    {
        Wan,
        Bing,
        Tiao,
        Feng,
        SanYuan
    }
    public struct CardKind : IComparable<CardKind>
    {
        public int value;

        public int huaseNum => value % 10 - (value >= 30 ? 0 : 1);
        public int huaseKind => value / 10;
        public HuaSe huaSe => (HuaSe)(value > 33 ? 4 : (value / 10));
        public bool haveHongBao => value == 0 || value == 5 || value == 10 || value == 15 || value == 20 || value == 25;
        public int realValue => isHongBao ? value + 5 : value;
        public bool isYaoJiu => value == 1 || value == 9 || value == 11 || value == 19 || value == 21 || value == 29 || value >= 30;
        public bool isFengPai => 30 <= value && value < 33;
        public bool isSanyuanPai => 33 <= value && value < 37;
        public bool isZiPai => 30 <= value && value < 37;
        public bool isHongBao => value == 0 || value == 10 || value == 20;
        public bool isBao => isHongBao || IGameLogicManager.instance.isBaoPai(this);

        public CardKind(int kind)
        {
            value = kind;
        }
        public CardKind(int kind, int num)
        {
            value = kind * 10 + num + kind == 3 ? 0 : 1;
        }

        public static bool operator ==(CardKind self, CardKind other)
        {
            return Equals(self, other);
        }
        public static bool operator !=(CardKind self, CardKind other)
        {
            return Equals(self, other);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)

                return false;

            if (GetType() != obj.GetType())
                return false;

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
        public static string ToString(CardKind[] kinds)
        {
            if (kinds == null)
            {
                return "null";
            }
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var _ in kinds)
                {
                    sb.Append($"{_.value}\t");
                }
                return sb.ToString();
            }
            catch
            {
                return "null";
            }
        }
        public static string ToString(CardKind[][] kindss)
        {
            if (kindss == null)
            {
                return "null";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var _ in kindss)
            {
                if (_ == null)
                {
                    sb.Append("null\n");
                }
                else
                {
                    sb.Append($"{ToString(_)}\n");
                }
            }
            return sb.ToString();
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
        public static CardKind[] Divider(IEnumerable<CardKind> group)
        {
            return group
                .GroupBy(_ => _.haveHongBao)
                .Select(_ => _.First())
                .ToArray();
        }
    }
}

public static class CardKindSerializer
{
    public static void Writer(NetworkWriter writer, CardKind kind)
    {
        writer.WriteInt(kind.value);
    }
    public static CardKind Read(NetworkReader reader)
    {
        return new CardKind(reader.ReadInt());
    }
}