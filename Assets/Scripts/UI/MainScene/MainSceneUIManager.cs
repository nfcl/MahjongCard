using TMPro;
using UnityEngine;

namespace MainSceneUI
{
    public class MainSceneUIManager : MonoBehaviour
    {
        public static MainSceneUIManager instance;

        public LoginPanel   loginPanel;     //登录界面
        public LobbyPanel   lobbyPanel;     //大厅界面
        public SearchPanel  searchPanel;    //搜索界面
        public RoomPanel    roomPanel;      //房间界面

        public TMP_Text     userName;       //左下角的用户名字

        private void Awake()
        {
            instance = this;
        }
    }
}