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
        public ChoiceGang.GangData data;

        public ActionGang(ChoiceGang.GangData data) : base(ActionKind.Gang)
        {
            this.data = data;
        }
    }
    public class ActionChi : Action
    {
        public CardKind[] chi;

        public ActionChi(CardKind[] chi) : base(ActionKind.Chi)
        {
            this.chi = chi;
        }
    }
    public class ActionPeng : Action
    {
        public CardKind[] peng;

        public ActionPeng(CardKind[] peng) : base(ActionKind.Peng)
        {
            this.peng = peng;
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
                    writer.Write(total.data);
                    break;
                }
            case ActionKind.Chi:
                {
                    ActionChi total = (action as ActionChi);
                    writer.WriteArray<CardKind>(total.chi);
                    break;
                }
            case ActionKind.Peng:
                {
                    ActionPeng total = (action as ActionPeng);
                    writer.WriteArray<CardKind>(total.peng);
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
                return new ActionGang(reader.Read<ChoiceGang.GangData>());
            case ActionKind.Chi:
                return new ActionChi(reader.ReadArray<CardKind>());
            case ActionKind.Peng:
                return new ActionPeng(reader.ReadArray<CardKind>());
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