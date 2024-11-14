using Data;
using Mirror;
using UnityEngine;

namespace Data
{
    public enum ChoiceKind
    {
        None,
        PlayCard
    }
    public class Choice
    {
        public ChoiceKind kind;

        public Choice()
        {
            kind = ChoiceKind.None;
        }
        public Choice(ChoiceKind kind)
        {
            this.kind = kind;
        }
    }
    public class ChoicePlayCard : Choice
    {
        public bool isWhite;
        public CardKind[] cards;

        public ChoicePlayCard() : base(ChoiceKind.PlayCard) { }

        public static ChoicePlayCard NormalPlayCard()
        {
            return new ChoicePlayCard()
            {
                cards = new CardKind[0],
                isWhite = false,
            };
        }
    }
}

public static class ChoiceSerializer
{
    public static void WriteChoiceKind(this NetworkWriter writer, ChoiceKind kind)
    {
        writer.WriteInt((int)kind);
    }
    public static ChoiceKind ReadChoiceKind(this NetworkReader reader)
    {
        return (ChoiceKind)reader.ReadInt();
    }
    public static void WriteChoice(this NetworkWriter writer, Choice choice)
    {
        writer.Write<ChoiceKind>(choice.kind);
        switch (choice.kind)
        {
            case ChoiceKind.PlayCard:
                ChoicePlayCard total = choice as ChoicePlayCard;
                writer.WriteBool(total.isWhite);
                writer.WriteArray(total.cards);
                break;
        }
    }
    public static Choice ReadChoice(this NetworkReader reader)
    {
        ChoiceKind kind = reader.Read<ChoiceKind>();
        switch (kind)
        {
            case ChoiceKind.PlayCard:
                ChoicePlayCard choicePlayCard = new ChoicePlayCard
                {
                    isWhite = reader.ReadBool(),
                    cards = reader.ReadArray<CardKind>(),
                };
                return choicePlayCard;
        }
        return new Choice();
    }
}