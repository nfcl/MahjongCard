using Card;
using Data;
using System;
using UnityEngine;
using static MainSceneUI.PropPanel;

namespace GameSceneUI
{
    public class CardChoiceItem : MonoBehaviour
    {
        public CardChoiceUI[] cards;
        public BoxCollider2D clickChecker;
        public D_Void_Void callBack;

        private void OnMouseUpAsButton()
        {
            callBack?.Invoke();
        }

        private void Init(CardKind[] cards)
        {
            this.cards = new CardChoiceUI[cards.Length];
            for (int i = 0; i < cards.Length; ++i)
            {
                this.cards[i] = GameObject.Instantiate(GameSceneUIManager.instance.gamePanel.cardChoicePanel.cardPrefab, this.transform);
                this.cards[i].Init(cards[i]);
                this.cards[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(CardChoicePanel.cardWidth / 2 + (CardChoicePanel.cardWidth + CardChoicePanel.cardDistance) * i, 0);
            }
        }
        public void Init(ClientCardTingPaiResult tingPai, bool isZhenTing)
        {
            callBack = null;
            cards = new CardChoiceUI[1];
            cards[0] = GameObject.Instantiate(GameSceneUIManager.instance.gamePanel.cardChoicePanel.cardPrefab, this.transform);
            cards[0].Init(tingPai, isZhenTing);
            cards[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(CardChoicePanel.cardWidth / 2, 19.5f);
        }
        public void Init(CardKind[] mingPai, D_Void_Void callBack)
        {
            this.callBack = callBack;
            Init(mingPai);
        }
    }
}