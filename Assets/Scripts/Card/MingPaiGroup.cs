using Data;
using System.Linq;
using UnityEngine;

namespace Card
{
    public class MingPaiGroup : MonoBehaviour
    {
        public MingPaiKind kind;
        public RealCard[] cards;
        public int fromPeople;

        public float Init(float x, RealCard card, bool isHorizental = false, bool isFanMian = false, bool isJiaGang = false)
        {
            x += (isHorizental ? 0.18f : 0.14f);
            card.transform.parent = this.transform;
            card.transform.localPosition = new Vector3(x, 0.135f, isHorizental ? (isJiaGang ? 0.23f : -0.0423f) : 0);
            card.Init(this, isHorizental, isFanMian, isJiaGang); 
            return x + (isHorizental ? 0.18f : 0.14f);
        }
        public float Init(MingPaiKind kind, RealCard[] cards, int fromPeople)
        {
            cards = cards.Reverse().ToArray();
            fromPeople = cards.Length - 1 - fromPeople;
            this.kind = kind;
            this.cards = cards;
            this.fromPeople = fromPeople;
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
                case MingPaiKind.BaBei:
                    {
                        break;
                    }
            }
            return width;
        }
        public void JiaGang(RealCard card)
        {
            kind = MingPaiKind.JiaGang;
            Init(fromPeople * 0.28f, card, true, false, true);
        }
    }
}