using Data;
using Mirror;
using Network;

namespace Data
{
    public class RoomPlayerInfo
    {
        public int id;
        public string uuid;
        public string name;
        public int roomSlotIndex;
        public bool isReady;
        public bool isHost;

        public static RoomPlayerInfo Create(UserInfo userInfo)
        {
            return new RoomPlayerInfo
            {
                id = userInfo.id,
                uuid = userInfo.uuid,
                name = userInfo.name,
                roomSlotIndex = -1,
                isReady = false,
                isHost = RoomPlayer.instance.isServer
            };
        }
    }
}

public static class RoomPlayerInfoSerializer
{
    public static void Writer(this NetworkWriter writer, RoomPlayerInfo value)
    {
        writer.WriteInt(value.id);
        writer.WriteString(value.uuid);
        writer.WriteString(value.name);
        writer.WriteInt(value.roomSlotIndex);
        writer.WriteBool(value.isReady);
        writer.WriteBool(value.isHost);
    }
    public static RoomPlayerInfo Reader(this NetworkReader reader)
    {
        return new RoomPlayerInfo
        {
            id = reader.ReadInt(),
            uuid = reader.ReadString(),
            name = reader.ReadString(),
            roomSlotIndex = reader.ReadInt(),
            isReady = reader.ReadBool(),
            isHost = reader.ReadBool()
        };
    }
}