using Data;
using Mirror;

namespace Data
{
    public class UserInfo
    {
        public int id;
        public string name;
        public string uuid;
    }
}

public static class UserInfoSerializer
{
    public static void Writer(this NetworkWriter writer, UserInfo value)
    {
        writer.WriteInt(value.id);
        writer.WriteString(value.name);
        writer.WriteString(value.uuid);
    }
    public static UserInfo Reader(this NetworkReader reader)
    {
        return new UserInfo
        {
            id = reader.ReadInt(),
            name = reader.ReadString(),
            uuid = reader.ReadString()
        };
    }
}