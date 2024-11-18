using System.Linq;

namespace Data
{
    public class LogicMingPaiGroup
    {
        public MingPaiKind kind;
        public int self2Otherdistance;
        public CardKind otherCard;
        public CardKind[] selfCard;

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
    }
}