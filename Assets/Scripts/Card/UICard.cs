using Data;
using DG.Tweening;
using GameSceneUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Card
{
    public class UICard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image cardFace;
        public Image cardBack;
        public Image moveShining;

        public bool isBao
        {
            set
            {
                moveShining.gameObject.SetActive(value);
            }
        }

        public bool isInteractable = false;
        public bool isDrag = false;
        public CardKind faceKind;
        public int handIndex;

        public Vector3 startDragPointerPosition;
        public Vector3 startDragCardPosition;

        public static UICard Create()
        {
            UICard newCard = GameObject.Instantiate(
                GameSceneUIManager.instance.gamePanel.handCard.prefab, 
                GameSceneUIManager.instance.gamePanel.handCard.transform
            );
            return newCard;
        }
        public void Init(CardKind kind)
        {
            faceKind = kind;

            cardFace.sprite = DataManager.GetUICardFaceSprite(faceKind);
            cardBack.sprite = DataManager.handBackSprites[0];

            isBao = kind.isBao;
        }
        public void DoDraw()
        {
            isInteractable = false;
            cardFace.transform.localPosition += new Vector3(0, 50, 0);
            DOTween.Sequence()
                .AppendInterval(DataManager.uiHandCardConfigurDrawPauseDuration)
                .Append(cardFace.transform.DOLocalMove(Vector3.zero, DataManager.uiHandCardConfigurDrawMoveDuration));
        }
        public void CalculateDragCardPosition(Vector3 pointerPosition)
        {
            //Debug.Log($"pointerPosition = {pointerPosition}\nstartDragPointerPosition = {startDragPointerPosition}\n" +
            //    $"transformPosition = {transform.position}\nstartDragCardPosition = {startDragCardPosition}");
            transform.position = startDragCardPosition + (pointerPosition - startDragPointerPosition);
            GameSceneUIManager.instance.gamePanel.handCard.CheckCardMove(this);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDrag)
            {
                return;
            }
            CalculateDragCardPosition(eventData.position);
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isInteractable)
            {
                return;
            }
            isInteractable = false;
            startDragPointerPosition = eventData.position;
            startDragCardPosition = transform.position;
            transform.SetAsLastSibling();
            isDrag = true;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDrag)
            {
                return;
            }
            isDrag = false;
            transform.localPosition = GameSceneUIManager.instance.gamePanel.handCard.GetExceptNormalCardPosition(handIndex);
            isInteractable = true;
        }
    }
}