using Network;
using UnityEngine;

namespace MainSceneUI
{
    public class RoomPanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public void Open()
        {
            canvasGroup.Open();
        }
        public void Close()
        {
            canvasGroup.Close();
        }
        public void Ready()
        {

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