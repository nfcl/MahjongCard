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

        public CardKind[] GetCards()
        {
            if(kind == MingPaiKind.BaBei || kind == MingPaiKind.AnGang)
            {
                return selfCard;
            }
            else
            {
                CardKind[] result = new CardKind[selfCard.Length + 1];
                result[selfCard.Length] = otherCard;
                return result;
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
                        MingPaiKind.Chi => throw new System.NotImplementedException(),
                        _ => throw new System.NotImplementedException(),
                    };
                }
            }
            return 0;
        }
    }
}