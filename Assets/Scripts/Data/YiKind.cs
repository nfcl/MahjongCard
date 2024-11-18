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

        public Yi Default(YiKind kind, bool ming = false)
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
    }
}