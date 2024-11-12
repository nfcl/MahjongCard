using Data;
using GameLogic;
using GameSceneUI;
using Manager;
using UnityEngine;

namespace Card
{
    public class GamePanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public UserInfoItem[] userInfoItems;
        public HandCardUI handCard;

        public void SyncName(string[] messages)
        {
            for (int i = 0; i < messages.Length; ++i)
            {
                userInfoItems[i].nameText.text = messages[IGameLogicManager.instance.GetplayerIndex(i)];
            }
        }
        public void Open()
        {
            canvasGroup.Open();
        }
        public void Close()
        {
            canvasGroup.Close();
        }
    }
}