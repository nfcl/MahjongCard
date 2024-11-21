using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Checker
{
    public class EachHandCardTingPaiResult
    {
        public ((int, int), EachDrewCardTingPaiResult[])[] choices;

        public bool isHePai => choices
            .Any(_ =>
                _.Item2
                    .Any(__ =>
                        __.lastDrewCard.huaseKind == _.Item1.Item1 && __.lastDrewCard.huaseNum == _.Item1.Item2
                    )
            );

        public static EachHandCardTingPaiResult Check(CheckInfo infos, CardKind[] hands, LogicMingPaiGroup[] mings, Func<YiKind, bool, Yi> fanDecoder)
        {
            List<((int, int), EachDrewCardTingPaiResult[])> results = new List<((int, int), EachDrewCardTingPaiResult[])>();

            hands
                .DistinctBy(_ => (_.huaseKind, _.huaseNum))
                .Foreach((card, __index) =>
                {
                    List<EachDrewCardTingPaiResult> eachDrewCardResults = new List<EachDrewCardTingPaiResult>();
                    int replaceIndex = hands.FindIndex(_ => _.huaseKind == card.huaseKind && _.huaseNum == card.huaseNum);
                    for (int i = 0; i < 34; ++i)
                    {
                        var newHands = hands.Clone() as CardKind[];
                        newHands[replaceIndex] = new CardKind(i / 9, i % 9);
                        var cardResult = EachDrewCardTingPaiResult.Check(infos, newHands, mings, newHands[replaceIndex], fanDecoder);
                        if (cardResult.tingPai.canTingPai)
                        {
                            eachDrewCardResults.Add(cardResult);
                        }
                    }
                    if (eachDrewCardResults.Count != 0)
                    {
                        results.Add(((card.huaseKind, card.huaseNum), eachDrewCardResults.ToArray()));
                    }
                });

            return new EachHandCardTingPaiResult { choices = results.ToArray() };
        }
    }
}