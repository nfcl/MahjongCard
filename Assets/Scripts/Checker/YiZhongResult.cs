using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;

namespace Checker
{
    public class YiZhongResult
    {
        public List<Yi> yizhongs;
        public int fanNum;

        public YiZhongResult()
        {
            yizhongs = new List<Yi>();
            fanNum = 0;
        }
        public YiZhongResult(YiZhongResult other)
        {
            yizhongs = new List<Yi>(yizhongs);
            fanNum = other.fanNum;
        }
        public void Add(Yi yi)
        {
            yizhongs.Add(yi);
            fanNum += yi.fan;
        }
        public void Add(YiZhongResult other)
        {
            yizhongs.Concat(other.yizhongs);
            fanNum += other.fanNum;
        }

        private static YiZhongResult CheckAdditionalYiZhong(CheckInfo infos, Func<YiKind, Yi> fanDecoder)
        {
            YiZhongResult result = new YiZhongResult();

            if(infos.roundInfo.lastCardNum == 0)
            {
                //河底摸鱼/海底捞月
                result.Add(fanDecoder(infos.hePaiInfo.isRongHe ? YiKind.HeDiMoYu : YiKind.HaiDiLaoYue));
            }
            if (infos.hePaiInfo.isLingShangPai)
            {
                //抢杠/岭上开花
                result.Add(fanDecoder(infos.hePaiInfo.isRongHe ? YiKind.QiangGang : YiKind.LingShangKaiHua));
            }
            if (infos.selfInfo.isLiZhi)
            {
                //立直
                result.Add(fanDecoder(infos.selfInfo.isLiangLiZhi ? YiKind.LiangLiZhi : YiKind.LiZhi));
                if (infos.selfInfo.hasYiFa)
                {
                    //一发
                    result.Add(fanDecoder(YiKind.YiFa));
                }
            }
            if (infos.roundInfo.isNoBodyMingPai && infos.selfInfo.drewCardNum == 1)
            {
                //天和/地和
                result.Add(fanDecoder(infos.selfInfo.isZhuang ? YiKind.TianHe : YiKind.DiHe));
            }
            if (!infos.hePaiInfo.isRongHe && infos.mingInfo.isMenQianQing)
            {
                //门前清自摸
                result.Add(fanDecoder(YiKind.MenQianQingZiMoHe));
            }

            return result;
        }
        private static YiZhongResult CheckQiDuiYiZhong(CheckInfo infos, CardKind[] hands, Func<YiKind, Yi> fanDecoder)
        {
            YiZhongResult result = new YiZhongResult();

            result.Add(fanDecoder(YiKind.QiDuiZi));
            if (hands.All(_ => !(_.huaseKind < 3 && (0 < _.huaseNum && _.huaseNum < 8))))
                result.Add(fanDecoder(YiKind.HunLaoTou));
            if (hands.All(_ => _.huaseKind == 3))
                result.Add(fanDecoder(YiKind.ZiYiSe));

            return result;
        }
        private static YiZhongResult CheckGuoShiYiZhong(CheckInfo infos, CardKind[] hands, CardKind lastDrewCard, Func<YiKind, Yi> fanDecoder)
        {
            YiZhongResult result = new YiZhongResult();

            if (infos.selfInfo.drewCardNum == 1 || hands.Count(_ => _.realValue == lastDrewCard.realValue) == 2)
            {
                result.Add(fanDecoder(YiKind.GuoShiWuShuang13Mian));
            }
            else
            {
                result.Add(fanDecoder(YiKind.GuoShiWuShuang));
            }

            return result;
        }
        private static YiZhongResult _CheckNormalYiZhong(CheckInfo infos, CardKind lastDrewCard, DivideResult tingPai, LogicMingPaiGroup[] mings, Func<YiKind, Yi> fanDecoder)
        {
            YiZhongResult result = new YiZhongResult();

            Block lastDrewCardBlock = tingPai.blocks.First(_ => _.isLastDrewCardExsist);
            Block queTou = tingPai.blocks.First(_ => _.kind == BlockKind.Dui);

            bool hasBai = tingPai.ExsistKeGroup(3, 4);
            bool hasFa = tingPai.ExsistKeGroup(3, 5);
            bool hasZhong = tingPai.ExsistKeGroup(3, 6);
            bool hasChangFeng = tingPai.ExsistKeGroup(3, (int)infos.roundInfo.changFeng);
            bool hasZiFeng = tingPai.ExsistKeGroup(3, (int)infos.selfInfo.ziFeng);
            bool hasYiPai = hasBai || hasFa || hasZhong || hasChangFeng || hasZiFeng;
            bool allShun = tingPai.blocks.All(_ => _.kind == BlockKind.Shun || _.kind == BlockKind.Dui) && mings.All(_ => _.kind == MingPaiKind.Chi || _.kind == MingPaiKind.BaBei);
            int gangNum = mings.Count(_ => _.kind == MingPaiKind.MingGang || _.kind == MingPaiKind.AnGang);

            IEnumerable<(int, int, bool isMing)> kes = tingPai.blocks
                .Where(_ => _.kind == BlockKind.Ke)
                .Select(_ => (_.signCardKind, _.signCardNum, _.isLastDrewCardExsist && infos.hePaiInfo.isRongHe))
                .Concat(
                    mings
                        .Where(_ => _.kind == MingPaiKind.Peng || _.kind == MingPaiKind.AnGang || _.kind == MingPaiKind.MingGang)
                        .Select(_ => (_.signKind, _.signNum, _.kind != MingPaiKind.AnGang))
                );
            IEnumerable<(int, int, bool isMing)> shuns = tingPai.blocks
                .Where(_ => _.kind == BlockKind.Shun)
                .Select(_ => (_.signCardKind, _.signCardNum, _.isLastDrewCardExsist && infos.hePaiInfo.isRongHe))
                .Concat(
                    mings
                        .Where(_ => _.kind == MingPaiKind.Chi)
                        .Select(_ => (_.signKind, _.signNum, true))
                );
            int anKeNum = kes.Count(_ => !_.isMing);
            int keNum = kes.Count();

            int ziNum = kes.Count(_ => _.Item1 == 3) + (queTou.signCardKind == 3 ? 1 : 0);

            if (hasChangFeng)
                //场风
                result.Add(fanDecoder(YiKind.ChangFeng));
            if (hasZiFeng)
                //自风
                result.Add(fanDecoder(YiKind.ZiFeng));
            if (hasBai)
                //白
                result.Add(fanDecoder(YiKind.Bai));
            if (hasFa)
                //發
                result.Add(fanDecoder(YiKind.Fa));
            if (hasZhong)
                //中
                result.Add(fanDecoder(YiKind.Zhong));
            if (keNum == 4)
                //对对和
                result.Add(fanDecoder(YiKind.DuiDuiHe));
            if (anKeNum >= 3)
                //三暗刻
                result.Add(fanDecoder(YiKind.SanAnKe));
            if (gangNum == 3)
                //三杠子
                result.Add(fanDecoder(YiKind.SanGangZi));
            else if (gangNum == 4)
                //四杠子
                result.Add(fanDecoder(YiKind.SiGangZi));
            if (keNum >= 3 && kes.GroupBy(_ => _.Item2).Any(_ => _.Count() == 3))
                //三色同刻
                result.Add(fanDecoder(YiKind.SanSeTongKe));
            if (keNum == 4
                && kes.All(_ => !(_.Item1 < 3 && (0 < _.Item2 && _.Item2 < 8)))
                && !(queTou.signCardKind < 3 && (0 < queTou.signCardNum && queTou.signCardNum < 8))
            )
            {
                if(ziNum == 0)
                {
                    //清老头
                    result.Add(fanDecoder(YiKind.QingLaoTou));
                }
                else if(ziNum < 5)
                {
                    //混老头
                    result.Add(fanDecoder(YiKind.HunLaoTou));
                }
            }
            {
                int sanYuanNum = kes.Count(_ => _.Item1 == 3 && 4 <= _.Item2 && _.Item2 <= 6);
                if (sanYuanNum == 2 && queTou.signCardKind == 3 && 4 <= queTou.signCardNum && queTou.signCardNum <= 6)
                    //小三元
                    result.Add(fanDecoder(YiKind.XiaoSanYuan));
                else if (sanYuanNum == 3)
                    //大三元
                    result.Add(fanDecoder(YiKind.DaSanYuan));
            }
            if (shuns.GroupBy(_ => _.Item2).Any(_ => _.Count() == 3))
                //三色同顺
                result.Add(fanDecoder(YiKind.SanSeTongKe));
            if (shuns.GroupBy(_ => _.Item1).Any(_ => new int[] { 0, 3, 6 }.Except(_.Select(_ => _.Item2)).Count() == 0))
                //一气贯通
                result.Add(fanDecoder(YiKind.YiQiGuanTong));
            if (keNum != 4 && kes.All(_ => !(_.Item1 < 3 && (0 < _.Item2 && _.Item2 < 8)))
                && shuns.All(_ => !(_.Item1 < 3 && (0 < _.Item2 && _.Item2 < 6)))
                && !(queTou.signCardKind < 3 && (0 < queTou.signCardNum && queTou.signCardNum < 8))
            )
            {
                if (ziNum == 0)
                    //纯全带幺九
                    result.Add(fanDecoder(YiKind.ChunQuanDaiYaoJiu));
                else if (ziNum < 5)
                    //混全带幺九
                    result.Add(fanDecoder(YiKind.HunQuanDaiYaoJiu));
            }
            if (new int[] { 0, 1, 2 }
                    .Except(
                        kes
                            .Concat(shuns)
                            .Append((queTou.signCardKind, queTou.signCardNum, queTou.isLastDrewCardExsist && infos.hePaiInfo.isRongHe))
                            .GroupBy(_ => _)
                            .Select(_ => _.Key.Item1))
                    .Count() == 2
            )
            {
                if (ziNum == 0)
                {
                    //清一色
                    result.Add(fanDecoder(YiKind.QingYiSe));
                    {
                        int[] nums = new int[9] { 3, 1, 1, 1, 1, 1, 1, 1, 3 };
                        tingPai.blocks.ForEach(_ => nums[_.signCardNum] -= 1);
                        if (nums.Count(_ => _ == 0) == 8 && nums.Count(_ => _ == -1) == 1)
                        {
                            if (nums.FindIndex(_ => _ == -1) == queTou.signCardNum)
                                //纯正九莲宝灯
                                result.Add(fanDecoder(YiKind.ChunZhenJiuLianBaoDeng));
                            else
                                //九莲宝灯
                                result.Add(fanDecoder(YiKind.JiuLianBaoDeng));
                        }
                    }
                }
                else
                    //混一色
                    result.Add(fanDecoder(YiKind.HunYiSe));
            }
            {
                int fengNum = kes.Count(_ => _.Item1 == 3 && 0 <= _.Item2 && _.Item2 <= 3);
                if (fengNum == 3 && queTou.signCardKind == 3 && 0 <= queTou.signCardNum && queTou.signCardNum <= 3)
                    //小四喜
                    result.Add(fanDecoder(YiKind.XiaoSiXi));
                else if (fengNum == 4)
                    //大四喜
                    result.Add(fanDecoder(YiKind.DaSiXi));
            }
            if (ziNum == 5)
                //字一色
                result.Add(fanDecoder(YiKind.ZiYiSe));
            if (kes
                .Concat(shuns)
                .Append((queTou.signCardKind, queTou.signCardNum, queTou.isLastDrewCardExsist && infos.hePaiInfo.isRongHe))
                .All(_ => _.Item1 == 2 || (_.Item1 == 3 && _.Item2 == 5)))
                //绿一色
                result.Add(fanDecoder(YiKind.LvYiSe));
            if(anKeNum == 4)
            {
                if (queTou.isLastDrewCardExsist)
                    //四暗刻单骑
                    result.Add(fanDecoder(YiKind.SiAnKeDanJi));
                else
                    //四暗刻
                    result. Add(fanDecoder(YiKind.SiAnKe));
            }
            if (infos.mingInfo.isMenQianQing)
            {
                if (allShun && !hasYiPai && lastDrewCardBlock.kind == BlockKind.Shun && lastDrewCardBlock.signCardNum + 1 != lastDrewCard.huaseNum)
                    //平和
                    result.Add(fanDecoder(YiKind.PingHe));
                {
                    int sameShunNum = tingPai.blocks
                        .Where(_ => _.kind == BlockKind.Shun)
                        .GroupBy(_ => (_.signCardKind, _.signCardNum))
                        .Where(_ => _.Count() >= 2)
                        .Count();
                    if (sameShunNum == 1)
                        //一杯口
                        result.Add(fanDecoder(YiKind.YiBeiKo));
                    else if (sameShunNum == 2)
                        //二杯口
                        result.Add(fanDecoder(YiKind.ErBeiKo));
                }
            }
            if (
                !(tingPai.ExsistCards(new (int kind, int num)[] {
                    (0, 0),(0, 8),(1, 0),(1, 8),(2, 0),(2, 8),
                    (3, 0),(3, 1),(3, 2),(3, 3)
                }) || hasBai || hasFa || hasZhong)
                && !mings.Any(_ => _.ExsistYaoJiu)
            )
            {
                //断幺九
                result.Add(fanDecoder(YiKind.DuanYaoJiu));
            }

            return result;
        }
        private static YiZhongResult CheckNormalYiZhong(CheckInfo infos, CardKind lastDrewCard, DivideResult tingPai, LogicMingPaiGroup[] mings, Func<YiKind, Yi> fanDecoder)
        {
            int[] exsistLastDrewCardBlocks = tingPai.GetExsistCardBlockIndex(lastDrewCard.huaseKind, lastDrewCard.huaseNum);
            YiZhongResult maxResult = new YiZhongResult();

            for(int i = 0; i < exsistLastDrewCardBlocks.Length; ++i)
            {
                tingPai.blocks[i].SwitchLastDrewCard(true);

                YiZhongResult newResult = _CheckNormalYiZhong(infos, lastDrewCard, tingPai, mings, fanDecoder);

                if (maxResult.fanNum < newResult.fanNum)
                {
                    maxResult = newResult;
                }

                tingPai.blocks[i].SwitchLastDrewCard(false);
            }

            return maxResult;
        }
        public static void Check(CheckInfo infos, TingPaiResult tingPai, CardKind[] hands, LogicMingPaiGroup[] mings, CardKind lastDrewCard, Func<YiKind, bool, Yi> fanDecoder)
        {
            Func<YiKind, Yi> bindFanDecoder = _ => fanDecoder(_, infos.mingInfo.isMenQianQing);
            tingPai.additionalYiZhongs = CheckAdditionalYiZhong(infos, bindFanDecoder);
            tingPai.qiDuiYiResult = CheckQiDuiYiZhong(infos, hands, bindFanDecoder);
            tingPai.guoShiYiResult = CheckGuoShiYiZhong(infos, hands, lastDrewCard, bindFanDecoder);
            tingPai.normalYiResult = tingPai.normal.Select(_ => CheckNormalYiZhong(infos, lastDrewCard, _, mings, bindFanDecoder)).ToArray();
        }
    }
}