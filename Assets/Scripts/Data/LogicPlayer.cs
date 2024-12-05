using Checker;
using System.Linq;

namespace Data
{
    public class LogicPlayer
    {

        public SelfInfo selfInfo;

        public int playerIndex;
        public LogicHandCard hand;
        public LogicPaiHe paiHe;
        public LogicMingPai ming;
        public float roundWaitTime;
        public float globalWaitTime;
        public float totalWaitTime => roundWaitTime + globalWaitTime;
        public CardKind LastDrewCard => hand.lastDrewCard;
        public int res;
        public bool[] zhenTingRecoder;

        public LogicPlayer(LogicPlayer other)
        {
            selfInfo = other.selfInfo;

            playerIndex = other.playerIndex;
            hand = new LogicHandCard(other.hand);
            paiHe = new LogicPaiHe(other.paiHe);
            ming = new LogicMingPai(other.ming);
            roundWaitTime = other.roundWaitTime;
            globalWaitTime = other.globalWaitTime;
            res = other.res;
            zhenTingRecoder = other.zhenTingRecoder.Clone() as bool[];
        }
        public LogicPlayer(int playerIndex, float roundWaitTime, float globalWaitTime, int res)
        {
            hand = null;
            paiHe = null;
            ming = null;
            zhenTingRecoder = null;

            this.playerIndex = playerIndex;
            this.roundWaitTime = roundWaitTime;
            this.globalWaitTime = globalWaitTime;
            this.res = res;
        }
        public void RoundStart(FengKind ziFeng, bool isZhuang)
        {
            selfInfo = new SelfInfo
            {
                ziFeng = ziFeng,
                drewCardNum = 0,
                hasYiFa = false,
                isLiangLiZhi = false,
                isLiZhi = false,
                isZhuang = isZhuang
            };
            hand = new LogicHandCard();
            paiHe = new LogicPaiHe();
            ming = new LogicMingPai();
            zhenTingRecoder = new bool[34];
        }
        public void ConfigurCard(CardKind[] cards)
        {
            hand.Init(cards);
        }
        public void DrawCard(CardKind card)
        {
            selfInfo.drewCardNum += 1;
            hand.DrawCard(card);
        }
        public void PlayCard(CardKind card)
        {
            hand.PlayCard(card);
            paiHe.PlayCard(card);
            zhenTingRecoder[card.huaseKind * 9 + card.huaseNum] = true;
        }
        public void MingMingPai(LogicPlayer otherPlayer, LogicMingPaiGroup group)
        {
            otherPlayer.paiHe.BeMingedCard(group.otherCard);
            hand.BeMingedCard(group);
            ming.MingCard(group);
        }
        public void AnMingPai(LogicMingPaiGroup group)
        {
            hand.BeMingedCard(group);
            ming.MingCard(group);
        }
        public void JiaGang(CardKind card)
        {
            hand.BeMingedCard(card);
            ming.JiaGang(card);
        }
        public bool CheckChi(out ChoiceChi choice, int fromPeople, CardKind other)
        {
            var result = hand.CheckChi(other);
            if(result.Length != 0)
            {
                choice = new ChoiceChi
                {
                    choices = result,
                    fromPeople = fromPeople,
                    fromPeopleCard = other
                };
                return true;
            }
            choice = null;
            return false;
        }
        public bool CheckPeng(out ChoicePeng choice, int fromPeople, CardKind other)
        {
            var result = hand.CheckPeng(other);
            if (result.Length != 0)
            {
                choice = new ChoicePeng
                {
                    choices = result,
                    fromPeople = fromPeople,
                    fromPeopleCard = other
                };
                return true;
            }
            choice = null;
            return false;
        }
        public bool CheckDrawCardGang(out ChoiceGang choice)
        {
            choice = ChoiceGang.Gang(hand.CheckAnGang().ToArray().Concat(ming.CheckJiaGang(hand.Cards)).ToArray());
            return choice.choices.Length != 0;
        }
        public bool CheckPlayCardGang(out ChoiceGang choice, CardKind other)
        {
            choice = ChoiceGang.Gang(hand.CheckMingGang(other).ToArray());
            return choice.choices.Length != 0;
        }
        public bool CheckJiuZhongJiuPai()
        {
            HandMatrix matrix = new HandMatrix(hand.Cards);
            (int, int)[] yaojius = new (int, int)[]
            {
                (0,0),(0,8),(1,0),(1,8),(2,0),(2,8),
                (3,0),(3,1),(3,2),(3,3),(3,4),(3,5),(3,6)
            };
            return yaojius.Count(_ => matrix[_.Item1, _.Item2] != 0) >= 9;
        }
        public bool CheckDrewCardCanGang(out ChoiceGang.GangData gang)
        {
            gang = new ChoiceGang.GangData();
            gang.cards = hand.Cards.Where(_ => CardKind.LogicEqualityComparer.Equals(_, hand.lastDrewCard)).ToArray();
            if (gang.cards.Length == 4)
            {
                gang.kind = ChoiceGang.GangKind.AnGang;
                return true;
            }
            gang = null;
            return false;
        }
    }
}
