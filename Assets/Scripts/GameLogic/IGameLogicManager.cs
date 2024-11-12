namespace GameLogic
{
    public interface IGameLogicManager
    {
        public static IGameLogicManager instance;
        /// <summary>
        /// 整局游戏开始
        /// </summary>
        public void GameStart();
        /// <summary>
        /// 获得玩家的相对于客户端玩家的下标（方便UI）
        /// </summary>
        /// <returns></returns>
        public int GetplayerIndex(int index);
        /// <summary>
        /// 获得庄家相对客户端玩家的下标
        /// </summary>
        /// <returns></returns>
        public int GetZhuangPlayerIndex();
    }
}