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
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            cards = new List<RealCard>();
        }
        private void Start()
        {
            for (int i = 0; i < 13; ++i) 
            {
                CreateCard(new CardKind(i));
            }
            DrawCard(new CardKind(13));
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
            card.transform.DOLocalMove(destPosition, length * DataManager.handCardMoveSpeed);
        }
        public void FormatCard()
        {
            cards.Sort();
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
            FormatCard();
            return playCard;
        }
    }
}