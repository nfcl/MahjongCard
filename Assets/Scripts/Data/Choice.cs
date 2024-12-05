using Checker;
using Data;
using Mirror;
using System.Linq;
using UnityEngine;
using static Data.ChoiceGang;

namespace Data
{
    public enum ChoiceKind
    {
        None,
        Skip,
        TingPai,
        PlayCard,
        LiZhi,
        Gang,
        Peng,
        Chi,
        BaBei,
        JiuZhongJiuPai,
        ZiMo,
        RongHe
    }
    public class Choice
    {
        public ChoiceKind kind;

        public Choice()
        {
            kind = ChoiceKind.None;
        }
        public Choice(ChoiceKind kind)
        {
            this.kind = kind;
        }

        public override string ToString()
        {
            return $"kind = {kind}";
        }
        public static string ToString(Choice[] choices)
        {
            return choices.Aggregate("Choices : ", (_, __) => $"{_}\n\t{__}");
        }
    }
    public class ChoiceTingPai: Choice
    {
        public ClientEachCardTingPais choices;
        public bool isLiZhi;

        public ChoiceTingPai() : base(ChoiceKind.TingPai) { }

        public static ChoiceTingPai TingPai(ClientEachCardTingPais choices, bool isLiZhi)
        {
            return new ChoiceTingPai
            {
                choices = choices,
                isLiZhi = isLiZhi
            };
        }
    }
    public class ChoicePlayCard : Choice
    {
        public bool isWhite;
        public CardKind[] cards;

        public ChoicePlayCard() : base(ChoiceKind.PlayCard) { }

        public static ChoicePlayCard NormalPlayCard()
        {
            return new ChoicePlayCard()
            {
                cards = new CardKind[0],
                isWhite = false
            };
        }
        public static ChoicePlayCard BanPlayCard(CardKind[] cards)
        {
            return new ChoicePlayCard()
            {
                cards = cards,
                isWhite = false
            };
        }
    }
    public class ChoiceLiZhi : Choice
    {
        public ChoiceLiZhi() : base(ChoiceKind.LiZhi) { }

        public static ChoiceLiZhi LiZhi()
        {
            return new ChoiceLiZhi();
        }
    }
    public class ChoiceGang : Choice
    {
        public enum GangKind
        {
            MingGang,
            AnGang,
            JiaGang
        }
        public class GangData
        {
            public CardKind[] cards;
            public GangKind kind;
        }
        public GangData[] choices;

        public ChoiceGang() : base(ChoiceKind.Gang) { }

        public static ChoiceGang Gang(GangData[] choices)
        {
            return new ChoiceGang
            {
                choices = choices
            };
        }
    }
    public class ChoicePeng : Choice
    {
        public int fromPeople;
        public CardKind fromPeopleCard;
        public CardKind[][] choices;

        public ChoicePeng() : base(ChoiceKind.Peng) { }

        public static ChoicePeng NormalPeng(int fromPeople, CardKind fromPeopleCard, CardKind[][] choices)
        {
            return new ChoicePeng
            {
                fromPeople = fromPeople,
                fromPeopleCard = fromPeopleCard,
                choices = choices
            };
        }
    }
    public class ChoiceChi : Choice
    {
        public int fromPeople;
        public CardKind fromPeopleCard;
        public CardKind[][] choices;

        public ChoiceChi(): base(ChoiceKind.Chi) { }

        public static ChoiceChi NormalChi(int fromPeople, CardKind fromPeopleCard, CardKind[][] choices)
        {
            return new ChoiceChi
            {
                fromPeople = fromPeople,
                fromPeopleCard = fromPeopleCard,
                choices = choices
            };
        }
    }
    public class ChoiceJiuZhongJiuPai : Choice
    {
        public ChoiceJiuZhongJiuPai() : base(ChoiceKind.JiuZhongJiuPai) { }
    }
    public class ChoiceZiMo : Choice
    {
        public ChoiceZiMo() : base(ChoiceKind.ZiMo) { }

        public static ChoiceZiMo ZiMo()
        {
            return new ChoiceZiMo();
        }
    }
    public class ChoiceRongHe : Choice
    {
        public ChoiceRongHe() : base(ChoiceKind.RongHe) { }

        public static ChoiceRongHe RongHe()
        {
            return new ChoiceRongHe();
        }
    }
}

public static class ChoiceSerializer
{
    public static void WriteChoiceKind(this NetworkWriter writer, ChoiceKind kind)
    {
        writer.WriteInt((int)kind);
    }
    public static ChoiceKind ReadChoiceKind(this NetworkReader reader)
    {
        return (ChoiceKind)reader.ReadInt();
    }
    public static void WriteGangKind(this NetworkWriter writer, GangKind kind)
    {
        writer.WriteInt((int)kind);
    }
    public static GangKind ReadGangKind(this NetworkReader reader)
    {
        return (GangKind)reader.ReadInt();
    }
    public static void WriteGangData(this NetworkWriter writer, GangData data)
    {
        writer.Write<GangKind>(data.kind);
        writer.WriteArray<CardKind>(data.cards);
    }
    public static GangData ReadGangData(this NetworkReader reader)
    {
        return new GangData
        {
            kind = reader.Read<GangKind>(),
            cards = reader.ReadArray<CardKind>()
        };
    }
    public static void WriteChoice(this NetworkWriter writer, Choice choice)
    {
        writer.Write<ChoiceKind>(choice.kind);
        switch (choice.kind)
        {
            case ChoiceKind.TingPai:
                {
                    ChoiceTingPai total = choice as ChoiceTingPai;
                    writer.Write<ClientEachCardTingPais>(total.choices);
                    writer.WriteBool(total.isLiZhi);
                    break;
                }
            case ChoiceKind.PlayCard:
                {
                    ChoicePlayCard total = choice as ChoicePlayCard;
                    writer.WriteBool(total.isWhite);
                    writer.WriteArray(total.cards);
                    break;
                }
            case ChoiceKind.Peng:
                {
                    ChoicePeng total = choice as ChoicePeng;
                    writer.WriteInt(total.fromPeople);
                    writer.Write(total.fromPeopleCard);
                    writer.WriteInt(total.choices.Length);
                    total.choices.Foreach((_, index) =>
                    {
                        writer.WriteArray(_);
                    });
                    break;
                }
            case ChoiceKind.Chi:
                {
                    ChoiceChi total = choice as ChoiceChi;
                    writer.WriteInt(total.fromPeople);
                    writer.Write(total.fromPeopleCard);
                    writer.WriteInt(total.choices.Length);
                    total.choices.Foreach((_, index) =>
                    {
                        writer.WriteArray(_);
                    });
                    break;
                }
            case ChoiceKind.Gang:
                {
                    ChoiceGang total = choice as ChoiceGang;
                    writer.WriteArray(total.choices);
                    break;
                }
        }
    }
    public static Choice ReadChoice(this NetworkReader reader)
    {
        ChoiceKind kind = reader.Read<ChoiceKind>();
        switch (kind)
        {
            case ChoiceKind.TingPai:
                {
                    return ChoiceTingPai.TingPai(reader.Read<ClientEachCardTingPais>(), reader.ReadBool());
                }
            case ChoiceKind.PlayCard:
                {
                    return new ChoicePlayCard
                    {
                        isWhite = reader.ReadBool(),
                        cards = reader.ReadArray<CardKind>()
                    };
                }
            case ChoiceKind.LiZhi:
                {
                    return ChoiceLiZhi.LiZhi();
                }
            case ChoiceKind.JiuZhongJiuPai:
                {
                    return new ChoiceJiuZhongJiuPai();
                }
            case ChoiceKind.Peng:
                {
                    ChoicePeng choice = new ChoicePeng
                    {
                        fromPeople = reader.ReadInt(),
                        fromPeopleCard = reader.Read<CardKind>(),
                        choices = new CardKind[reader.ReadInt()][]
                    };
                    for(int i = 0; i < choice.choices.Length; ++i)
                    {
                        choice.choices[i] = reader.ReadArray<CardKind>();
                    }
                    return choice;
                }
            case ChoiceKind.Chi:
                {
                    ChoiceChi choice = new ChoiceChi
                    {
                        fromPeople = reader.ReadInt(),
                        fromPeopleCard = reader.Read<CardKind>(),
                        choices = new CardKind[reader.ReadInt()][]
                    };
                    for (int i = 0; i < choice.choices.Length; ++i)
                    {
                        choice.choices[i] = reader.ReadArray<CardKind>();
                    }
                    return choice;
                }
            case ChoiceKind.Gang:
                {
                    return ChoiceGang.Gang(reader.ReadArray<GangData>());
                }
            case ChoiceKind.ZiMo:
                {
                    return ChoiceZiMo.ZiMo();
                }
            case ChoiceKind.RongHe:
                {
                    return ChoiceRongHe.RongHe();
                }
        }
        return new Choice();
    }
}