using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public class LogicHandCard
    {
        public List<CardKind> cards;
        public CardKind[] Cards => cards.ToArray();
        public bool hasYaoJiu => cards.Any(_ => _.isYaoJiu);
        public CardKind lastDrewCard;

        public LogicHandCard()
        {
            cards = new List<CardKind>();
        }
        public void Init(CardKind[] configurCards)
        {
            cards = configurCards.ToList();
            lastDrewCard = cards[cards.Count - 1];
        }
        public void DrawCard(CardKind card)
        {
            cards.Add(card);
            lastDrewCard = card;
        }
        public void PlayCard(CardKind card)
        {
            cards.Remove(card);
        }
        public void BeMingedCard(LogicMingPaiGroup group)
        {
            foreach (CardKind card in group.selfCard)
            {
                cards.Remove(card);
            }
        }
        public CardKind[][] CheckChi(CardKind other)
        {
            if (other.huaseKind == 3)
            {
                return new CardKind[0][];
            }
            List<CardKind[]> results = new List<CardKind[]>();

            CardKind[][] cardsOrderedPosition = new CardKind[4][]
            {
                CardKind.Divider(cards.Where(_ => CardKind.LogicEqualityComparer.Equals(_, other.huaseKind, other.huaseNum - 2))).ToArray(),
                CardKind.Divider(cards.Where(_ => CardKind.LogicEqualityComparer.Equals(_, other.huaseKind, other.huaseNum - 1))).ToArray(),
                CardKind.Divider(cards.Where(_ => CardKind.LogicEqualityComparer.Equals(_, other.huaseKind, other.huaseNum + 1))).ToArray(),
                CardKind.Divider(cards.Where(_ => CardKind.LogicEqualityComparer.Equals(_, other.huaseKind, other.huaseNum + 2))).ToArray()
            };

            cardsOrderedPosition[0].Foreach((_, index) =>
                cardsOrderedPosition[1].Foreach((__, _index) =>
                    results.Add(new CardKind[] { _, __, other })));
            cardsOrderedPosition[1].Foreach((_, index) =>
                cardsOrderedPosition[2].Foreach((__, _index) =>
                    results.Add(new CardKind[] { _, __, other })));
            cardsOrderedPosition[2].Foreach((_, index) =>
                cardsOrderedPosition[3].Foreach((__, _index) =>
                    results.Add(new CardKind[] { _, __, other })));

            return results.ToArray();
        }
        private void MingPaiSelect(int resultNum,int index, (CardKind, int)[] src, List<CardKind> road, List<CardKind[]> result)
        {
            List<CardKind> tempRoad = new List<CardKind>(road);
            for (int i = 0; i < src[index].Item2 && i <= resultNum - road.Count; ++i)
            {
                tempRoad.Add(src[index].Item1);
                List<CardKind> _tempRoad = new List<CardKind>(tempRoad);
                if(_tempRoad.Count == resultNum)
                {
                    result.Add(_tempRoad.ToArray());
                }
                else if(index + 1 < src.Length)
                {
                    MingPaiSelect(resultNum, index + 1, src, _tempRoad, result);
                }
            }
        }
        public CardKind[][] CheckPeng(CardKind other)
        {
            List<CardKind[]> results = new List<CardKind[]>();
            var dividedKinds = cards
                .Where(_ => _.realValue == other.realValue)
                .GroupBy(_ => _.isHongBao)
                .Select(_ =>(_.First(),_.Count()))
                .ToArray();
            if (dividedKinds.Length != 0)
            {
                MingPaiSelect(2, 0, dividedKinds, new List<CardKind>(), results);
            }
            return results.Select(_ => _.Append(other).ToArray()).ToArray();
        }
        public ChoiceGang.GangData[] CheckMingGang(CardKind other)
        {
            List<CardKind[]> results = new List<CardKind[]>();
            var dividedKinds = cards
                .Where(_ => _.realValue == other.realValue)
                .GroupBy(_ => _.isHongBao)
                .Select(_ => (_.First(), _.Count()))
                .ToArray();
            if (dividedKinds.Length != 0)
            {
                MingPaiSelect(3, 0, dividedKinds, new List<CardKind>(), results);
            }
            return results.Select(_ => new ChoiceGang.GangData
            {
                kind = ChoiceGang.GangKind.AnGang,
                cards = _.Append(other).ToArray()
            }).ToArray();
        }
        public ChoiceGang.GangData[] CheckAnGang()
        {
            List<CardKind[]> results = new List<CardKind[]>();
            var over4kinds = cards.GroupBy(_ => _.realValue).Where(_ => _.Count() >= 4);
            over4kinds.Foreach((IGrouping<int, CardKind> _, int index) =>
            {
                var dividedKinds = _
                    .GroupBy(_ => _.isHongBao)
                    .Select(_ => (_.First(), _.Count()))
                    .ToArray();
                if (dividedKinds.Length != 0)
                {
                    MingPaiSelect(4, 0, dividedKinds, new List<CardKind>(), results);
                }
            });
            return results.Select(_ => new ChoiceGang.GangData
            {
                kind = ChoiceGang.GangKind.AnGang,
                cards = _
            }).ToArray();
        }
        public int CountCardNum(CardKind card)
        {
            return cards.Count(_ => _.huaseKind == card.huaseKind && _.huaseNum == card.huaseNum);
        }
    }
}