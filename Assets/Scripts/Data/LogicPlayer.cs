using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class LogicPlayer
    {
        public int playerIndex;
        public LogicHandCard hand;
        public LogicPaiHe paiHe;
        public LogicMingPai ming;
        public float roundWaitTime;
        public float globalWaitTime;
        public float totalWaitTime => roundWaitTime + globalWaitTime;
        public CardKind LastDrewCard => hand.lastDrewCard;
        public int res;
        public int drewCard;

        public LogicPlayer(int playerIndex, float roundWaitTime, float globalWaitTime, int res)
        {
            hand = null;
            paiHe = null;
            ming = null;

            this.playerIndex = playerIndex;
            this.roundWaitTime = roundWaitTime;
            this.globalWaitTime = globalWaitTime;
            this.res = res;
            this.drewCard = 0;
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
            drewCard = cards.Length;
        }
        public void DrawCard(CardKind card)
        {
            hand.DrawCard(card);
            drewCard += 1;
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
            choice = ChoiceGang.ChoiceDrawCardGang(hand.CheckAnGang().ToArray(), ming.CheckJiaGang(hand.Cards));
            return choice.choices.Length + choice.jiaGang.Length != 0;
        }
        public bool CheckPlayCardGang(out ChoiceGang choice,int fromPeople, CardKind other)
        {
            choice = ChoiceGang.ChoicePlayCardGang(fromPeople, hand.CheckMingGang(other).ToArray());
            return choice.choices.Length != 0;
        }
        public void CheckTingPai()
        {
            #region 可在手牌中检测
            //断幺九
            //三元
            //门前清自摸和 
            //一杯口
            //对对和
            //三暗刻
            //三杠子
            //三色同刻
            //混老头
            //小三元
            //三色同顺
            //一气贯通
            //混全带幺九
            //七对子
            //混一色
            //纯全带幺九
            //二杯口
            //清一色
            //大三元
            //小四喜
            //大四喜
            //字一色
            //绿一色
            //清老头
            //国士无双
            //国士无双13面
            //四暗刻
            //四暗刻单骑
            //九莲宝灯
            //纯正九莲宝灯
            //四杠子

            #endregion

            #region 需搭配对局信息

            //自风
            //场风
            //河底摸鱼
            //岭上开花
            //抢杠
            //海底捞月
            //立直
            //一发
            //平和
            //两立直
            //流局满贯
            //天和
            //地和

            #endregion
        }
    }
}
