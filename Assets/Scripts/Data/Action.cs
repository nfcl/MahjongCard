using Data;
using Mirror;

namespace Data
{
    public enum ActionKind
    {
        None,
        Skip,
        PlayCard,
        LiZhi,
        Gang,
        Chi,
        Peng,
        JiuZhongJiuPai,
        ZiMo,
        RongHe
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
    public class ActionSkip : Action
    {
        public ActionSkip() : base(ActionKind.Skip) { }
    }
    public class ActionPlayCard : Action
    {
        public CardKind card;

        public ActionPlayCard(CardKind card) : base(ActionKind.PlayCard)
        {
            this.card = card;
        }
    }
    public class ActionLiZhi : Action
    {
        public CardKind card;

        public ActionLiZhi(CardKind card) : base(ActionKind.LiZhi)
        {
            this.card = card;
        }
    }
    public class ActionGang : Action
    {
        public int index;

        public ActionGang(int index) : base(ActionKind.Gang)
        {
            this.index = index;
        }
    }
    public class ActionChi : Action
    {
        public int index;

        public ActionChi(int index) : base(ActionKind.Chi)
        {
            this.index = index;
        }
    }
    public class ActionPeng : Action
    {
        public int index;

        public ActionPeng(int index) : base(ActionKind.Peng)
        {
            this.index = index;
        }
    }
    public class ActionJiuZhongJiuPai : Action
    {
        public ActionJiuZhongJiuPai() : base(ActionKind.JiuZhongJiuPai) { }
    }
    public class ActionZimo : Action
    {
        public ActionZimo() : base(ActionKind.ZiMo) { }
    }
    public class ActionRongHe : Action
    {
        public ActionRongHe() : base(ActionKind.RongHe) { }
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
            case ActionKind.PlayCard:
                {
                    ActionPlayCard total = (action as ActionPlayCard);
                    writer.Write<CardKind>(total.card);
                    break;
                }
            case ActionKind.LiZhi:
                {
                    ActionLiZhi total = (action as ActionLiZhi);
                    writer.Write<CardKind>(total.card);
                    break;
                }
            case ActionKind.Gang:
                {
                    ActionGang total = (action as ActionGang);
                    writer.WriteInt(total.index);
                    break;
                }
            case ActionKind.Chi:
                {
                    ActionChi total = (action as ActionChi);
                    writer.WriteInt(total.index);
                    break;
                }
            case ActionKind.Peng:
                {
                    ActionPeng total = (action as ActionPeng);
                    writer.WriteInt(total.index);
                    break;
                }

        }
    }
    public static Action ActionReader(this NetworkReader reader)
    {
        ActionKind kind = reader.Read<ActionKind>();
        switch (kind)
        {
            case ActionKind.None:
                return new Action(kind);
            case ActionKind.Skip:
                return new ActionSkip();
            case ActionKind.PlayCard:
                return new ActionPlayCard(reader.Read<CardKind>());
            case ActionKind.LiZhi:
                return new ActionLiZhi(reader.Read<CardKind>());
            case ActionKind.Gang:
                return new ActionGang(reader.ReadInt());
            case ActionKind.Chi:
                return new ActionChi(reader.ReadInt());
            case ActionKind.Peng:
                return new ActionPeng(reader.ReadInt());
            case ActionKind.JiuZhongJiuPai:
                return new ActionJiuZhongJiuPai();
            case ActionKind.ZiMo:
                return new ActionZimo();
            case ActionKind.RongHe:
                return new ActionRongHe();
        }
        return null;
    }
}