using UnityEngine;

namespace GameSceneUI
{
    public class EnterPanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public RectTransform processBarBase;
        public RectTransform processBarValue;

        public float ProcessBarValue
        {
            set
            {
                processBarValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * processBarBase.rect.width);
            }
        }

        public void Open()
        {
            ProcessBarValue = 0f;
            canvasGroup.Open();
        }
        public void Close()
        {
            canvasGroup.Close();
        }
    }
}