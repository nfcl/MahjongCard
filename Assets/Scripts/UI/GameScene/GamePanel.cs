using Data;
using GameLogic;
using GameSceneUI;
using UnityEngine;
using UnityEngine.UI;

namespace Card
{
    public class GamePanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public UserInfoItem[] userInfoItems;
        public HandCardUI handCard;
        public Text liZhiNumText;
        public Text changGongNumText;
        public BaoPaiItem[] baoPaiItems;

        public void SyncName(string[] messages)
        {
            for (int i = 0; i < messages.Length; ++i)
            {
                userInfoItems[i].nameText.text = messages[IGameLogicManager.instance.GetRelativeplayerIndex(i)];
            }
        }
        public void SyncChang(int chang)
        {
            changGongNumText.text = (chang - 1).ToString();
        }
        public void SyncLiZhiNum(int liZhiNum)
        {
            liZhiNumText.text = liZhiNum.ToString();
        }
        public void SyncBaoPai(CardKind[] baoPais)
        {
            for (int i = 0; i < baoPais.Length; ++i)
            {
                baoPaiItems[i].ShowBaoPai(baoPais[i]);
            }
            for (int i = baoPais.Length; i < baoPaiItems.Length; ++i)
            {
                baoPaiItems[i].Clear();
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