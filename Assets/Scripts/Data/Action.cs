namespace Data
{
    public enum ActionKind
    {
        None = 0,
        PlayCard = 1
    };
    public abstract class Action
    {
        public ActionKind kind;

        public Action(ActionKind kind)
        {
            this.kind = kind;
        }
    }
    public class ActionPlayCard : Action
    {
        public CardKind card;

        public ActionPlayCard(CardKind card) : base(ActionKind.PlayCard)
        {
            this.card = card;
        }
    }
}