using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class LogicMingPai
    {
        public List<LogicMingPaiGroup> groups;

        public bool isMingPai => groups.All(_ => _.kind == MingPaiKind.AnGang);

        public LogicMingPai()
        {
            groups = new List<LogicMingPaiGroup>();
        }
        public void MingCard(LogicMingPaiGroup group)
        {
            groups.Add(group);
        }
        public int Count()
        {
            return groups.Count;
        }
        public CardKind[][] CheckJiaGang(CardKind[] cards)
        {
            return groups
                .Where(_ => _.kind == MingPaiKind.Peng && cards.Count(__ => __.realValue == _.otherCard.realValue) != 0)
                .Select(_ => _.GetCards())
                .ToArray();
        }
    }
}