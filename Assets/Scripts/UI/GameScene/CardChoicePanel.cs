using Data;
using UnityEngine;
using UnityEngine.UI;

namespace GameSceneUI
{
    public class CardChoicePanel : MonoBehaviour
    {
        public static float margin = (240 - 80) / 2;
        public static float cardWidth = 80;
        public static float cardDistance = 5;
        public static float itemDistance = 50;

        public CanvasGroup canvasGroup;
        public Image background;
        public Transform container;

        public CardChoiceUI cardPrefab;
        public CardChoiceItem itemPrefab;
        public CardChoiceItem[] items;
        public Sprite[] ChoiceStateSprite;
        public Button tingPaiButton;

        public void Awake()
        {
            Clear();
        }

        public void Open(Data.TingPai[] tingPai)
        {
            Init(tingPai);
            tingPaiButton.gameObject.SetActive(false);
            canvasGroup.Open();
        }
        public void Open(Data.MingPai[] mingPai)
        {
            Init(mingPai);
            tingPaiButton.gameObject.SetActive(true);
            canvasGroup.Open();
        }
        public void Close()
        {
            canvasGroup.Close();
        }
        public void Clear()
        {
            items = null;
            while (container.childCount > 0)
            {
                DestroyImmediate(container.GetChild(0).gameObject);
            }
        }
        public void Init(Data.TingPai[] tingPai)
        {
            Clear();
            items = new CardChoiceItem[tingPai.Length];
            float x = margin - itemDistance;
            for (int i = 0; i < items.Length; ++i)
            {
                items[i] = GameObject.Instantiate(itemPrefab, container);
                x += itemDistance;
                items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                items[i].Init(tingPai[i]);
                float itemWidth = cardWidth;
                items[i].clickChecker.size = new Vector2(itemWidth, 175);
                items[i].clickChecker.offset = new Vector2(itemWidth / 2, 0);
                x += itemWidth;
            }
            x += margin;
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
        }

        public void Init(Data.MingPai[] mingPai)
        {
            items = new CardChoiceItem[mingPai.Length];
            float x = margin - itemDistance;
            for (int i = 0; i < items.Length; ++i)
            {
                items[i] = GameObject.Instantiate(itemPrefab, container);
                x += itemDistance;
                items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                items[i].Init(mingPai[i]);
                float itemWidth = cardWidth + (cardWidth + cardDistance) * (items[i].cards.Length - 1);
                items[i].clickChecker.size = new Vector2(itemWidth, 175);
                items[i].clickChecker.offset = new Vector2(itemWidth / 2, 0);
                x += itemWidth;
            }
            x += margin;
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
        }

        public enum ChoiceCardState
        {
            LastNum,
            FanFu,
            WuYi
        }

//#if UNITY_EDITOR

//        private void OnValidate()
//        {
//            items = container.GetComponentsInChildren<CardChoiceItem>();
//            float x = margin - itemDistance;
//            items.Foreach((_, index) =>
//            {
//                x += itemDistance;
//                _.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
//                _.cards.Foreach((__, _index) =>
//                {
//                    __.GetComponent<RectTransform>().anchoredPosition = new Vector2(cardWidth / 2 + (cardWidth + cardDistance) * _index, 19.5f);
//                });
//                float itemWidth = cardWidth + (cardWidth + cardDistance) * (_.cards.Length - 1);
//                _.clickChecker.size = new Vector2(itemWidth, 175);
//                _.clickChecker.offset = new Vector2(itemWidth / 2, 0);
//                x += itemWidth;
//            });
//            x += margin;
//            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
//        }

//#endif
    }
}