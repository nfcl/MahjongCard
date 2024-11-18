using GameSceneUI;

namespace Data
{
    public class TingPai
    {
        public CardKind card;
        public int lastNum;
        public bool isZhenTing;
        public bool isWuYi;
        public bool isFanFu;

        public CardChoicePanel.ChoiceCardState GetState()
        {
            return
                    isFanFu ?   CardChoicePanel.ChoiceCardState.FanFu
                :   isWuYi  ?   CardChoicePanel.ChoiceCardState.WuYi
                :               CardChoicePanel.ChoiceCardState.LastNum;
        }
    }
}