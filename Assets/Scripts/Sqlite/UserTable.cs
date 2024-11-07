using Data;
using SQLite4Unity3d;

namespace SQLiteTable
{
    public class UserTable
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        public string uuid { get; set; }
        public UserInfo ToUserInfo()
        {
            return new UserInfo
            {
                id = id,
                name = name,
                uuid = uuid
            };
        }
        public static UserTable FromUserInfo(UserInfo info)
        {
            return new UserTable
            {
                id = info.id,
                name = info.name,
                uuid = info.uuid
            };
        }
        public static int Insert(UserTable table)
        {
            if(SqliteUtils.connection.Table<UserTable>().Count(_=>_.name == table.name) > 0)
            {
                return -1;
            }
            return SqliteUtils.connection.Insert(table);
        }
        public static bool Update(UserTable table)
        {
            if (SqliteUtils.connection.Table<UserTable>().Count(_ => _.id != table.id && _.name == table.name) > 0)
            {
                return false;
            }
            SqliteUtils.connection.Update(table);
            return true;
        }
        public static int Delete(UserTable table)
        {
            return SqliteUtils.connection.Delete(table);
        }
        public static UserTable FindById(int id)
        {
            return SqliteUtils.connection.Table<UserTable>().Where(_ => _.id == id).FirstOrDefault();
        }
        public static UserTable FindByName(string name)
        {
            return SqliteUtils.connection.Table<UserTable>().Where(_ => _.name == name).FirstOrDefault();
        }
    }
}