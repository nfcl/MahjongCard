using Data;
using System;

namespace Checker
{
    public class EachDrewCardTingPaiResult
    {
        public CardKind lastDrewCard;
        public TingPaiResult tingPai;

        public EachDrewCardTingPaiResult(CardKind lastDrewCard)
        {
            this.lastDrewCard = lastDrewCard;
            this.tingPai = new TingPaiResult();
        }
        public EachDrewCardTingPaiResult(CardKind lastDrewCard, TingPaiResult tingPai)
        {
            this.lastDrewCard = lastDrewCard;
            this.tingPai = tingPai;
        }

        public static EachDrewCardTingPaiResult Check(CheckInfo infos, CardKind[] hands, LogicMingPaiGroup[] mings, CardKind lastDrewCard, Func<YiKind, bool, Yi> fanDecoder)
        {
            EachDrewCardTingPaiResult result = new(lastDrewCard, TingPaiResult.CheckTingPaiResult(hands, mings));

            YiZhongResult.Check(infos, result.tingPai, hands, mings, lastDrewCard, fanDecoder);

            return result;
        }
    }
}