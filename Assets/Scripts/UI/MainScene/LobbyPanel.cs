using UnityEngine;

namespace MainSceneUI
{
    public class LobbyPanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

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