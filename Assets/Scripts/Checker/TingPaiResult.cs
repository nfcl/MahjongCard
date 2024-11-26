using Data;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Checker
{
    public class TingPaiResult
    {
        public DivideResult[] normal;
        public YiZhongResult[] normalYiResult;
        public bool isQiDui;
        public YiZhongResult qiDuiYiResult;
        public bool isGuoShi;
        public YiZhongResult guoShiYiResult;
        public YiZhongResult additionalYiZhongs;

        public bool IsTingPai => normal.Length != 0 || isQiDui || isGuoShi;
        public bool IsWuYi =>
            normalYiResult.All(_ => _.fanNum == 0)
            && (isQiDui && qiDuiYiResult.fanNum == 0)
            && (isGuoShi && guoShiYiResult.fanNum == 0);

        public YiZhongResult BestChoice()
        {
            YiZhongResult result = normalYiResult.OrderByDescending(_ => _.fanNum).FirstOrDefault();

            if (isQiDui && result.fanNum < qiDuiYiResult.fanNum)
                result = qiDuiYiResult;
            if (isGuoShi && result.fanNum < guoShiYiResult.fanNum)
                result = guoShiYiResult;

            result = new YiZhongResult(result);

            result.Add(additionalYiZhongs);

            return result;
        }

        public TingPaiResult()
        {
            normal = null;
            normalYiResult = null;
            isQiDui = false;
            qiDuiYiResult = null;
            isGuoShi = false;
            guoShiYiResult = null;
            additionalYiZhongs = null;
        }
        private static void DivideBlock(int index, bool dontSelectHead, HandMatrix handsMatrix, DivideResult road, List<DivideResult> results)
        {
            while (index < 34 && handsMatrix[index] == 0)
            {
                index += 1;
            }
            if (index >= 34)
            {
                if (road.hasHead)
                {
                    results.Add(road);
                }
                return;
            }
            int kind = index / 9;
            int num = index % 9;
            if (!dontSelectHead)
            {
                if (!road.hasHead && handsMatrix.TryDui(kind, num))
                {
                    HandMatrix dividedMatrix = new HandMatrix(handsMatrix);
                    dividedMatrix.ExtractDui(kind, num);
                    DivideResult newRoad = new DivideResult(road);
                    newRoad.Add(BlockKind.Dui, kind, num);
                    DivideBlock(index, true, dividedMatrix, newRoad, results);
                }
            }
            int cardNum = handsMatrix[kind, num];
            int keNum = 0;
            while (true)
            {
                if (handsMatrix.TryKe(kind, num, keNum))
                {
                    if (handsMatrix.TryShun(kind, num, cardNum - 3 * keNum))
                    {
                        HandMatrix dividedMatrix = new HandMatrix(handsMatrix);
                        dividedMatrix.ExtractKe(kind, num, keNum);
                        dividedMatrix.ExtractShun(kind, num, cardNum - 3 * keNum);
                        DivideResult newRoad = new DivideResult(road);
                        newRoad.Add(BlockKind.Ke, kind, num, keNum);
                        newRoad.Add(BlockKind.Shun, kind, num, cardNum - 3 * keNum);
                        DivideBlock(index + 1, false, dividedMatrix, newRoad, results);
                    }
                    keNum += 1;
                    continue;
                }
                else
                {
                    break;
                }
            }
            // this(n, hasHead) = !hasHead  => dui + this(n - 2, true)
            //                    all       => Σ (i => 0, n / 3) shun * (n - i * 3) + ke * (i)
        }
        public static List<DivideResult> Check3NP2(HandMatrix handsMatrix)
        {
            List<DivideResult> results = new List<DivideResult>();
            DivideResult origin = new DivideResult { blocks = new List<Block>(), hasHead = false };
            DivideBlock(0, false, handsMatrix, origin, results);
            return results;
        }
        public static TingPaiResult CheckTingPaiResult(CardKind[] hands, LogicMingPaiGroup[] mings)
        {
            TingPaiResult result = new TingPaiResult();
            HandMatrix handsMatrix = new HandMatrix(hands);
            if (mings.All(_ => _.kind == MingPaiKind.BaBei))
            {
                //全是拔北的鸣牌可以判断其他两种牌型
                if (handsMatrix.isQiDui)
                {
                    //七对子
                    result.isQiDui = true;
                }
                else if (handsMatrix.isGuoShi)
                {
                    //国士无双
                    result.isGuoShi = true;
                }
            }
            result.normal = Check3NP2(handsMatrix).ToArray();
            return result;
        }
    }
}