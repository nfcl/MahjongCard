namespace Data
{
    public class LogicPlayer
    {
        public LogicHandCard hand;
        public LogicPaiHe paiHe;
        public LogicMingPai ming;
        public float roundWaitTime;
        public float globalWaitTime;
        public float totalWaitTime => roundWaitTime + globalWaitTime;
        public CardKind LastDrewCard => hand.lastDrewCard;
        public int res;

        public LogicPlayer(float roundWaitTime, float globalWaitTime, int res)
        {
            hand = null;
            paiHe = null;
            ming = null;

            this.roundWaitTime = roundWaitTime;
            this.globalWaitTime = globalWaitTime;
            this.res = res;
        }
        public void RoundStart()
        {
            hand = new LogicHandCard();
            paiHe = new LogicPaiHe();
            ming = new LogicMingPai();
        }
        public void ConfigurCard(CardKind[] cards)
        {
            hand.Init(cards);
        }
        public void DrawCard(CardKind card)
        {
            hand.DrawCard(card); 
        }
        public void PlayCard(CardKind card)
        {
            hand.PlayCard(card);
            paiHe.PlayCard(card);
        }
        public void Peng(LogicPlayer otherPlayer, LogicMingPaiGroup group)
        {
            otherPlayer.paiHe.BeMingedCard(group.otherCard);
            hand.BeMingedCard(group);
            ming.MingCard(group);
        }
        public void Chi(LogicPlayer otherPlayer, LogicMingPaiGroup group)
        {
            otherPlayer.paiHe.BeMingedCard(group.otherCard);
            hand.BeMingedCard(group);
            ming.MingCard(group);
        }
        public void MingGang(LogicPlayer otherPlayer, LogicMingPaiGroup group)
        {
            otherPlayer.paiHe.BeMingedCard(group.otherCard);
            hand.BeMingedCard(group);
            ming.MingCard(group);
        }
        public void AnGang(LogicMingPaiGroup group)
        {
            hand.BeMingedCard(group);
            ming.MingCard(group);
        }
        public void BaBei(LogicMingPaiGroup group)
        {
            hand.BeMingedCard(group);
            ming.MingCard(group);
        }
    }
}