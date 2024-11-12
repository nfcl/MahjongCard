using Mirror;
using System;
using UnityEngine;
using static MainSceneUI.PropPanel;

namespace Network
{
    public class NetworkRoomManager : Mirror.NetworkRoomManager
    {
        public static NetworkRoomManager instance;

        public D_Void_Void enterRoomCallback;
        public D_Void_Void leaveRoomCallback;

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);

            Debug.Log($"OnClientChangeScene : {newSceneName}");

            if(newSceneName == "Assets/Scenes/GameScene.unity")
            {
                enterRoomCallback = null;
                leaveRoomCallback = null;

                if (RoomPlayer.instance.isServer)
                {
                    DataManager.CancelAllReadyState();
                }

                LogEvent.instance.logTextColors[3] = new Color32(255, 255, 255, 255);
            }
            else
            {
                LogEvent.instance.logTextColors[3] = new Color32(0, 0, 0, 255);
            }
        }

        public override void OnServerChangeScene(string newSceneName)
        {
            base.OnServerChangeScene(newSceneName);
            DataManager.OnSceneChanged();
        }

        public void StartClient(Uri uri, D_Void_Void enterRoomCallback, D_Void_Void leaveRoomCallback)
        {
            this.enterRoomCallback = enterRoomCallback;
            this.leaveRoomCallback = leaveRoomCallback;
            base.StartClient(uri);
        }

        public void StartHost(D_Void_Void enterRoomCallback, D_Void_Void leaveRoomCallback)
        {
            this.enterRoomCallback = enterRoomCallback;
            this.leaveRoomCallback = leaveRoomCallback;
            base.StartHost();
        }

        public override void OnRoomStartHost()
        {
            base.OnRoomStartHost();
            NetworkDiscovery.instance.AdvertiseServer();
        }

        public override void OnRoomStopHost()
        {
            base.OnRoomStopHost();
            NetworkDiscovery.instance.StopDiscovery();
        }

        public override void OnRoomClientConnect()
        {
            base.OnRoomClientConnect();
            Debug.Log($"进入房间");
            enterRoomCallback?.Invoke();
        }

        public override void OnRoomClientDisconnect()
        {
            base.OnRoomClientDisconnect();
            Debug.Log($"离开房间");
            leaveRoomCallback?.Invoke();
        }

        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);
        }

        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerDisconnect(conn);

            if(conn.connectionId != 0)
            {
                RoomPlayer.instance.SendPlayerLeaveMsg(conn);
            }
        }

        private new void Awake()
        {
            base.Awake();
            if (instance != null)
            {
                DestroyImmediate(this.gameObject);
            }
            instance = this;
        }
    }
}