﻿using Data;
using Mirror;

namespace GameLogic
{
    public interface IGameLogicManager
    {
        public static IGameLogicManager instance;
        /// <summary>
        /// 整局游戏开始
        /// </summary>
        [Server]
        public void GameStart();
        /// <summary>
        /// 获得离客户端玩家相对Index玩家的下标
        /// </summary>
        /// <returns></returns>
        public int GetRelativeplayerIndex(int index);
        /// <summary>
        /// 获得庄家相对客户端玩家的下标
        /// </summary>
        /// <returns></returns>
        public int GetZhuangPlayerIndex();
        /// <summary>
        /// 获得玩家的相对于客户端玩家的下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetAbsolutePlayerIndex(int index);
        /// <summary>
        /// 获得客户端玩家和其他玩家的距离
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int GetPlayerDistance(int other);
        /// <summary>
        /// 获得两个玩家间的距离
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public int GetPlayerDistance(int self, int other);
        /// <summary>
        /// 判断是不是宝牌
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public bool isBaoPai(CardKind kind);
        /// <summary>
        /// 客户端提交操作
        /// </summary>
        /// <param name="action"></param>
        public void SubmitAction(long uuid, Action action);
    }
}