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
using UnityEditor;
using MainSceneUI;

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
        public void SubmitAction(long uuid, Data.Action action)
        {
            CmdSubmitAction(roomIndex, uuid, action);
        }
        public int GetPlayerDistance(int other)
        {
            return GetPlayerDistance(roomIndex, other);
        }
        public int GetPlayerDistance(int self, int other)
        {
            return (other + DataManager.playerNum - self) % DataManager.playerNum;
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
                DesktopManager.instance.handCards[GetAbsolutePlayerIndex(index)].ConfigurCard(_.cards);
            });
        }
        [ClientRpc]
        public void RpcSyncBaoPai(CardsMessage cards)
        {
            Debug.Log($"\n宝牌 : {cards}");

            foreach(var card in cards.cards)
            {
                baoPaiList[baoPaiMap[card.value]] = true;
            }

            GameSceneUIManager.instance.gamePanel.SyncBaoPai(cards.cards);
        }
        [ClientRpc]
        public void RpcSyncLastCardNum(int lastCardNum)
        {
            DesktopManager.instance.lastCardText.text = lastCardNum.ToString("00");
        }

        #endregion

        #region 玩家操作

        public override void OnSendPlayerChoice(LogicPlayer player, long uuid, Choice[] choices, bool isDrawCard)
        {
            base.OnSendPlayerChoice(player, uuid, choices, isDrawCard);

            TargetSendPlayerChoice(
                DataManager.GetRoomPlayerConnection(player.playerIndex),
                uuid,
                player.roundWaitTime,
                player.globalWaitTime,
                choices,
                isDrawCard
            );
        }
        [TargetRpc]
        public void TargetSendPlayerChoice(NetworkConnectionToClient connection, long uuid, float roundTime, float globalTime, Choice[] choices, bool isDrawCard)
        {
            Debug.Log(Choice.ToString(choices));
            GameSceneUIManager.instance.gamePanel.InitChoices(uuid, choices, isDrawCard);
            Debug.Log($"倒计时{roundTime}+{globalTime}秒启动");
            GameSceneUIManager.instance.gamePanel.SetAlarm(roundTime, globalTime);
        }
        [Command(requiresAuthority = false)]
        public void CmdSubmitAction(int playerIndex, long uuid, Data.Action action)
        {
            if(uuid != wait.uuid)
            {
                Debug.Log($"玩家{playerIndex} : 未通过UUID检测");
            }
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
                        var result = wait.PlayerComplete(players[playerIndex], action);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.Skip:
                    {
                        actionString.Append("跳过");
                        var result = wait.PlayerComplete(players[playerIndex], action as ActionSkip);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.LiZhi:
                    {
                        ActionLiZhi total = action as ActionLiZhi;
                        actionString.Append($"立直 {total.card}");
                        var result = wait.PlayerComplete(players[playerIndex], total);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.Gang:
                    {
                        ActionGang total = action as ActionGang;
                        actionString.Append($"{(playerChoices.First(_ => _.player.playerIndex == playerIndex).choices.First(_ => _.kind == ChoiceKind.Gang) as ChoiceGang).choices[total.index].kind}");
                        var result = wait.PlayerComplete(players[playerIndex], total);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.Chi:
                    {
                        actionString.Append("吃");
                        var result = wait.PlayerComplete(players[playerIndex], action as ActionChi);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.Peng:
                    {
                        actionString.Append("碰");
                        var result = wait.PlayerComplete(players[playerIndex], action as ActionPeng);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.JiuZhongJiuPai:
                    {
                        actionString.Append("九种九牌");
                        var result = wait.PlayerComplete(players[playerIndex], action as ActionJiuZhongJiuPai);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.ZiMo:
                    {
                        actionString.Append("自摸");
                        var result = wait.PlayerComplete(players[playerIndex], action as ActionZimo);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
                case ActionKind.RongHe:
                    {
                        actionString.Append("荣和");
                        var result = wait.PlayerComplete(players[playerIndex], action as ActionRongHe);
                        if (!result.result)
                        {
                            actionString.Append($"\n\t失败 : {result.reason}");
                        }
                        break;
                    }
            }
            Debug.Log(actionString.ToString());
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

            RpcSyncLastCardNum(paiShan.LastDrawCardCount);

            RpcSyncConfigurCard(cards);

            DOTween.Sequence()
                .AppendInterval(3f)
                .AppendCallback(() => OnPlayerRoundStart(players[0], true));
        }

        #endregion

        #region 玩家回合

        [Server]
        public override void OnPlayerRoundStart(LogicPlayer player, bool needDrawCard)
        {
            RpcOnPlayerRound(currentPlayerIndex);

            base.OnPlayerRoundStart(player, needDrawCard);
        }
        [ClientRpc]
        public void RpcOnPlayerRound(int playerIndex)
        {
            DesktopManager.instance.OnPlayerRound(playerIndex);
        }

        #endregion

        #region 玩家抽牌

        public override void OnPlayerDrawCard(LogicPlayer player, CardKind card, bool isLingShang)
        {
            base.OnPlayerDrawCard(player, card, isLingShang);

            RpcPlayerDrawCard(
                new DrawCardMessage
                {
                    card = card,
                    playerIndex = player.playerIndex
                }
            );

            AfterPlayerDrawCard(player, card, isLingShang);
        }
        public override void AfterPlayerDrawCard(LogicPlayer player, CardKind card, bool isLingShang)
        {
            base.AfterPlayerDrawCard(player, card, isLingShang);

            RpcSyncLastCardNum(paiShan.LastDrawCardCount);
        }
        [ClientRpc]
        public void RpcPlayerDrawCard(DrawCardMessage message)
        {
            if (message.playerIndex == roomIndex)
            {
                GameSceneUIManager.instance.gamePanel.handCard.DrawCard(message.card, true);
            }
            DesktopManager.instance.handCards[GetAbsolutePlayerIndex(message.playerIndex)].DrawCard(message.card);
        }

        #endregion

        #region 玩家打牌

        [Server]
        public override void OnPlayerPlayCard(LogicPlayer player, CardKind card, bool isLiZhi)
        {
            base.OnPlayerPlayCard(player, card, isLiZhi);

            RpcPlayerPlayCard(player.playerIndex, card, isLiZhi);

            base.AfterPlayerPlayCard(player, card, isLiZhi);
        }
        [ClientRpc]
        public void RpcPlayerPlayCard(int playerIndex, CardKind card, bool isLiZhi)
        {
            if(roomIndex == playerIndex)
            {
                GameSceneUIManager.instance.gamePanel.handCard.PlayCard(card);
            }
            if (isLiZhi)
            {
                GameSceneUIManager.instance.gamePanel.ShowActionSprite(playerIndex, ChoiceKind.LiZhi);
            }
            DesktopManager.instance.OnPlayerPlayCard(playerIndex, card, isLiZhi);
        }

        #endregion

        #region 玩家鸣牌

        public override void OnPlayerMingCard(LogicPlayer player, MingPaiKind kind, CardKind[] cards)
        {
            base.OnPlayerMingCard(player, kind, cards);

            LogicMingPaiGroup group = player.ming.groups.Last();

            if (kind == MingPaiKind.AnGang || kind == MingPaiKind.BaBei)
            {//暗杠，拔北
                RpcMingPai(player.playerIndex, group.kind, group.selfCard);
            }
            else
            {//吃，碰，明杠，加杠
                RpcMingPai(player.playerIndex, currentPlayerIndex, group.kind, group.selfCard, group.otherCard);
            }

            OnPlayerRoundStart(player, player.playerIndex == currentPlayerIndex);
        }
        [ClientRpc]
        public void RpcMingPai(int mingPlayerIndex, MingPaiKind kind, CardKind[] selfCards)
        {
            if (mingPlayerIndex == roomIndex)
            {
                //自己鸣牌需要额外处理UI手牌
                GameSceneUIManager.instance.gamePanel.handCard.MingCard(selfCards);
            }
            if (kind == MingPaiKind.BaBei)
            {
                DesktopManager.instance.BaBei(mingPlayerIndex);
                GameSceneUIManager.instance.gamePanel.ShowActionSprite(mingPlayerIndex, ChoiceKind.BaBei);
            }
            else if (kind == MingPaiKind.AnGang)
            {
                DesktopManager.instance.AnMingPai(kind, mingPlayerIndex, selfCards);
                GameSceneUIManager.instance.gamePanel.ShowActionSprite(mingPlayerIndex, ChoiceKind.Gang);
            }
            else
            {
                throw new Exception($"不被允许的鸣牌类型 {kind}");
            }
        }
        [ClientRpc]
        public void RpcMingPai(int mingPlayerIndex, int mingedPlayerIndex, MingPaiKind kind, CardKind[] selfCards, CardKind otherCard)
        {
            if (mingPlayerIndex == roomIndex)
            {
                //自己鸣牌需要额外处理UI手牌
                if(kind == MingPaiKind.JiaGang)
                {
                    GameSceneUIManager.instance.gamePanel.handCard.MingCard(selfCards[2]);
                }
                else
                {
                    GameSceneUIManager.instance.gamePanel.handCard.MingCard(selfCards);
                }
            }
            if (kind == MingPaiKind.JiaGang)
            {
                DesktopManager.instance.JiaGang(mingPlayerIndex, selfCards[2]);
                GameSceneUIManager.instance.gamePanel.ShowActionSprite(mingPlayerIndex, ChoiceKind.Gang);
            }
            else if (kind == MingPaiKind.Peng || kind == MingPaiKind.Chi || kind == MingPaiKind.MingGang)
            {
                DesktopManager.instance.MingMingPai(kind, mingPlayerIndex, mingedPlayerIndex, otherCard, selfCards);
                GameSceneUIManager.instance.gamePanel.ShowActionSprite(mingPlayerIndex, kind switch
                {
                    MingPaiKind.Peng => ChoiceKind.Peng,
                    MingPaiKind.Chi => ChoiceKind.Chi,
                    MingPaiKind.MingGang => ChoiceKind.Gang,
                    _ => throw new NotImplementedException()
                });
            }
            else
            {
                throw new Exception($"不被允许的鸣牌类型 {kind}");
            }
        }

        #endregion

        #region 玩家回合结束

        public override void OnPlayerRoundEnd(LogicPlayer player)
        {
            base.OnPlayerRoundEnd(player);
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