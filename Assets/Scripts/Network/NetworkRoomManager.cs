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