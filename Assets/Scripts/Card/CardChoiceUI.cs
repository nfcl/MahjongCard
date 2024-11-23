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

        public void Init(ClientCardTingPaiResult tingPai, bool isZhenTing)
        {
            face.sprite = DataManager.GetUICardFaceSprite(tingPai.tingPai);
            back.sprite = DataManager.handBackSprites[0];
            tagBackground.sprite = GameSceneUIManager.instance.gamePanel.cardChoicePanel.ChoiceStateSprite[(int)tingPai.state];
            tagText.text = tingPai.state == ClientCardTingPaiState.last ? tingPai.lastNum.ToString() : "";
            zhenTing.SetActive(isZhenTing);
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