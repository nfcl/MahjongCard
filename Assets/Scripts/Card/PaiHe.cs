using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Card
{
    public class PaiHe : MonoBehaviour
    {
        public List<RealCard> cards;

        private int chunkIndex;
        private Vector2 chunkLimit;
        private Vector3 chunkStartPosition;
        public int ChunkIndex
        {
            get
            {
                return chunkIndex;
            }
            set
            {
                chunkIndex = value;
                chunkLimit = DataManager.GetChunkLimit(chunkIndex);
                chunkStartPosition = DataManager.GetChunkStartLeftBorder(chunkIndex);
            }
        }

        private void Awake()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            cards = new List<RealCard>();
        }
        private void Start()
        {
            ChunkIndex = 0;
            bool isLizhi = false;
            for (int i = 0; i < 40; ++i)
            {
                if (!isLizhi && Random.Range(i, 40) == 39)
                {
                    isLizhi = true;
                    AddCard(RealCard.Create(), true);
                }
                else
                {
                    AddCard(RealCard.Create(), false);
                }
            }
        }
        public Vector3 GetLastCardRightBorder()
        {
            if(cards.Count == 0)
            {
                Vector2 chunkStartPosition = DataManager.GetChunkStartLeftBorder(chunkIndex);
                return new Vector3(
                    chunkStartPosition.x,
                    DataManager.paiHeCardY,
                    chunkStartPosition.y
                );
            }
            RealCard lastCard = cards[cards.Count - 1];
            Vector3 result = lastCard.transform.localPosition;
            result.x += lastCard.isLiZhi ? DataManager.paiHeLiZhiCardHorizentalDistance
                                         : DataManager.paiHeCardNormalHorizentalDistance;
            return result;
        }
        public Vector3 GetNextCardPosition(bool isLiZhi)
        {
            Vector3 result = GetLastCardRightBorder();
            result.x += isLiZhi ? DataManager.paiHeLiZhiCardHorizentalDistance
                                : DataManager.paiHeCardNormalHorizentalDistance;
            if(result.x > chunkLimit.x)
            {
                result.z -= DataManager.paiHeCardVerticalDistance;
                if (result.z < chunkLimit.y)
                {
                    ChunkIndex += 1;
                    result.z = chunkStartPosition.y;
                }
                result.x = chunkStartPosition.x
                           + (isLiZhi ? DataManager.paiHeLiZhiCardHorizentalDistance
                                      : DataManager.paiHeCardNormalHorizentalDistance);
            }
            return result;
        }
        public void AddCard(RealCard card, bool isLiZhi)
        {
            card.Init(this, isLiZhi);
            card.transform.localPosition = GetNextCardPosition(isLiZhi);
            cards.Add(card);
        }
    }
}