using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public Image rayCastBackground;
    public TMP_Text content;
    public RectTransform contentScrollView;
    public Button[] buttons;

    public delegate void D_Void_Void();

    public enum ButtonStyle
    {
        Confirm     = 0b001,
        Cancel      = 0b010,
        Retry       = 0b100
    }

    public void Open(string contentString, int buttonStyle = (int)ButtonStyle.Confirm, bool isForce = true, D_Void_Void[] buttonCallback = null)
    {
        content.text = contentString;
        rayCastBackground.raycastTarget = isForce;
        if (buttonStyle == 0b000)
        {
            contentScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);
        }
        else
        {
            contentScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 430);
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].onClick.RemoveAllListeners();
                bool activeButton = ((buttonStyle >> i) & 0b1) == 0b1;
                buttons[i].gameObject.SetActive(activeButton);
                if (activeButton && buttonCallback != null && buttonCallback.Length > i)
                {
                    buttons[i].onClick.AddListener(delegate { buttonCallback[i]?.Invoke(); });
                }
            }
        }
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }
    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
