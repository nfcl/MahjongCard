using Data;
using MainSceneUI;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class RoomPlayer : NetworkRoomPlayer
    {
        public static RoomPlayer instance;

        private void Awake()
        {
            instance = this;
        }
        private void OnDestroy()
        {
            instance = null;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isLocalPlayer)
            {
                SendPlayerEnterMsg();
            }
        }

        #region 玩家进出房间消息

        public void SendPlayerEnterMsg()
        {
            CmdSendUserInfo(RoomPlayerInfo.Create(DataManager.userInfo));
        }
        public void SendPlayerLeaveMsg(NetworkConnectionToClient conn)
        {
            RoomPlayerInfo playerInfo = DataManager.GetRoomPlayerUserInfo(conn);

            if (playerInfo != null)
            {
                RpcSendPlayerLeaveRoomMsg(playerInfo.uuid, playerInfo.name);
            }
        }
        [Command(requiresAuthority = false)]
        public void CmdSendUserInfo(RoomPlayerInfo playerInfo, NetworkConnectionToClient sender = null)
        {
            playerInfo.roomSlotIndex = DataManager.GetFirstEmptySlot();
            playerInfo.isReady = false;
            DataManager.AddRoomPlayer(sender, playerInfo);
            Debug.Log("服务器完成信息确认");
            RpcSendPlayerEnterRoomMsg(playerInfo);
            Debug.Log("服务器向全房间播报玩家进入房间");
            TargetSyncAllPlayerToNewPlayer(sender, DataManager.GetAllRoomPlayerUserInfos());
        }
        [ClientRpc]
        public void RpcSendPlayerEnterRoomMsg(RoomPlayerInfo playerInfo)
        {
            Debug.Log($"玩家_{playerInfo.uuid}_{playerInfo.name}进入房间");
            if(playerInfo.uuid != DataManager.userInfo.uuid)
            {
                MainSceneUIManager.instance.roomPanel.AddRoomPlayerItem(playerInfo);
            }
        }
        [TargetRpc]
        public void TargetSyncAllPlayerToNewPlayer(NetworkConnectionToClient connextion, RoomPlayerInfo[] infos)
        {
            MainSceneUIManager.instance.roomPanel.SyncAllPlayerInfos(infos);
        }
        [ClientRpc]
        public void RpcSendPlayerLeaveRoomMsg(string uuid, string name)
        {
            Debug.Log($"玩家_{uuid}_{name}离开房间");
        }

        #endregion

        [Command]
        public void CmdChangeRoomPlayerReadyState(NetworkConnectionToClient sender = null)
        {
            RoomPlayerInfo info = DataManager.GetRoomPlayerUserInfo(sender);
            if (info == null)
            {
                Debug.LogException(new Exception("未知的房间成员"));
                return;
            }
            info.isReady = !info.isReady;
            RpcPlayerReadyStateChanged(info.uuid, info.isReady, DataManager.IsAllRoomPlayerReady());
        }
        [ClientRpc]
        public void RpcPlayerReadyStateChanged(string uuid, bool isReady, bool isAllPlayerReady)
        {
            if (DataManager.userInfo.uuid == uuid)
            {
                MainSceneUIManager.instance.roomPanel.SwitchReadyButtonState(isReady ? RoomPanel.ReadyButtonState.Ready : RoomPanel.ReadyButtonState.NotReady);
            }
            MainSceneUIManager.instance.roomPanel.ChangeSlotReadyState(uuid, isReady);
            if (RoomPlayer.instance.isServer)
            {
                MainSceneUIManager.instance.roomPanel.SwitchReadyButtonState(isAllPlayerReady ? RoomPanel.ReadyButtonState.AllReady : RoomPanel.ReadyButtonState.NotAllReady);
            }
        }
    }
}