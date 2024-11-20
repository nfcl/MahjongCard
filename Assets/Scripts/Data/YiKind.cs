using System.Collections.Generic;

namespace Data
{
    public enum YiKind
    {
        DuanYaoJiu,
        ZiFeng,
        ChangFeng,
        Zhong,
        Fa,
        Bai,
        HeDiMoYu,
        LingShangKaiHua,
        QiangGang,
        HaiDiLaoYue,
        LiZhi,
        YiFa,
        MenQianQingZiMoHe,
        PingHe,
        YiBeiKo,
        DuiDuiHe,
        SanAnKe,
        SanGangZi,
        SanSeTongKe,
        HunLaoTou,
        XiaoSanYuan,
        SanSeTongShun,
        YiQiGuanTong,
        HunQuanDaiYaoJiu,
        QiDuiZi,
        LiangLiZhi,
        HunYiSe,
        ChunQuanDaiYaoJiu,
        ErBeiKo,
        LiuJuManGuan,
        QingYiSe,
        DaSanYuan,
        XiaoSiXi,
        DaSiXi,
        ZiYiSe,
        LvYiSe,
        QingLaoTou,
        GuoShiWuShuang,
        GuoShiWuShuang13Mian,
        SiAnKe,
        SiAnKeDanJi,
        JiuLianBaoDeng,
        ChunZhenJiuLianBaoDeng,
        TianHe,
        DiHe,
        SiGangZi
    }
    public class Yi
    {
        public YiKind kind;
        public int fan;

        public static Yi Default(YiKind kind, bool ming = false)
        {
            return new Yi
            {
                kind = kind,
                fan = kind switch
                {
                    YiKind.DuanYaoJiu               => 1,
                    YiKind.ZiFeng                   => 1,
                    YiKind.ChangFeng                => 1,
                    YiKind.Zhong                    => 1,
                    YiKind.Fa                       => 1,
                    YiKind.Bai                      => 1,
                    YiKind.HeDiMoYu                 => 1,
                    YiKind.LingShangKaiHua          => 1,
                    YiKind.QiangGang                => 1,
                    YiKind.HaiDiLaoYue              => 1,
                    YiKind.LiZhi                    => ming ? 0 : 1,
                    YiKind.YiFa                     => ming ? 0 : 1,
                    YiKind.MenQianQingZiMoHe        => ming ? 0 : 1,
                    YiKind.PingHe                   => ming ? 0 : 1,
                    YiKind.YiBeiKo                  => ming ? 0 : 1,
                    YiKind.DuiDuiHe                 => 2,
                    YiKind.SanAnKe                  => 2,
                    YiKind.SanGangZi                => 2,
                    YiKind.SanSeTongKe              => 2,
                    YiKind.HunLaoTou                => 2,
                    YiKind.XiaoSanYuan              => 2,
                    YiKind.SanSeTongShun            => ming ? 1 : 2,
                    YiKind.YiQiGuanTong             => ming ? 1 : 2,
                    YiKind.HunQuanDaiYaoJiu         => ming ? 1 : 2,
                    YiKind.QiDuiZi                  => ming ? 0 : 2,
                    YiKind.LiangLiZhi               => ming ? 0 : 2,
                    YiKind.HunYiSe                  => ming ? 2 : 3,
                    YiKind.ChunQuanDaiYaoJiu        => ming ? 2 : 3,
                    YiKind.ErBeiKo                  => ming ? 0 : 3,
                    YiKind.LiuJuManGuan             => 5,
                    YiKind.QingYiSe                 => ming ? 5 : 6,
                    YiKind.DaSanYuan                => 13,
                    YiKind.XiaoSiXi                 => 13,
                    YiKind.DaSiXi                   => 13,
                    YiKind.ZiYiSe                   => 13,
                    YiKind.LvYiSe                   => 13,
                    YiKind.QingLaoTou               => 13,
                    YiKind.SiGangZi                 => 13,
                    YiKind.GuoShiWuShuang           => ming ? 0 : 13,
                    YiKind.GuoShiWuShuang13Mian     => ming ? 0 : 13,
                    YiKind.SiAnKe                   => ming ? 0 : 13,
                    YiKind.SiAnKeDanJi              => ming ? 0 : 13,
                    YiKind.JiuLianBaoDeng           => ming ? 0 : 13,
                    YiKind.ChunZhenJiuLianBaoDeng   => ming ? 0 : 13,
                    YiKind.TianHe                   => ming ? 0 : 13,
                    YiKind.DiHe                     => ming ? 0 : 13,
                    _ => -1
                }
            };
        }
        public static void CheckTingPaiChoice(CardKind[] hands, LogicMingPaiGroup mings)
        {

            #region 可在手牌中检测

            //√断幺九
            //√中
            //√发
            //√白
            //√一杯口
            //√对对和
            //√三暗刻
            //√三杠子
            //√三色同刻
            //√混老头
            //√小三元
            //√三色同顺
            //√一气贯通
            //√混全带幺九
            //√七对子
            //√混一色
            //√纯全带幺九
            //√二杯口
            //√清一色
            //√大三元
            //√小四喜
            //√大四喜
            //√字一色
            //√绿一色
            //√清老头
            //√国士无双
            //√国士无双13面
            //√四暗刻
            //√四暗刻单骑
            //√九莲宝灯
            //√纯正九莲宝灯
            //√四杠子

            #endregion

            #region 需搭配对局信息

            //役种        需要知道
            //√自风        自风
            //√场风        场风
            //√河底摸鱼     剩余牌数，荣和
            //√岭上开花     岭上牌，自摸
            //√抢杠        岭上牌，荣和
            //√海底捞月     剩余牌数，自摸
            //√立直        立直
            //√门前清自摸和 门前清，自摸
            //√一发        立直后抽牌数，立直后鸣牌数
            //√平和        自风场风
            //√两立直      立直时自身抽牌数，立直时鸣牌数
            //×流局满贯     剩余牌数，是否被碰吃杠，是否打过非幺九
            //√天和        是否庄家，摸牌数，鸣牌数
            //√地和        是否子家，摸牌数，鸣牌数

            #endregion
        }
    }
}