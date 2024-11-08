using Network;
using UnityEngine;

namespace MainSceneUI
{
    public class LobbyPanel : MonoBehaviour
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
        public void CreateRoom()
        {
            try
            {
                MainSceneUIManager.instance.propPanel.Wait("创建房间ing...");
                DataManager.OnServerInit();
                NetworkRoomManager.instance.StartHost(
                    delegate
                    {
                        MainSceneUIManager.instance.lobbyPanel.Close();
                        MainSceneUIManager.instance.propPanel.Close();
                        MainSceneUIManager.instance.roomPanel.Open();
                    },
                    delegate
                    {
                        MainSceneUIManager.instance.roomPanel.Close();
                        MainSceneUIManager.instance.lobbyPanel.Open();
                    }
                );
            }
            catch (System.Exception e)
            {
                NetworkRoomManager.instance.StopHost();
                MainSceneUIManager.instance.propPanel.Log(e.Message);
                Debug.LogException(e);
            }
        }
        public void SearchRoom()
        {
            MainSceneUIManager.instance.lobbyPanel.Close();
            MainSceneUIManager.instance.searchPanel.Open();
        }
    }
}