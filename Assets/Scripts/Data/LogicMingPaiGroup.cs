using System.Linq;

namespace Data
{
    public class LogicMingPaiGroup
    {
        public MingPaiKind kind;
        public int self2Otherdistance;
        public CardKind otherCard;
        public CardKind[] selfCard;
        public int signKind;
        public int signNum;

        public LogicMingPaiGroup(LogicMingPaiGroup other)
        {
            kind = other.kind;
            self2Otherdistance= other.self2Otherdistance;
            otherCard = other.otherCard;
            selfCard = other.selfCard.Clone() as CardKind[];
            signKind = other.signKind;
            signNum = other.signNum;
        }
        public LogicMingPaiGroup(MingPaiKind kind, int self2Otherdistance, CardKind otherCard, CardKind[] selfCard, int signKind, int signNum)
        {
            this.kind = kind;
            this.self2Otherdistance = self2Otherdistance;
            this.otherCard = otherCard;
            this.selfCard = selfCard;
            this.signKind = signKind;
            this.signNum = signNum;
        }
        public CardKind[] GetCards()
        {
            if(kind == MingPaiKind.BaBei || kind == MingPaiKind.AnGang)
            {
                return selfCard.Clone() as CardKind[];
            }
            else
            {
                return selfCard.Append(otherCard).ToArray();
            }
        }
        public bool ExsistYaoJiu => otherCard.isYaoJiu || selfCard.Any(_ => _.isYaoJiu);
        public int CountCardNum(CardKind card)
        {
            if (signKind == card.huaseKind)
            {
                if (kind == MingPaiKind.Chi)
                {
                    if (signNum <= card.huaseNum && card.huaseNum <= signNum + 2)
                    {
                        return 1;
                    }
                }
                else
                {
                    return kind switch
                    {
                        MingPaiKind.BaBei => 1,
                        MingPaiKind.MingGang => 4,
                        MingPaiKind.AnGang => 4,
                        MingPaiKind.Peng => 3,
                        MingPaiKind.JiaGang => 4,
                        _ => throw new System.NotImplementedException(),
                    };
                }
            }
            return 0;
        }
        public void JiaGang(CardKind card)
        {
            selfCard = selfCard.Append(card).ToArray();
            kind = MingPaiKind.JiaGang;
        }
        public int OtherCardPosition()
        {
            if(kind != MingPaiKind.Chi)
            {
                throw new System.Exception("不是吃不允许调用 OtherCardIsZhongZhang");
            }
            return otherCard.huaseNum - signNum;
        }
    }
}