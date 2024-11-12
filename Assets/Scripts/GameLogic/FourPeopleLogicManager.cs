using Data;
using Mirror;
using System.Collections;
using UnityEngine;

namespace GameLogic
{
    public class FourPeopleLogicManager : NetworkBehaviour, IGameLogicManager
    {
        public LogicPaiShan paiShan;
        public LogicPlayer[] players;
        public (FengKind feng, int ju, int chang) round;
        public int currentPlayerIndex;
        public LogicPlayer currentPlayer => players[currentPlayerIndex];

        public Wait<Action> wait;

        public virtual void GameStart()
        {
            throw new System.NotImplementedException();
        }
        public virtual int GetplayerIndex(int index)
        {
            throw new System.NotImplementedException();
        }
        public virtual int GetZhuangPlayerIndex()
        {
            throw new System.NotImplementedException();
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
                players[i] = new LogicPlayer(5, 20, 35000);
            }
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
                    ProcessPlayerSelect(currentPlayer, _[0]);
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
                        OnPlayerPlayCard(player, resultAction.card);
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
        public virtual void OnPlayerPlayCard(LogicPlayer player, CardKind card)
        {
            player.PlayCard(card);
        }
    }
}