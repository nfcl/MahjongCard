using Data;
using Mirror;
using SQLiteTable;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private static Dictionary<NetworkConnectionToClient, RoomPlayerInfo> roomPlayerInfos;

    public static void OnServerInit()
    {
        roomPlayerInfos = new Dictionary<NetworkConnectionToClient, RoomPlayerInfo>();
    }
    public static int GetFirstEmptySlot()
    {
        for(int i = 0; i < 4; ++i)
        {
            if(roomPlayerInfos.Count(_=>_.Value.roomSlotIndex == i) == 0)
            {
                return i;
            }
        }
        return -1;
    }
    public static int AddRoomPlayer(NetworkConnectionToClient connection, RoomPlayerInfo roomPlayerUserInfo)
    {
        roomPlayerInfos[connection] = roomPlayerUserInfo;
        return roomPlayerUserInfo.roomSlotIndex;
    }
    public static RoomPlayerInfo GetRoomPlayerUserInfo(NetworkConnectionToClient connection)
    {
        if(roomPlayerInfos.TryGetValue(connection, out RoomPlayerInfo userInfo))
        {
            return userInfo;
        }
        return null;
    }
    public static RoomPlayerInfo[] GetAllRoomPlayerUserInfos()
    {
        return roomPlayerInfos.Values.ToArray();
    }
    public static bool IsAllRoomPlayerReady()
    {
        return roomPlayerInfos.Count == 4 && roomPlayerInfos.Count(_ => _.Value.isReady) == roomPlayerInfos.Count;
    }
    public static void RemoveRoomPlayer(NetworkConnectionToClient connection)
    {
        roomPlayerInfos.Remove(connection);
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