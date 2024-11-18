using Data;
using UnityEngine;

namespace GameSceneUI
{
    public class CardChoiceItem : MonoBehaviour
    {
        public CardChoiceUI[] cards;
        public BoxCollider2D clickChecker;

        private void OnMouseUpAsButton()
        {
            
        }

        public void Init(TingPai tingPai)
        {
            cards = new CardChoiceUI[1];
            cards[0] = GameObject.Instantiate(GameSceneUIManager.instance.gamePanel.cardChoicePanel.cardPrefab, this.transform);
            cards[0].Init(tingPai);
            cards[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(CardChoicePanel.cardWidth / 2, 19.5f);
        }
        public void Init(MingPai mingPai)
        {
            cards = new CardChoiceUI[mingPai.kinds.Length];
            for (int i = 0; i < cards.Length; ++i)
            {
                cards[i] = GameObject.Instantiate(GameSceneUIManager.instance.gamePanel.cardChoicePanel.cardPrefab, this.transform);
                cards[i].Init(mingPai.kinds[i]);
                cards[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(CardChoicePanel.cardWidth / 2 + (CardChoicePanel.cardWidth + CardChoicePanel.cardDistance) * i, 0);
            }
        }
    }
}