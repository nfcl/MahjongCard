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
        public PropPanel    propPanel;      //��������

        public TMP_Text     userName;       //���½ǵ��û�����

        private void Awake()
        {
            instance = this;
        }
    }
}

public static class CanvasGroupExtension
{
    public static void Open(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }
    public static void Close(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}