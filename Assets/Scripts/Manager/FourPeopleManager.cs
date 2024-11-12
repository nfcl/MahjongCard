using Data;
using GameLogic;
using GameSceneUI;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public class FourPeopleManager : FourPeopleLogicManager, IGameLogicManager
    {
        public int roomIndex;

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
        }
        [ClientRpc]
        public void RpcClientGameRoundStart()
        {
            DesktopManager.instance.ClearDesktop();
            GameSceneUIManager.instance.gamePanel.handCard.Clear();
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