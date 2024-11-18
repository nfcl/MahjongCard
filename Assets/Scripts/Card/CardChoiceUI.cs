using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameSceneUI
{
    public class CardChoiceUI : MonoBehaviour
    {
        public Image face;
        public Image back;
        public Image tagBackground;
        public TMP_Text tagText;
        public GameObject zhenTing;

        public void Init(TingPai tingpai)
        {
            face.sprite = DataManager.GetUICardFaceSprite(tingpai.card);
            back.sprite = DataManager.handBackSprites[0];
            tagBackground.sprite = GameSceneUIManager.instance.gamePanel.cardChoicePanel.ChoiceStateSprite[(int)tingpai.GetState()];
            tagText.text = tingpai.GetState() == CardChoicePanel.ChoiceCardState.LastNum ? tingpai.lastNum.ToString() : "";
            zhenTing.SetActive(tingpai.isZhenTing);
        }
        public void Init(CardKind mingpai)
        {
            face.sprite = DataManager.GetUICardFaceSprite(mingpai);
            back.sprite = DataManager.handBackSprites[0];
            tagBackground.transform.parent.gameObject.SetActive(false);
            zhenTing.SetActive(false);
        }
    }
}