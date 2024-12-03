using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameSceneUI
{
    public class UserInfoItem : MonoBehaviour
    {
        public TMP_Text nameText;
        public Image actionSprite;

        private void Awake()
        {
            actionSprite.gameObject.SetActive(false);
        }

        public void ShowActionSprite(ChoiceKind kind)
        {
            switch (kind)
            {
                case ChoiceKind.LiZhi:
                    actionSprite.sprite = GameSceneUIManager.instance.gamePanel.ActionSprite[4];
                    break;
                case ChoiceKind.Gang:
                    actionSprite.sprite = GameSceneUIManager.instance.gamePanel.ActionSprite[2];
                    break;
                case ChoiceKind.Peng:
                    actionSprite.sprite = GameSceneUIManager.instance.gamePanel.ActionSprite[7];
                    break;
                case ChoiceKind.Chi:
                    actionSprite.sprite = GameSceneUIManager.instance.gamePanel.ActionSprite[1];
                    break;
                case ChoiceKind.ZiMo:
                    actionSprite.sprite = GameSceneUIManager.instance.gamePanel.ActionSprite[10];
                    break;
                case ChoiceKind.RongHe:
                    actionSprite.sprite = GameSceneUIManager.instance.gamePanel.ActionSprite[9];
                    break;
            }
            actionSprite.SetNativeSize();
            actionSprite.gameObject.SetActive(true);
            if (kind != ChoiceKind.ZiMo && kind != ChoiceKind.RongHe)
            {

            }
            else
            {

            }
            DOTween.Sequence().AppendInterval(1).AppendCallback(HideActionSprite);
        }
        public void HideActionSprite()
        {
            actionSprite.gameObject.SetActive(false);
        }
    }
}