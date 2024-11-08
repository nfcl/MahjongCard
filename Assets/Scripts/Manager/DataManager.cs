using Data;
using Mirror;
using SQLiteTable;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    #region 用户信息

    private static int userId;
    public static UserInfo userInfo
    {
        get
        {
            return UserTable.FindById(userId).ToUserInfo();
        }
    }
    public static bool Login(string name)
    {
        name = name.Trim();
        if(name.Length == 0)
        {
            return false;
        }
        UserTable rec = UserTable.FindByName(name);
        if(rec == null)
        {
            int newId = Register(name);
            if(newId != -1)
            {
                userId = newId;
                return true;
            }
            return false;
        }
        userId = rec.id;
        return true;
    }
    public static int Register(string name)
    {
        UserTable newUser = new UserTable
        {
            name = name,
            uuid = UUIDGenertor()
        };
        return UserTable.Insert(newUser);
    }
    public static string UUIDGenertor()
    {
        string randomStartTrim = UnityEngine.Random.Range(0, 1000000).ToString("000000");
        string timestamp = DateTime.Now.Ticks.ToString();
        return $"{randomStartTrim}_{timestamp}";
    }

    #endregion

    #region 房间信息

    private static Dictionary<NetworkConnectionToClient, UserInfo> roomPlayerUserInfos;

    public static void OnServerInit()
    {
        roomPlayerUserInfos = new Dictionary<NetworkConnectionToClient, UserInfo>();
    }
    public static void AddRoomPlayer(NetworkConnectionToClient connection, UserInfo roomPlayerUserInfo)
    {
        roomPlayerUserInfos[connection] = roomPlayerUserInfo;
    }
    public static UserInfo GetRoomPlayerUserInfo(NetworkConnectionToClient connection)
    {
        if(roomPlayerUserInfos.TryGetValue(connection, out UserInfo userInfo))
        {
            return userInfo;
        }
        return null;
    }

    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }
    private void OnApplicationQuit()
    {
        SqliteUtils.connection.Close();
    }
}