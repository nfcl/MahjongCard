using Data;
using Mirror;

namespace Data
{
    public enum ActionKind
    {
        None = 0,
        PlayCard = 1
    };
    public class Action
    {
        public ActionKind kind;

        public Action() { }
        public Action(ActionKind kind)
        {
            this.kind = kind;
        }
        public static void ToString(Action[] actions)
        {

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

public static class ActionSerializer
{
    public static void ActionKindWriter(this NetworkWriter writer, ActionKind kind)
    {
        writer.WriteInt((int)kind);
    }
    public static ActionKind ActionKindReader(this NetworkReader reader)
    {
        return (ActionKind)reader.ReadInt();
    }
    public static void ActionWriter(this NetworkWriter writer, Action action)
    {
        writer.Write<ActionKind>(action.kind);
        switch (action.kind)
        {
            case ActionKind.None:
                break;
            case ActionKind.PlayCard:
                ActionPlayCard total = (action as ActionPlayCard);
                writer.Write<CardKind>(total.card);
                break;
        }
    }
    public static Action ActionReader(this NetworkReader reader)
    {
        ActionKind kind = reader.Read<ActionKind>();
        switch (kind)
        {
            case ActionKind.None:
                return new Action(kind);
            case ActionKind.PlayCard:
                return new ActionPlayCard(reader.Read<CardKind>());
        }
        return null;
    }
}