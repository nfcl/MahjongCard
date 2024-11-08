using Data;
using DG.Tweening;
using Mirror;
using UnityEngine;

namespace Network
{
    public class NetworkManager : NetworkBehaviour
    {
        public static NetworkManager instance;

        private void Awake()
        {
            if(instance != null)
            {
                DestroyImmediate(this.gameObject);
            }
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            CmdSendUserInfo(DataManager.userInfo);
        }

        [Command(requiresAuthority = false)]
        public void CmdSendUserInfo(UserInfo userInfo, NetworkConnectionToClient sender = null)
        {
            DataManager.AddRoomPlayer(sender, userInfo);
            Debug.Log("服务器完成信息确认");
            RpcSendPlayerEnterRoomMsg(userInfo.uuid, userInfo.name);
            Debug.Log("服务器向全房间播报玩家进入房间");
        }

        public void SendPlayerLeaveMsg(NetworkConnectionToClient conn)
        {
            #region 通知所有玩家有玩家离开房间的消息

            UserInfo userInfo = DataManager.GetRoomPlayerUserInfo(conn);

            if (userInfo != null)
            {
                RpcSendPlayerLeaveRoomMsg(userInfo.uuid, userInfo.name);
            }

            #endregion
        }

        #region 玩家进出房间消息

        [ClientRpc]
        public void RpcSendPlayerEnterRoomMsg(string uuid, string name)
        {
            Debug.Log($"玩家_{uuid}_{name}进入房间");
        }
        [ClientRpc]
        public void RpcSendPlayerLeaveRoomMsg(string uuid, string name)
        {
            Debug.Log($"玩家_{uuid}_{name}离开房间");
        }

        #endregion
    }
}