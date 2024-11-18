using Data;
using UnityEngine;

namespace Card
{
    public class MingPaiGroup : MonoBehaviour
    {
        public RealCard[] cards;

        public float Init(float x, RealCard card, bool isHorizental = false, bool isFanMian = false, bool isJiaGang = false)
        {
            x += (isHorizental ? 0.18f : 0.14f);
            card.transform.parent = this.transform;
            card.transform.localEulerAngles = new Vector3(0, isHorizental ? 90 : 0, isFanMian ? -90 : 90);
            card.transform.localPosition = new Vector3(x, 0.135f, isHorizental ? (isJiaGang ? 0.23f : -0.0423f) : 0);
            return x + (isHorizental ? 0.18f : 0.14f);
        }
        public float Init(MingPaiKind kind, RealCard[] cards, int fromPeople)
        {
            this.cards = cards;
            float width = 0;
            switch (kind)
            {
                case MingPaiKind.Peng:
                case MingPaiKind.Chi:
                case MingPaiKind.MingGang:
                    {
                        cards.Foreach((_, index) => width = Init(width, _, fromPeople == index));
                        break;
                    }
                case MingPaiKind.AnGang:
                    {
                        cards.Foreach((_, index) => width = Init(width, _, false, index == 0 || index == 3));
                        break;
                    }
            }
            return width;
        }
        public void JiaGang(RealCard card, int index)
        {
            Init(index * 0.28f + 0.18f, card, true, false, true);
        }
    }
}