using System.Collections.Generic;

namespace Data
{
    public class LogicPaiHe
    {
        private List<CardKind> cards;

        public LogicPaiHe()
        {
            cards = new List<CardKind>();
        }
        public void PlayCard(CardKind card)
        {
            cards.Add(card);
        }
        public void BeMingedCard(CardKind card)
        {
            cards.Remove(card);
        }
    }
}