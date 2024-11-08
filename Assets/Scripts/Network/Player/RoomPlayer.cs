using Data;
using Mirror;
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
    }
}