using Data;

namespace Checker
{
    public struct RoundInfo
    {
        public FengKind changFeng;
        public int lastCardNum;
        public bool isNoBodyMingPai;
    }
    public struct MingInfo
    {
        public bool isMingPai;
        public bool isMenQianQing;
    }
    public struct SelfInfo
    {
        public FengKind ziFeng;
        public int drewCardNum;
        public bool isZhuang;
        public bool isLiZhi;
        public bool isLiangLiZhi;
        public bool IsLiZhi => isLiangLiZhi || isLiZhi;
        /// <summary>
        /// 一发取消的时机:打出一张牌|别人鸣牌|自己暗杠
        /// </summary>
        public bool hasYiFa;
    }
    public struct HePaiInfo
    {
        public bool isRongHe;
        public bool isLingShangPai;
    }
    public struct CheckInfo
    {
        public RoundInfo roundInfo;
        public SelfInfo selfInfo;
        public MingInfo mingInfo;
        public HePaiInfo hePaiInfo;
    }
}