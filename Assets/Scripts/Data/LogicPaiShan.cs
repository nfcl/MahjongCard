﻿using System;

namespace Data
{
    public class LogicPaiShan
    {
        private CardKind[] cards;
        private int lingShang;
        private int baoPai;
        private int drawCard;

        public int LastDrawCardCount => 34 * 4 - 14 - (drawCard + lingShang);
        public bool IsDrawEnd => LastDrawCardCount == 0;
        public bool CanLiZhi => LastDrawCardCount >= 4;
        public bool CanGang => LastDrawCardCount > 0;

        public LogicPaiShan(int seed)
        {
            cards = new CardKind[34 * 4];
            int num = 0;

            for (int i = 0; i < 37; ++i)
            {
                if (i == 0 || i == 10 || i == 20)
                {
                    cards[num++] = new CardKind(i);
                }
                else if (i == 5 || i == 15 || i == 25)
                {
                    cards[num++] = new CardKind(i);
                }
                else
                {
                    cards[num++] = new CardKind(i);
                    cards[num++] = new CardKind(i);
                }
            }

            for (int i = 0; i < 37; ++i)
            {
                if (i == 0 || i == 10 || i == 20)
                {
                    cards[num++] = new CardKind(i + 5);
                }
                else if (i == 5 || i == 15 || i == 25)
                {
                    cards[num++] = new CardKind(i);
                }
                else
                {
                    cards[num++] = new CardKind(i);
                    cards[num++] = new CardKind(i);
                }
            }
            lingShang = 0;
            baoPai = 1;
            drawCard = 0;
            //Randomize(seed);
        }
        private void Randomize(int seed)
        {
            Random rand = new Random(seed);

            CardKind tmp;
            int swapIndex;
            for (int i = 0; i < cards.Length; ++i)
            {
                tmp = cards[i];
                swapIndex = rand.Next(i, cards.Length);
                cards[i] = cards[swapIndex];
                cards[swapIndex] = tmp;
            }
        }
        public CardKind PeekLingShang(int index)
        {
            return cards[cards.Length - 1 - index];
        }
        public CardKind GetLingShang()
        {
            return cards[cards.Length - 1 - lingShang++];
        }
        public CardKind PeekBaoPai(int index)
        {
            return cards[cards.Length - 5 - 2 * index - 0];
        }
        public CardKind PeekLiBaoPai(int index)
        {
            return cards[cards.Length - 5 - 2 * index - 1];
        }
        public void ShowBaoPai()
        {
            baoPai += 1;
        }
        public CardKind[] GetBaoPai()
        {
            CardKind[] result = new CardKind[baoPai];
            for (int i = 0; i < baoPai; ++i)
            {
                result[i] = cards[cards.Length - 5 - 2 * i - 0];
            }
            return result;
        }
        public CardKind[] GetLiBaoPai()
        {
            CardKind[] result = new CardKind[baoPai];
            for (int i = 0; i < baoPai; ++i)
            {
                result[i] = cards[cards.Length - 5 - 2 * i - 1];
            }
            return result;
        }
        public CardKind[] GetConfigurCard()
        {
            CardKind[] result = new CardKind[13];
            for(int i = 0; i < result.Length; ++i)
            {
                result[i] = cards[drawCard + i];
            }
            drawCard += result.Length;
            return result;
        }
        public CardKind PeekDrawCard(int index)
        {
            return cards[index];
        }
        public CardKind GetDrawCard()
        {
            return cards[drawCard++];
        }
    }
}