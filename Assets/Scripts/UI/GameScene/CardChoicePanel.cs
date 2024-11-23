using Data;
using System;
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
        public Button cancelButton;

        public void Awake()
        {
            Clear();
        }

        public void Open()
        {
            canvasGroup.Open();
        }
        public void Open(ClientCardTingPai tingPai)
        {
            Init(tingPai);
            Open();
        }
        public void Open(CardKind[][] mingPai, Action<CardKind[]> callBack)
        {
            Init(mingPai, callBack);
            Open();
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
        public void Init(ClientCardTingPai tingPai)
        {
            Clear();
            cancelButton.gameObject.SetActive(false);
            items = new CardChoiceItem[tingPai.tingPais.Length];
            float x = margin - itemDistance;
            for (int i = 0; i < items.Length; ++i)
            {
                items[i] = GameObject.Instantiate(itemPrefab, container);
                x += itemDistance;
                items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                items[i].Init(tingPai.tingPais[i], tingPai.isZhenTing);
                float itemWidth = cardWidth;
                items[i].clickChecker.enabled = false;
                x += itemWidth;
            }
            x += margin;
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
        }

        public void Init(CardKind[][] mingPai, Action<CardKind[]> callBack)
        {
            cancelButton.gameObject.SetActive(true);
            items = new CardChoiceItem[mingPai.Length];
            float x = margin - itemDistance;
            for (int i = 0; i < items.Length; ++i)
            {
                items[i] = GameObject.Instantiate(itemPrefab, container);
                x += itemDistance;
                items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                items[i].Init(mingPai[i], callBack);
                float itemWidth = cardWidth + (cardWidth + cardDistance) * (items[i].cards.Length - 1);
                items[i].clickChecker.size = new Vector2(itemWidth, 175);
                items[i].clickChecker.offset = new Vector2(itemWidth / 2, 0);
                x += itemWidth;
            }
            x += margin;
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
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