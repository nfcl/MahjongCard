using SQLite4Unity3d;
using SQLiteTable;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class SqliteUtils
{
    private static SQLiteConnection _connection;
    public static SQLiteConnection connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = GetConnection();
            }
            return _connection;
        }
    }
    private static SQLiteConnection GetConnection()
    {
        string dbPath = UnityEngine.Application.streamingAssetsPath;
        string dbName = "db.db";
        SQLiteConnection conn = null;
        if (!Directory.Exists(dbPath))
        {
            Directory.CreateDirectory(dbPath);
        }
        else
        {
            if (!File.Exists($"{dbPath}/{dbName}"))
            {
                conn = new SQLiteConnection($"{dbPath}/{dbName}", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                UnityEngine.Debug.Log("创建数据库成功");
                CreateTable(conn);
            }
            else
            {
                conn = new SQLiteConnection($"{dbPath}/{dbName}", SQLiteOpenFlags.ReadWrite);
            }
        }
        return conn;
    }
    private static void CreateTable(SQLiteConnection connection)
    {
        connection.CreateTable<UserTable>();
        UnityEngine.Debug.Log("创建UserTable成功");
    }
}
