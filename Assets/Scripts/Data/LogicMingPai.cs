using Checker;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class LogicMingPai
    {
        public List<LogicMingPaiGroup> groups;

        public LogicMingPaiGroup[] Groups => groups.ToArray();
        public bool isMingPai => groups.Count != 0;
        public bool isMenQianQing => groups.All(_ => _.kind == MingPaiKind.AnGang);

        public MingInfo mingInfo => new MingInfo
        {
            isMenQianQing = isMenQianQing,
            isMingPai = isMingPai
        };

        public LogicMingPai()
        {
            groups = new List<LogicMingPaiGroup>();
        }
        public LogicMingPai(LogicMingPai other)
        {
            groups = new List<LogicMingPaiGroup>(other.groups.Count);
            other.groups.Foreach((_, _index) =>
            {
                groups[_index] = new LogicMingPaiGroup(_)
            });
        }
        public void MingCard(LogicMingPaiGroup group)
        {
            groups.Add(group);
        }
        public void JiaGang(CardKind card)
        {
            groups
                .First(_ => _.kind == MingPaiKind.Peng && CardKind.LogicEqualityComparer.Equals(card, _.signKind, _.signNum))
                .JiaGang(card);
        }
        public int Count()
        {
            return groups.Count;
        }
        public ChoiceGang.GangData[] CheckJiaGang(CardKind[] cards)
        {
            return groups
                .Where(_ => _.kind == MingPaiKind.Peng && cards.Count(__ => __.realValue == _.otherCard.realValue) != 0)
                .Select(_ =>
                    new ChoiceGang.GangData
                    {
                        kind = ChoiceGang.GangKind.JiaGang,
                        cards = _
                            .GetCards()
                            .Append(cards.First(__ => CardKind.LogicEqualityComparer.Equals(__, _.signKind, _.signNum)))
                            .ToArray()
                    }
                )
                .ToArray();
        }
    }
}