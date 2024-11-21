using Checker;
using Data;
using Mirror;
using System;
using System.Linq;

namespace Data
{
    public struct ClientEachCardTingPais
    {
        public ClientCardTingPai[] cards;

        public static ClientEachCardTingPais Create(EachHandCardTingPaiResult data, int fanFu, Func<CardKind, int> lastCardCalculate, Func<int, int, bool> zhenTingChecker)
        {
            ClientEachCardTingPais result = new()
            {
                cards = new ClientCardTingPai[data.choices.Length]
            };

            data.choices.Foreach((choice, index) =>
                result.cards[index] = new ClientCardTingPai(
                    zhenTingChecker(choice.Item1.Item1, choice.Item1.Item2),
                    choice, fanFu, lastCardCalculate
                )
            );

            return result;
        }
    }
    public struct ClientCardTingPai
    {
        public CardKind playCard;
        public bool isZhenTing;
        public ClientCardTingPaiResult[] tingPais;

        public ClientCardTingPai(bool isZhenTing, ((int, int), EachDrewCardTingPaiResult[]) choice, int fanFu, Func<CardKind, int> lastCardCalculate)
        {
            playCard = new CardKind(choice.Item1.Item1, choice.Item1.Item2);
            this.isZhenTing = isZhenTing;
            tingPais = new ClientCardTingPaiResult[choice.Item2.Length];
            for(int i = 0; i < choice.Item2.Length; ++i)
            {
                tingPais[i] = new ClientCardTingPaiResult
                {
                    tingPai = choice.Item2[i].lastDrewCard
                };
                if (choice.Item2[i].tingPai.IsWuYi)
                {
                    tingPais[i].state = ClientCardTingPaiState.WuYi;
                }
                else if (choice.Item2[i].tingPai.BestChoice().fanNum <fanFu)
                {
                    tingPais[i].state = ClientCardTingPaiState.FanFu;
                }
                else
                {
                    tingPais[i].state = ClientCardTingPaiState.last;
                    tingPais[i].lastNum = lastCardCalculate(tingPais[i].tingPai);
                }
            }
        }
    }
    public enum ClientCardTingPaiState
    {
        last,
        WuYi,
        FanFu
    }
    public struct ClientCardTingPaiResult
    {
        public CardKind tingPai;
        public ClientCardTingPaiState state;
        public int lastNum;
    }
}

public static class ClientCardTingPaiSerializer
{
    public static void ClientEachCardTingPaisWriter(this NetworkWriter writer, ClientEachCardTingPais content)
    {
        writer.WriteArray(content.cards);
    }
    public static ClientEachCardTingPais ClientEachCardTingPaisReader(this NetworkReader reader)
    {
        return new ClientEachCardTingPais
        {
            cards = reader.ReadArray<ClientCardTingPai>()
        };
    }
    public static void ClientCardTingPaiWriter(this NetworkWriter writer, ClientCardTingPai content)
    {
        writer.Write<CardKind>(content.playCard);
        writer.WriteArray(content.tingPais);
    }
    public static ClientCardTingPai ClientCardTingPaiReader(this NetworkReader reader)
    {
        return new ClientCardTingPai
        {
            playCard = reader.Read<CardKind>(),
            tingPais = reader.ReadArray<ClientCardTingPaiResult>()
        };
    }
    public static void ClientCardTingPaiResultWriter(this NetworkWriter writer, ClientCardTingPaiResult content)
    {
        writer.Write<CardKind>(content.tingPai);
        writer.Write<ClientCardTingPaiState>(content.state);
        writer.WriteInt(content.lastNum);
    }
    public static ClientCardTingPaiResult ClientCardTingPaiResultReader(this NetworkReader reader)
    {
        return new ClientCardTingPaiResult
        {
            tingPai = reader.Read<CardKind>(),
            state = reader.Read<ClientCardTingPaiState>(),
            lastNum = reader.ReadInt()
        };
    }
    public static void ClientCardTingPaiStateWriter(this NetworkWriter writer, ClientCardTingPaiState content)
    {
        writer.WriteInt((int)content);
    }
    public static ClientCardTingPaiState ClientCardTingPaiStateReader(this NetworkReader reader)
    {
        return (ClientCardTingPaiState)reader.ReadInt();
    }
}