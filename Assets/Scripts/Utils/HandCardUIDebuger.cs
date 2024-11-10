//扩展ToggleTest类在Inspector面板的显示内容
using UnityEditor;
using UnityEngine;

public class HandCardUIDebuger : MonoBehaviour
{
    public RectTransform[] cards;
    public Vector3 startPosistion;
    public Vector3 distance;

    public void FormatCards()
    {
        cards = new RectTransform[transform.childCount];
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }
        Vector3 temp = startPosistion;
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].localPosition = temp;
            temp += distance;
        }
    }
}