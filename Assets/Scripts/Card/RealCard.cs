using Data;
using System.ComponentModel;
using UnityEngine;

namespace Card
{
    public class RealCard : MonoBehaviour
    {
        public MeshRenderer cardSkinRender;
        public SpriteRenderer face;
        public Transform shadow;
        public GameObject moveShining;

        public CardKind faceKind;
        public bool isBao
        {
            set
            {
                moveShining.SetActive(value);
            }
        }

        #region PaiHe

        public bool isLiZhi;

        #endregion

        public static RealCard Create()
        {
            return Create(DataManager.realCardBackMat);
        }
        public static RealCard Create(Material skinMat)
        {
            RealCard newCard = GameObject.Instantiate(DesktopManager.instance.prefab, DesktopManager.instance.gameObject.transform);
            newCard.cardSkinRender.material = skinMat;
            return newCard;
        }
        public void Init(CardKind kind)
        {
            faceKind = kind;
            face.sprite = DataManager.GetCardFaceSprite(faceKind);
            isBao = kind.isBao;
        }
        public void Init(PaiHe paiHe, bool isLiZhi, bool isHide)
        {
            transform.SetParent(paiHe.transform);
            Vector3 newEuler;
            newEuler = transform.localEulerAngles;
            newEuler.z = 90;
            newEuler.y = (isLiZhi ? 90 : 0) + Random.Range(-3.0f, 3.0f);
            transform.localEulerAngles = newEuler;
            newEuler = shadow.localEulerAngles;
            newEuler.y = 90;
            shadow.localEulerAngles = newEuler;

            this.isLiZhi = isLiZhi;

            face.gameObject.SetActive(!isHide);
            moveShining.SetActive(!isHide && faceKind.isBao);
        }
        public void Init(HandCard handCard, bool isHide)
        {
            transform.SetParent(handCard.transform);
            Vector3 newEuler = new Vector3(-90, 0, 90);
            transform.localEulerAngles = newEuler;
            newEuler = shadow.localEulerAngles;
            newEuler.y = handCard.IsShowHandCard ? 90 : 0;
            shadow.localEulerAngles = newEuler;

            face.gameObject.SetActive(!isHide);
            moveShining.SetActive(!isHide && faceKind.isBao);
        }

        //private void OnValidate()
        //{
        //    float eulerx = (transform.localEulerAngles.x + 360) % 360;
        //    Vector3 newShadowEuler = shadow.localEulerAngles;
        //    if (eulerx == 0)
        //    {
        //        newShadowEuler.y = 90;
        //    }
        //    else
        //    {
        //        newShadowEuler.y = 0;
        //    }
        //    shadow.localEulerAngles = newShadowEuler;
        //}
    }
}