using Data;
using UnityEngine;
using UnityEngine.UI;

public class BaoPaiItem : MonoBehaviour
{
    public Image face;
    public Image back;

    public void Clear()
    {
        back.sprite = DataManager.handBackSprites[1];
    }
    public void ShowBaoPai(CardKind kind)
    {
        face.sprite = DataManager.GetUICardFaceSprite(kind);
        back.sprite = DataManager.handBackSprites[0];
    }
}
