using Data;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    public class FourPeopleLogicManager : NetworkBehaviour
    {
        public LogicPaiShan paiShan;
        public LogicPlayer[] players;
        public (FengKind feng, int ju, int chang) round;
        public int currentPlayerIndex;
        public LogicPlayer currentPlayer => players[currentPlayerIndex];
        public int liZhiNum;

        public Wait<Action> wait;

        public void NextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        }
        public Choice[] GetChoiceAfterDrawCard()
        {
            List<Choice> choices = new List<Choice>();
            //打牌
            choices.Add(ChoicePlayCard.NormalPlayCard());
            //立直TODO
            //暗杠或加杠
            if (paiShan.CanGang && currentPlayer.CheckDrawCardGang(out ChoiceGang choice))
            {
                choices.Add(choice);
            }
            //自摸TODO
            //流局（九种九牌）
            if (currentPlayer.drewCard == 14 && players.Count(_ => _.ming.Count() != 0) == 0)
            {
                choices.Add(new ChoiceJiuZhongJiuPai());
            }
            return choices.ToArray();
        }
        public Choice[] GetChoiceAfterPlayCard(LogicPlayer player, CardKind playedCard)
        {
            List<Choice> choices = new List<Choice>();
            //碰
            {
                if(player.CheckPeng(out ChoicePeng choice, player.playerIndex, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //吃
            {
                if (player.CheckChi(out ChoiceChi choice, player.playerIndex, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //明杠
            {
                if (paiShan.CanGang && player.CheckPlayCardGang(out ChoiceGang choice, player.playerIndex, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //荣和TODO
            return choices.ToArray();
        }

        /// <summary>
        /// 游戏开始事件
        /// </summary>
        public virtual void OnGameStart()
        {
            round = (FengKind.Dong, 1, 1);

            players = new LogicPlayer[4];
            for (int i = 0; i < players.Length; ++i)
            {
                players[i] = new LogicPlayer(i, 5, 20, 35000);
            }

            liZhiNum = 0;
        }
        /// <summary>
        /// 一轮游戏开始事件
        /// </summary>
        public virtual void OnGameRoundStart()
        {
            paiShan = new LogicPaiShan();

            for (int i = 0; i < players.Length; ++i)
            {
                players[i].RoundStart();
            }

            currentPlayerIndex = (int)round.feng;
        }
        /// <summary>
        /// 配牌事件
        /// </summary>
        public virtual void OnConfigurCard()
        {
            for (int i = 0; i < players.Length; ++i)
            {
                players[i].ConfigurCard(paiShan.GetConfigurCard());
            }
        }
        /// <summary>
        /// 玩家回合开始事件
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnPlayerRoundStart()
        {
            OnPlayerDrawCard(currentPlayer, paiShan.GetDrawCard());

            WaitPlayer<Action> waitPlayer = WaitPlayer<Action>.WaitForPlayerSelect(
                currentPlayer,
                new ActionPlayCard(currentPlayer.LastDrewCard)
            );
            wait = new Wait<Action>(
                new WaitPlayer<Action>[] { waitPlayer },
                _ =>
                {
                    wait = null;
                    ProcessPlayerSelect(_[0].Item1, _[0].Item2);
                }
            );
            wait.AlarmStartAll(this);
        }
        private void ProcessPlayerSelect(LogicPlayer player, Action action)
        {
            switch (action.kind)
            {
                case ActionKind.None:
                    {
                        break;
                    }
                case ActionKind.PlayCard:
                    {
                        ActionPlayCard resultAction = action as ActionPlayCard;
                        OnPlayerPlayCard(player, resultAction.card, false);
                        break;
                    }
            }
        }
        public virtual void OnPlayerDrawCard(LogicPlayer player, CardKind card) 
        {
            player.DrawCard(card);
        }
        /// <summary>
        /// 玩家打出牌
        /// </summary>
        public virtual void OnPlayerPlayCard(LogicPlayer player, CardKind card, bool isLiZhi)
        {
            player.PlayCard(card);
        }

        public virtual void OnPlayerRoundEnd(LogicPlayer player)
        {
            NextPlayer();
        }
    }
}