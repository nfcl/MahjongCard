using Data;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Card
{
    public class HandCard : MonoBehaviour
    {
        public List<RealCard> cards;

        private bool isShowHandCard = false;
        public bool IsShowHandCard
        {
            get
            {
                return isShowHandCard;
            }
            set
            {
                isShowHandCard = value;
                Vector3 newEuler = transform.localEulerAngles;
                newEuler.x = isShowHandCard ? 90 : 0;
                transform.localEulerAngles = newEuler;
            }
        }

        private void Awake()
        {
            DestroyImmediate(gameObject.GetComponent<HandCardDebuger>());
            Clear();
        }
        private void Start()
        {
            //for (int i = 0; i < 13; ++i) 
            //{
            //    CreateCard(new CardKind(i));
            //}
            //DrawCard(new CardKind(13));
        }
        public void ConfigurCard(CardKind[] kinds)
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < kinds.Length; i += DataManager.uiHandCardConfigurPerGroupCount)
            {
                int startIndex = i;
                sequence
                    .AppendCallback(() =>
                    {
                        for (int k = 0; k < Mathf.Min(DataManager.uiHandCardConfigurPerGroupCount, kinds.Length - startIndex); ++k)
                        {
                            int tempIndex = startIndex + k;
                            DrawCard(kinds[tempIndex]);
                        }
                    })
                    .AppendInterval(DataManager.uiHandCardConfigurGroupAppearDuration);
            }
            sequence
                .AppendCallback(() =>
                {
                    this.cards.Sort((x, y) => CardKind.LogicComparer.Compare(x.faceKind, y.faceKind));
                    for (int i = 0; i < this.cards.Count; ++i)
                    {
                        MoveCard(this.cards[i], i);
                    }
                });
        }
        public void Clear()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            cards = new List<RealCard>();
        }
        public void AddCard(RealCard card)
        {
            card.Init(this, transform.parent.localEulerAngles.y != 0);
            card.transform.localPosition = DataManager.handCardStartPosition 
                                           + cards.Count * DataManager.handCardNormalDistance;
            cards.Add(card);
        }
        public void AddNewCard(RealCard card)
        {
            card.Init(this, transform.parent.localEulerAngles.y != 0);
            card.transform.localPosition = DataManager.handCardStartPosition
                                           + (cards.Count - 1) * DataManager.handCardNormalDistance
                                           + DataManager.handCardNewDistance;
            cards.Add(card);
        }
        public void MoveCard(RealCard card, int dest)
        {
            Vector3 destPosition = DataManager.handCardStartPosition + dest * DataManager.handCardNormalDistance;
            float length = Vector3.Distance(card.transform.localPosition, destPosition);
            card.transform.DOLocalMove(destPosition, 0.1f);
        }
        public void FormatCard()
        {
            cards.Sort((x, y) => CardKind.LogicComparer.Compare(x.faceKind, y.faceKind));
            for (int i = 0; i < cards.Count; ++i)
            {
                MoveCard(cards[i], i);
            }
        }
        public void CreateCard(CardKind kind)
        {
            RealCard newCard = RealCard.Create();
            newCard.Init(kind);
            AddCard(newCard);
        }
        public void DrawCard(CardKind kind)
        {
            RealCard newCard = RealCard.Create();
            newCard.Init(kind);
            AddNewCard(newCard);
        }
        public RealCard PlayCard(CardKind kind)
        {
            RealCard playCard = cards.Where(_ => _.faceKind == kind).FirstOrDefault();
            cards.Remove(playCard);
            DOTween.Sequence().AppendInterval(0.5f).AppendCallback(FormatCard);
            return playCard;
        }
        public RealCard[] MingCard(CardKind[] kind)
        {
            var results = kind.Select(_ =>
            {
                var result = cards.First(__ => __.faceKind == _);
                cards.Remove(result);
                return result;
            }).ToArray();

            FormatCard();

            return results;
        }
    }
}