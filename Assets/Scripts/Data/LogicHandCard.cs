using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class LogicHandCard
    {
        private List<CardKind> cards;
        public CardKind[] Cards => cards.ToArray();
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
            foreach(CardKind card in group.selfCard)
            {
                cards.Remove(card);
            }
        }
    }
}