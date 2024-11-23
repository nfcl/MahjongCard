﻿using System.Collections.Generic;
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
            if (other.huaSe == HuaSe.Feng || other.huaSe == HuaSe.SanYuan)
            {
                return new CardKind[0][];
            }
            List<CardKind[]> results = new List<CardKind[]>();
            var closeKinds = cards
                .Where(_ => _.huaSe == other.huaSe && Mathf.Abs(_.realValue - other.realValue) <= 2)
                .GroupBy(_ => _.realValue)
                .ToList();
            closeKinds.Sort((x, y) => CardKind.LogicComparer.Compare(x.First(), y.First()));
            for (int i = 0; i < closeKinds.Count - 2; ++i)
            {
                if (closeKinds[i].First().realValue + 1 == closeKinds[i + 1].First().realValue
                    && closeKinds[i].First().realValue + 2 == closeKinds[i + 2].First().realValue)
                {
                    CardKind[][] dividedKinds =
                        new CardKind[][]
                        {
                            CardKind.Divider(closeKinds[i + 0]),
                            CardKind.Divider(closeKinds[i + 1]),
                            CardKind.Divider(closeKinds[i + 2])
                        };
                    int _ = 0, __ = 0, ___ = 0;
                    while (true)
                    {
                        results.Add(new CardKind[]
                        {
                            dividedKinds[000][_],
                            dividedKinds[01][__],
                            dividedKinds[2][___],
                        });
                        ___ += 1;
                        if (___ == dividedKinds[2].Length)
                        {
                            ___ = 0;
                            __ += 1;
                            if (__ == dividedKinds[1].Length)
                            {
                                __ = 0;
                                _ += 1;
                                if (_ == dividedKinds[0].Length)
                                {
                                    _ = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
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
            return results.ToArray();
        }
        public CardKind[][] CheckMingGang(CardKind other)
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
            return results.ToArray();
        }
        public CardKind[][] CheckAnGang()
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
            return results.ToArray();
        }
        public int CountCardNum(CardKind card)
        {
            return cards.Count(_ => _.huaseKind == card.huaseKind && _.huaseNum == card.huaseNum);
        }
    }
}