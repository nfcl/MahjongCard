using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Data
{
    public class LogicPaiHe
    {
        private List<CardKind> cards;

        public LogicPaiHe()
        {
            cards = new List<CardKind>();
        }
        public LogicPaiHe(LogicPaiHe other)
        {
            cards = (cards.ToArray().Clone() as CardKind[]).ToList();
        }
        public void PlayCard(CardKind card)
        {
            cards.Add(card);
        }
        public void BeMingedCard(CardKind card)
        {
            cards.Remove(card);
        }
        public int CountCardNum(CardKind card)
        {
            return cards.Count(_ => _.huaseKind == card.huaseKind && _.huaseNum == card.huaseNum);
        }
    }
}