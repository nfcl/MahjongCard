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
    }
}