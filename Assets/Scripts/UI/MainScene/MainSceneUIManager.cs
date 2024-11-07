using TMPro;
using UnityEngine;

namespace MainSceneUI
{
    public class MainSceneUIManager : MonoBehaviour
    {
        public static MainSceneUIManager instance;

        public LoginPanel   loginPanel;     //��¼����
        public LobbyPanel   lobbyPanel;     //��������
        public SearchPanel  searchPanel;    //��������
        public RoomPanel    roomPanel;      //�������

        public TMP_Text     userName;       //���½ǵ��û�����

        private void Awake()
        {
            instance = this;
        }
    }
}