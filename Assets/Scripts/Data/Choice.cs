using Checker;
using Data;
using Mirror;
using UnityEngine;

namespace Data
{
    public enum ChoiceKind
    {
        None,
        Skip,
        PlayCard,
        LiZhi,
        Gang,
        Peng,
        Chi,
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
                isWhite = false,
            };
        }
    }
    public class ChoiceLiZhi : Choice
    {
        public ClientEachCardTingPais choices;

        public ChoiceLiZhi() : base(ChoiceKind.LiZhi) { }

        public static ChoiceLiZhi LiZhi(ClientEachCardTingPais choices)
        {
            return new ChoiceLiZhi
            {
                choices = choices
            };
        }
    }
    public class ChoiceGang : Choice
    {
        public bool isAnGang;
        public int fromPeople;
        public CardKind[][] choices;
        public CardKind[][] jiaGang;

        public ChoiceGang() : base(ChoiceKind.Gang) { }

        public static ChoiceGang ChoicePlayCardGang(int fromPeople, CardKind[][] choices)
        {
            return new ChoiceGang
            {
                isAnGang = false,
                fromPeople = fromPeople,
                choices = choices,
                jiaGang = null
            };
        }
        public static ChoiceGang ChoiceDrawCardGang(CardKind[][] choices, CardKind[][] jiaGang)
        {
            return new ChoiceGang
            {
                isAnGang = true,
                fromPeople = -1,
                choices = choices,
                jiaGang = jiaGang
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
    public static void WriteChoice(this NetworkWriter writer, Choice choice)
    {
        writer.Write<ChoiceKind>(choice.kind);
        switch (choice.kind)
        {
            case ChoiceKind.PlayCard:
                {
                    ChoicePlayCard total = choice as ChoicePlayCard;
                    writer.WriteBool(total.isWhite);
                    writer.WriteArray(total.cards);
                    break;
                }
            case ChoiceKind.LiZhi:
                {
                    ChoiceLiZhi total = choice as ChoiceLiZhi;
                    writer.Write<ClientEachCardTingPais>(total.choices);
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
                    writer.WriteBool(total.isAnGang);
                    writer.WriteInt(total.choices.Length);
                    total.choices.Foreach((_, index) =>
                    {
                        writer.WriteArray(_);
                    });
                    if (!total.isAnGang)
                    {
                        writer.WriteInt(total.fromPeople);
                    }
                    else
                    {
                        writer.WriteInt(total.jiaGang.Length);
                        total.jiaGang.Foreach((_, index) =>
                        {
                            writer.WriteArray(_);
                        });
                    }
                    break;
                }
        }
    }
    public static Choice ReadChoice(this NetworkReader reader)
    {
        ChoiceKind kind = reader.Read<ChoiceKind>();
        switch (kind)
        {
            case ChoiceKind.PlayCard:
                {
                    return new ChoicePlayCard
                    {
                        isWhite = reader.ReadBool(),
                        cards = reader.ReadArray<CardKind>(),
                    };
                }
            case ChoiceKind.LiZhi:
                {
                    return ChoiceLiZhi.LiZhi(reader.Read<ClientEachCardTingPais>());
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
                        choice.choices = reader.ReadArray <CardKind[]>();
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
                        choice.choices = reader.ReadArray<CardKind[]>();
                    }
                    return choice;
                }
            case ChoiceKind.Gang:
                {
                    bool isAnGang = reader.ReadBool();
                    CardKind[][] choices = new CardKind[reader.ReadInt()][];
                    for(int i = 0; i < choices.Length; ++i)
                    {
                        choices[i] = reader.ReadArray<CardKind>();
                    }
                    if (!isAnGang)
                    {
                        return ChoiceGang.ChoicePlayCardGang(reader.ReadInt(), choices);
                    }
                    else
                    {
                        CardKind[][] jiaGang = new CardKind[reader.ReadInt()][];
                        for (int i = 0; i < choices.Length; ++i)
                        {
                            choices[i] = reader.ReadArray<CardKind>();
                        }
                        return ChoiceGang.ChoiceDrawCardGang(choices, jiaGang);
                    }
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