using System.Collections.Generic;

namespace Data
{
    public class LogicMingPai
    {
        private List<LogicMingPaiGroup> groups;

        public LogicMingPai()
        {
            groups = new List<LogicMingPaiGroup>();
        }
        public void MingCard(LogicMingPaiGroup group)
        {
            groups.Add(group);
        }
    }
}