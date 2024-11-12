using Data;
using GameLogic;
using GameSceneUI;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;

namespace Manager
{
    public class FourPeopleManager : FourPeopleLogicManager
    {
        public int roomIndex;

        private void Awake()
        {
            IGameLogicManager.instance = this;
        }

        public override int GetplayerIndex(int index)
        {
            return (roomIndex + index) % DataManager.playerNum;
        }
        public override int GetZhuangPlayerIndex()
        {
            return DataManager.playerNum - roomIndex;
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
            Debug.Log($"ZhuangIndex : {GetZhuangPlayerIndex()}");
            DesktopManager.instance.SyncRound(round, ju);
        }

        #endregion

        #region 总的游戏开始，玩家刚进入场景

        [Server]
        public override void GameStart()
        {
            OnGameStart();
        }
        [Server]
        public override void OnGameStart()
        {
            base.OnGameStart();

            RpcSyncIndex(DataManager.roomIndexToPlayers.Select((_) => _.uuid).ToArray());
            RpcSyncName(DataManager.roomIndexToPlayers.Select((_) => _.name).ToArray());
            RpcSyncScore(players.Select((_) => _.res).ToArray());
            RpcSyncRound(round.feng, round.ju, round.chang);

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
            base.OnGameRoundStart();
        }
        [Server]
        public override void OnGameRoundStart()
        {
            base.OnGameRoundStart();


        }
        [ClientRpc]
        public void RpcClientGameRoundStart()
        {

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
}