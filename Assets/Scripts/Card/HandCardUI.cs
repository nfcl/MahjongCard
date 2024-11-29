using Data;
using DG.Tweening;
using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Card
{
    public class HandCardUI : MonoBehaviour
    {
        public UICard prefab;

        public List<UICard> cards;
        public UICard lastCard;

        private void Awake()
        {
            DestroyImmediate(GetComponent<HandCardUIDebuger>());
            Clear();
        }
        public void Start()
        {
            //CardKind[] kinds = new CardKind[13];
            //for(int i = 0; i < kinds.Length; ++i)
            //{
            //    kinds[i] = CardKind.GetRandomKind();
            //}
            //ConfigurInitialHandCard(kinds)
            //    .AppendInterval(1)
            //    .AppendCallback(() =>
            //    {
            //        DrawCard(CardKind.GetRandomKind(), true);
            //    });
        }
        public void FormatCard()
        {
            cards.Foreach((_, index) => MoveCard(_, index));
        }
        public void PlayCard(CardKind card)
        {
            if (lastCard == null || card != lastCard.faceKind)
            {
                lastCard = cards.Where(_ => _.faceKind == card).FirstOrDefault();
            }
            cards.Remove(lastCard);
            DestroyImmediate(lastCard.gameObject);
            lastCard = null;
            FormatCard();
        }
        public void UnBanCard()
        {
            cards.ForEach(_ => _.IsInteractableWithColor = true);
        }
        public void BanCard()
        {
            cards.ForEach(_ => _.IsInteractable = false);
        }
        public void BanCard(CardKind card)
        {
            cards.ForEach(_ => _.IsInteractableWithColor = !CardKind.LogicEqualityComparer.Equals(_.faceKind, card));
        }
        public void BanCard(ChoicePlayCard choice)
        {
            HashSet<CardKind> set = choice.cards.ToHashSet(CardKind.LogicEqualityComparer);
            cards.ForEach(_ => _.IsInteractableWithColor = set.Contains(_.faceKind, CardKind.LogicEqualityComparer) == choice.isWhite);
        }
        public void BanCard(ChoiceLiZhi choice)
        {
            HashSet<CardKind> set = choice.choices.cards.Select(_ => _.playCard).ToHashSet(CardKind.LogicEqualityComparer);
            cards.ForEach(_ => _.IsInteractableWithColor = !set.Contains(_.faceKind, CardKind.LogicEqualityComparer));
        }
        public void SyncBaoPai()
        {
            foreach(UICard card in cards)
            {
                card.isBao = IGameLogicManager.instance.isBaoPai(card.faceKind);
            }
        }
        public void Clear()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            cards = new List<UICard>();
        }
        public void ConfigurInitialHandCard(CardKind[] kinds)
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
                    cards.Sort((x, y) => CardKind.LogicComparer.Compare(x.faceKind, y.faceKind));
                    for (int i = 0; i < cards.Count; ++i)
                    {
                        MoveCard(cards[i], i);
                    }
                })
                .AppendInterval(0.1f)
                .AppendCallback(() => cards.ForEach(_ => _.isInteractable = true));
        }
        public Vector3 GetExceptNormalCardPosition(int index)
        {
            return DataManager.uiHandCardStartPosition + DataManager.uiHandCardNormalDistance * index;
        }
        public Vector3 GetExceptNewDrewCardPosition()
        {
            return DataManager.uiHandCardStartPosition + DataManager.uiHandCardNormalDistance * (cards.Count - 1) + DataManager.uiHandCardNewDrewDistance;
        }
        public void DrawCard(CardKind kind, bool isDraw = false)
        {
            UICard newCard = UICard.Create();
            newCard.Init(kind);

            newCard.transform.localPosition = isDraw
                ? GetExceptNewDrewCardPosition()
                : GetExceptNormalCardPosition(cards.Count);

            newCard.handIndex = cards.Count;
            
            newCard.DoDraw();

            cards.Add(newCard);
        }
        public void MoveCard(UICard card, int dest)
        {
            card.handIndex = dest;
            Vector3 destPosition = GetExceptNormalCardPosition(dest);
            card.transform.DOLocalMove(destPosition, 0.1f);
        }
        public void CheckCardMove(UICard card)
        {
            float currentPosition = card.transform.localPosition.x;
            float exceptPosition = GetExceptNormalCardPosition(card.handIndex).x;
            int orig = card.handIndex;
            int move = (int)((currentPosition - exceptPosition) / DataManager.uiHandCardNormalDistance.x);
            if(move == 0)
            {
                return;
            }
            int dest = orig + move;
            if(dest < 0)
            {
                dest = 0;
            }
            else if(dest >= cards.Count)
            {
                dest = cards.Count - 1;
            }
            if(orig < dest)
            {
                for (int i = orig + 1; i <= dest; ++i)
                {
                    MoveCard(cards[i], i - 1);
                    cards[i - 1] = cards[i];
                }
            }
            else
            {
                for(int i = orig - 1; i >= dest; --i)
                {
                    MoveCard(cards[i], i + 1);
                    cards[i + 1] = cards[i];
                }
            }
            cards[dest] = card;
            card.handIndex = dest;
        }
        public void MingCard(CardKind kind)
        {
            var card = this.cards.First(__ => __.faceKind == kind);
            this.cards.Remove(card);
            DestroyImmediate(card.gameObject);
            FormatCard();
        }
        public void MingCard(CardKind[] kinds)
        {
            kinds.Foreach((_, index) =>
            {
                var card = this.cards.First(__ => __.faceKind == _);
                this.cards.Remove(card);
                DestroyImmediate(card.gameObject);
                FormatCard();
            });
        }
    }
}