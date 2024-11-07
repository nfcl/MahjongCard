using TMPro;
using UnityEngine;

namespace MainSceneUI
{
    public class LoginPanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public TMP_InputField nameInput;
        
        public void Confirm()
        {
            if (DataManager.Login(nameInput.text))
            {
                MainSceneUIManager.instance.userName.text = DataManager.userInfo.name;
                MainSceneUIManager.instance.loginPanel.Close();
                MainSceneUIManager.instance.lobbyPanel.Open();
            }
            else
            {

            }
        }
        public void Open()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
        public void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }
}