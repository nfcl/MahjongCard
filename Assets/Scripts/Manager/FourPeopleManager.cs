using Data;
using GameLogic;
using GameSceneUI;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Message;
using DG.Tweening;
using System.Text;

namespace Manager
{
    public class FourPeopleManager : FourPeopleLogicManager, IGameLogicManager
    {
        public int roomIndex;
        private bool[] baoPaiList;
        private int[] baoPaiMap = new int[]
        {
             6, 2, 3, 4, 5, 6, 7, 8, 9, 1,
            16,12,13,14,15,16,17,18,19,11,
            26,22,23,24,25,26,27,28,29,21,
            31,32,33,30,35,36,34
        };
        private void Awake()
        {
            IGameLogicManager.instance = this;
        }

        public int GetRelativeplayerIndex(int index)
        {
            return (roomIndex + index) % DataManager.playerNum;
        }
        public int GetZhuangPlayerIndex()
        {
            return round.ju - 1;
        }
        public int GetAbsolutePlayerIndex(int index)
        {
            return (DataManager.playerNum + index - roomIndex) % DataManager.playerNum;
        }
        public bool isBaoPai(CardKind kind)
        {
            return baoPaiList[kind.value];
        }
        public void SubmitAction(Data.Action action)
        {
            CmdSubmitAction(roomIndex, action);
        }
        [Command(requiresAuthority = false)]
        public void CmdSubmitAction(int playerIndex, Data.Action action)
        {
            StringBuilder actionString = new StringBuilder();
            actionString.Append($"玩家{playerIndex} : ");
            switch (action.kind)
            {
                case ActionKind.None:
                    {
                        actionString.Append("无操作");
                        break;
                    }
                case ActionKind.PlayCard:
                    {
                        ActionPlayCard total = action as ActionPlayCard;
                        actionString.Append($"打出{total.card}");
                        break;
                    }
            }
            Debug.Log(actionString.ToString());
        }

        #region 信息同步

        [ClientRpc]
        public void RpcSyncIndex(string[] messages)
        {
            string myUuid = DataManager.userInfo.uuid;
            roomIndex = messages.FindIndex(_ => _ == myUuid);
            Debug.Log($"MyRoomIndex : {roomIndex}");
        }
        [ClientRpc]
        public void RpcSyncName(string[] messages)
        {
            GameSceneUIManager.instance.gamePanel.SyncName(messages);
        }
        [ClientRpc]
        public void RpcSyncScore(int[] messages)
        {
            DesktopManager.instance.SyncScore(messages);
        }
        /// <summary>
        /// 东二局3本场 => round = FengKind.Dong, ju = 2, chang = 3
        /// </summary>
        /// <param name="round">什么风</param>
        /// <param name="ju">几局</param>
        /// <param name="chang">几本场</param>
        [ClientRpc]
        public void RpcSyncRound(FengKind round, int ju, int chang)
        {
            if (!isServer)
            {
                base.round = (round, ju, chang);
            }
            DesktopManager.instance.SyncRound(base.round.feng, base.round.ju);
            GameSceneUIManager.instance.gamePanel.SyncChang(base.round.chang);
            DesktopManager.instance.OnPlayerRound(base.round.ju - 1);
            Debug.Log($"ZhuangIndex : {GetZhuangPlayerIndex()}");
        }
        [ClientRpc]
        public void RpcSyncLiZhi(int liZhi)
        {
            GameSceneUIManager.instance.gamePanel.SyncLiZhiNum(liZhi);
        }
        [ClientRpc]
        public void RpcSyncConfigurCard(CardsMessage[] cards)
        {
            Debug.Log($"\n{CardsMessage.ToString(cards)}");
            cards.Foreach((_, index) =>
            {
                if (index == roomIndex)
                {
                    GameSceneUIManager.instance.gamePanel.handCard.ConfigurInitialHandCard(_.cards);
                }
                else
                {
                    DesktopManager.instance.handCards[GetAbsolutePlayerIndex(index)].ConfigurCard(_.cards);
                }
            });
        }
        [ClientRpc]
        public void RpcSyncBaoPai(CardsMessage cards)
        {
            Debug.Log($"\n包牌 : {cards}");

            foreach(var card in cards.cards)
            {
                baoPaiList[baoPaiMap[card.value]] = true;
            }

            GameSceneUIManager.instance.gamePanel.SyncBaoPai(cards.cards);
        }

        #endregion

        #region 总的游戏开始，玩家刚进入场景

        [Server]
        public void GameStart()
        {
            OnGameStart();
        }
        [Server]
        public override void OnGameStart()
        {
            base.OnGameStart();

            RpcSyncIndex(DataManager.roomIndexToPlayers.Select((_) => _.uuid).ToArray());
            RpcSyncName(DataManager.roomIndexToPlayers.Select((_) => _.name).ToArray());

            RpcClientGameStart();

            GameRoundStart();
        }
        [ClientRpc]
        public void RpcClientGameStart()
        {
            Debug.Log("游戏开始");

            GameSceneUIManager.instance.enterPanel.Close();
            GameSceneUIManager.instance.gamePanel.Open();
        }

        #endregion

        #region 一轮游戏开始

        [Server]
        public void GameRoundStart()
        {
            OnGameRoundStart();
        }
        [Server]
        public override void OnGameRoundStart()
        {
            base.OnGameRoundStart();

            RpcSyncScore(players.Select((_) => _.res).ToArray());
            RpcSyncRound(round.feng, round.ju, round.chang);
            RpcSyncLiZhi(liZhiNum);

            RpcClientGameRoundStart();

            ConfigurCards();
        }
        [ClientRpc]
        public void RpcClientGameRoundStart()
        {
            DesktopManager.instance.ClearDesktop();
            GameSceneUIManager.instance.gamePanel.handCard.Clear();

            baoPaiList = new bool[37];
            baoPaiList[00] = true;
            baoPaiList[10] = true;
            baoPaiList[20] = true;
        }

        #endregion

        #region 玩家配牌

        [Server]
        public void ConfigurCards()
        {
            OnConfigurCard();
        }
        [Server]
        public override void OnConfigurCard()
        {
            base.OnConfigurCard();
            
            CardsMessage[] cards = base.players.Select((_) => new CardsMessage { cards = _.hand.Cards }).ToArray();

            Debug.Log(CardsMessage.ToString(cards));

            RpcSyncBaoPai(new CardsMessage(paiShan.GetBaoPai()));

            RpcSyncConfigurCard(cards);

            DOTween.Sequence()
                .AppendInterval(3f)
                .AppendCallback(() => OnPlayerRoundStart());
        }

        #endregion

        #region 玩家回合

        [Server]
        public override void OnPlayerRoundStart()
        {
            base.OnPlayerRoundStart();

            RpcPlayerDrawCard(
                new DrawCardMessage
                {
                    card = currentPlayer.LastDrewCard,
                    playerIndex = currentPlayerIndex
                }
            );
            TargetGivePlayerChoices(
                DataManager.GetRoomPlayerConnection(currentPlayerIndex),
                wait.uuid,
                currentPlayer.roundWaitTime, 
                currentPlayer.globalWaitTime,
                new Choice[]
                {
                    ChoicePlayCard.NormalPlayCard()
                }
            );
        }
        [ClientRpc]
        public void RpcPlayerDrawCard(DrawCardMessage message)
        {
            if (message.playerIndex == roomIndex)
            {
                GameSceneUIManager.instance.gamePanel.handCard.DrawCard(message.card, true);
            }
            else
            {
                DesktopManager.instance.handCards[GetAbsolutePlayerIndex(message.playerIndex)].DrawCard(message.card);
            }
        }
        [TargetRpc]
        public void TargetGivePlayerChoices(NetworkConnectionToClient connection, long uuid, float roundTime, float globalTime, Choice[] choices)
        {
            GameSceneUIManager.instance.gamePanel.InitChoices(uuid, choices);
            Debug.Log($"倒计时{roundTime}+{globalTime}秒启动");
            GameSceneUIManager.instance.gamePanel.SetAlarm(roundTime, globalTime);
        }

        #endregion

    }
}

public static class LinqExtension
{
    public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
    {
        int index = 0;
        foreach (var item in items)
        {
            if (predicate(item)) break;
            index += 1;
        }
        return index;
    }
    public static void Foreach<T>(this IEnumerable<T> items, Action<T, int> action)
    {
        int index = 0;
        foreach (var item in items)
        {
            action(item, index);
            index += 1;
        }
    }
}