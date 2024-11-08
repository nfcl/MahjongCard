using Data;
using Network;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainSceneUI
{
    public class RoomPanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public RoomPlayerInfoItem[] roomPlayerInfoItems;
        public Button readyButton;
        public TMP_Text readyButtonText;

        public enum ReadyButtonState
        {
            NotAllReady,
            AllReady,
            NotReady,
            Ready
        }
        public void SwitchReadyButtonState(ReadyButtonState state)
        {
            switch (state)
            {
                case ReadyButtonState.AllReady:
                    readyButton.interactable = true;
                    readyButton.image.color = new Color32(0, 255, 0, 255);
                    readyButtonText.text = "开始";
                    break;
                case ReadyButtonState.NotAllReady:
                    readyButton.interactable = false;
                    readyButton.image.color = new Color32(128, 128, 128, 255);
                    readyButtonText.text = "开始";
                    break;
                case ReadyButtonState.Ready:
                    readyButton.interactable = true;
                    readyButton.image.color = new Color32(255, 0, 0, 255);
                    readyButtonText.text = "取消";
                    break;
                case ReadyButtonState.NotReady:
                    readyButton.interactable = true;
                    readyButton.image.color = new Color32(0, 255, 0, 255);
                    readyButtonText.text = "准备";
                    break;
            }
        }
        public void ChangeSlotReadyState(string uuid, bool isReady)
        {
            RoomPlayerInfoItem item = roomPlayerInfoItems.Where(_ => _.uuid == uuid).FirstOrDefault();
            if(item == null)
            {
                Debug.LogException(new System.Exception("未知的房间成员UUID"));
            }
            else
            {
                item.ChangeReadyState(isReady);
            }
        }
        public void AddRoomPlayerItem(RoomPlayerInfo info)
        {
            roomPlayerInfoItems[info.roomSlotIndex].Init(info);
        }
        public void SyncAllPlayerInfos(RoomPlayerInfo[] infos)
        {
            foreach(RoomPlayerInfo info in infos)
            {
                roomPlayerInfoItems[info.roomSlotIndex].Init(info);
            }
        }
        public void RemovePlayer(string uuid)
        {
            roomPlayerInfoItems.Where(_ => _.uuid == uuid).FirstOrDefault()?.Clear();
        }
        public void Open(bool isHost)
        {
            SwitchReadyButtonState(isHost ? ReadyButtonState.NotAllReady : ReadyButtonState.NotReady);
            foreach(var item in roomPlayerInfoItems)
            {
                item.Clear();
            }
            canvasGroup.Open();
        }
        public void Close()
        {
            canvasGroup.Close();
        }
        public void Ready()
        {
            if (RoomPlayer.instance.isServer)
            {
                //开始游戏
            }
            else
            {
                //切换准备状态
                RoomPlayer.instance.CmdChangeRoomPlayerReadyState();
            }
        }
        public void Return()
        {
            if (RoomPlayer.instance.isServer)
            {
                NetworkRoomManager.instance.StopHost();
            }
            else
            { 
                NetworkRoomManager.instance.StopClient();
            }
        }
    }
}