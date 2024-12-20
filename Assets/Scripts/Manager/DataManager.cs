using Card;
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

    public static int playerNum = 4;
    private static Dictionary<NetworkConnectionToClient, RoomPlayerInfo> roomPlayerInfos;
    public static RoomPlayerInfo[] roomIndexToPlayers;

    public static void OnServerInit()
    {
        roomPlayerInfos = new Dictionary<NetworkConnectionToClient, RoomPlayerInfo>();
        roomIndexToPlayers = new RoomPlayerInfo[playerNum];
    }
    public static int GetFirstEmptySlot()
    {
        for(int i = 0; i < playerNum; ++i)
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
        roomIndexToPlayers[roomPlayerUserInfo.roomSlotIndex] = roomPlayerUserInfo;
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
        return roomPlayerInfos.Count == playerNum && roomPlayerInfos.Count(_ => _.Value.isReady || _.Value.isHost) == roomPlayerInfos.Count;
    }
    public static void RemoveRoomPlayer(NetworkConnectionToClient connection)
    {
        roomPlayerInfos.Remove(connection);
    }
    public static void CancelAllReadyState()
    {
        foreach(var info in roomPlayerInfos)
        {
            info.Value.isReady = false;
        }
    }
    public static NetworkConnectionToClient GetRoomPlayerConnection(int index)
    {
        return roomPlayerInfos.Where(_ => _.Value.uuid == roomIndexToPlayers[index].uuid).First().Key;
    }

    private static int clientSceneChangedNum = 0;
    public static int ClientSceneChangedNum { get { return clientSceneChangedNum; } }

    public static void OnSceneChanged()
    {
        clientSceneChangedNum = 0;
    }
    public static void OnClientSceneChanged()
    {
        clientSceneChangedNum += 1;
    }

    #endregion

    #region 实体牌信息

    public static int cardSkin = 1;
    public static Material realCardBackMat;
    public static Sprite[] handBackSprites;
    private static Sprite[] realCardFaceSprites;

    public static float paiHeCardY = 0.135f;
    public static float paiHeCardNormalHorizentalDistance = 0.14f;
    public static float paiHeLiZhiCardHorizentalDistance = 0.18f;
    public static float paiHeCardVerticalDistance = 0.36f;
    private static Vector2 paiHeStartLeftBorder = new Vector2(-0.7f - paiHeCardNormalHorizentalDistance, -1.17f);
    private static float paiHeBottomestLimit = paiHeStartLeftBorder.y - (4 - 1) * paiHeCardVerticalDistance - 0.001f;
    private static float paiHeMainChunkHorizentalDistance = (2 * 6 + 0.8f) * paiHeCardNormalHorizentalDistance;
    private static float paiHeSubChunkHorizentalDistance = (2 * 4 + 0.8f) * paiHeCardNormalHorizentalDistance;

    public static Vector3 handCardStartPosition = new Vector3(0, 0.23f, -0.1f);
    public static Vector3 handCardNormalDistance = new Vector3(0.265f, 0, 0);
    public static Vector3 handCardNewDistance = new Vector3(0.47f, 0, 0);

    public static Vector2 GetChunkLimit(int chunkIndex)
    {
        Vector2 result = new Vector2(GetChunkStartLeftBorder(chunkIndex).x, paiHeBottomestLimit);
        if(chunkIndex > 0)
        {
            result.x += 2 * 4 * paiHeCardNormalHorizentalDistance;
        }
        else
        {
            result.x += 2 * 6 * paiHeCardNormalHorizentalDistance;
        }
        return result;
    }
    public static Vector2 GetChunkStartLeftBorder(int chunkIndex)
    {
        Vector2 result = paiHeStartLeftBorder;
        if(chunkIndex > 0)
        {
            result.x += paiHeMainChunkHorizentalDistance;
        }
        if (chunkIndex > 1)
        {
            result.x += (chunkIndex - 1) * paiHeSubChunkHorizentalDistance;
        }
        return result;
    }

    public static void LoadCardBackData()
    {
        realCardBackMat = Resources.Load<Material>($"{CardSkins.skinPaths[cardSkin]}/Real/Material");
        handBackSprites = Resources.LoadAll<Sprite>($"{CardSkins.skinPaths[cardSkin]}/Hand/hand");
    }
    public static void LoadCardFaceData()
    {
        realCardFaceSprites = Resources.LoadAll<Sprite>("Card/RealFace");
    }
    public static Sprite GetCardFaceSprite(CardKind faceKind)
    {
        return realCardFaceSprites[faceKind.value];
    }

    #endregion

    #region UI牌信息

    private static Sprite[] normalFace;
    private static int[] faceOrder = new int[] {
        14,6,28,15,22,2,31,25,18,3,
        4,7,35,21,23,8,1,32,26,11,
        5,12,13,29,30,16,9,0,33,19,
        20,34,36,37,24,17,10
    };
    public static Vector3 uiHandCardStartPosition = new Vector3(-680, 0, 0);
    public static Vector3 uiHandCardNormalDistance = new Vector3(89, 0, 0);
    public static Vector3 uiHandCardNewDrewDistance = new Vector3(117, 0, 0);

    public static int uiHandCardConfigurPerGroupCount = 4;
    public static float uiHandCardConfigurDrawPauseDuration = 0.5f;
    public static float uiHandCardConfigurDrawMoveDuration = 0.3f;
    public static float uiHandCardConfigurGroupAppearDuration = 0.5f;

    public void LoadUICardFaceSprites()
    {
        normalFace = Resources.LoadAll<Sprite>("Card/Hand");
    }
    public static Sprite GetUICardFaceSprite(CardKind kind)
    {
        return normalFace[faceOrder[kind.value]];
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

        LoadCardBackData();
        LoadCardFaceData();
        LoadUICardFaceSprites();
    }
    private void OnApplicationQuit()
    {
        SqliteUtils.connection.Close();
    }
}