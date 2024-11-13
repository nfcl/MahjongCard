using Data;
using Message;
using Mirror;
using System.Text;

namespace Message
{
    public class CardsMessage
    {
        public CardKind[] cards;

        public CardsMessage() { }
        public CardsMessage(CardKind[] cards)
        {
            this.cards = cards;
        }

        public override string ToString()
        {
            return CardKind.ToString(cards);
        }

        public static string ToString(CardsMessage[] messages)
        {
            if (messages == null)
            {
                return "null";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var _ in messages)
            {
                if (_ == null)
                {
                    sb.Append("null\n");
                }
                else
                {
                    sb.Append($"{_.ToString()}\n");
                }
            }
            return sb.ToString();
        }
    }
    public class DrawCardMessage
    {
        public int playerIndex;
        public CardKind card;
    }
}

public static class MessageSerializer
{
    public static void WriteCardsMessage(NetworkWriter writer, CardsMessage message)
    {
        writer.WriteArray(message.cards);
    }
    public static CardsMessage ReadCardsMessage(NetworkReader reader)
    {
        return new CardsMessage { cards = reader.ReadArray<CardKind>() };
    }
    public static void WriteDrawCardMessage(NetworkWriter writer, DrawCardMessage message)
    {
        writer.WriteInt(message.playerIndex);
        writer.Write<CardKind>(message.card);
    }
    public static DrawCardMessage ReadDrawCardMessage(NetworkReader reader)
    {
        return new DrawCardMessage
        {
            playerIndex = reader.ReadInt(),
            card = reader.Read<CardKind>()
        };
    }
}