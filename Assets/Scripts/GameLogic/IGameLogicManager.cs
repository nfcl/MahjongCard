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
    }
}