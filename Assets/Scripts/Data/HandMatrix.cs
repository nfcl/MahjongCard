using System.Linq;
using UnityEngine;

namespace Data
{
    public class HandMatrix
    {
        private int[][] matrix;

        public bool isQiDui => matrix.All(_ => _.All(__ => __ == 0 || __ == 2));
        public bool isGuoShi => matrix.Take(3).All(_ => _[0] >= 1 && _[8] >= 1) && matrix[3].All(_ => _ >= 1);

        public int this[int _, int __ = -1]
        {
            get
            {
                if (__ == -1)
                {
                    return matrix[_ / 9][_ % 9];
                }
                else
                {
                    return matrix[_][__];
                }
            }
            set
            {
                if (__ == -1)
                {
                    matrix[_ / 9][_ % 9] = value;
                }
                else
                {
                    matrix[_][__] = value;
                }
            }
        }

        public HandMatrix()
        {
            matrix = new int[4][]
            {
                new int[9]{ 0,0,0,0,0,0,0,0,0 },
                new int[9]{ 0,0,0,0,0,0,0,0,0 },
                new int[9]{ 0,0,0,0,0,0,0,0,0 },
                new int[7]{ 0,0,0,0,0,0,0     }
            };
        }
        public HandMatrix(CardKind[] hands)
        {
            matrix = new int[4][]
            {
                new int[9]{ 0,0,0,0,0,0,0,0,0 },
                new int[9]{ 0,0,0,0,0,0,0,0,0 },
                new int[9]{ 0,0,0,0,0,0,0,0,0 },
                new int[7]{ 0,0,0,0,0,0,0     }
            };
            hands.Foreach((_, __) => Add(_));
        }
        public HandMatrix(HandMatrix other)
        {
            matrix = new int[other.matrix.Length][];
            for (int i = 0; i < matrix.Length; ++i)
            {
                matrix[i] = new int[other.matrix[i].Length];
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    matrix[i][j] = other.matrix[i][j];
                }
            }
        }
        public void Add(CardKind card)
        {
            try
            {
                matrix[card.huaseKind][card.huaseNum] += 1;
            }
            catch (System.Exception e)
            {
                Debug.Log(card);
                throw e;
            }
        }
        public bool TryShun(int huaseKind, int huaseNum, int num = 1)
        {
            if (num == 0)
            {
                return true;
            }
            if (huaseKind == 3)
            {
                return false;
            }
            if (huaseNum >= 7)
            {
                return false;
            }
            return matrix[huaseKind][huaseNum + 0] >= num
                && matrix[huaseKind][huaseNum + 1] >= num
                && matrix[huaseKind][huaseNum + 2] >= num;
        }
        public void ExtractShun(int huaseKind, int huaseNum, int num = 1)
        {
            if (num == 0)
            {
                return;
            }
            matrix[huaseKind][huaseNum + 0] -= num;
            matrix[huaseKind][huaseNum + 1] -= num;
            matrix[huaseKind][huaseNum + 2] -= num;
        }
        public bool TryKe(int huaseKind, int huaseNum, int num = 1)
        {
            if (num == 0)
            {
                return true;
            }
            if (matrix[huaseKind][huaseNum] < num * 3)
            {
                return false;
            }
            return true;
        }
        public void ExtractKe(int huaseKind, int huaseNum, int num = 1)
        {
            if (num == 0)
            {
                return;
            }
            matrix[huaseKind][huaseNum] -= 3 * num;
        }
        public bool TryDui(int huaseKind, int huaseNum)
        {
            if (matrix[huaseKind][huaseNum] <= 1)
            {
                return false;
            }
            return true;
        }
        public void ExtractDui(int huaseKind, int huaseNum)
        {
            matrix[huaseKind][huaseNum] -= 2;
        }
    }
}