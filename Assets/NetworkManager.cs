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
            Debug.Log("�����������Ϣȷ��");
            RpcSendPlayerEnterRoomMsg(userInfo.uuid, userInfo.name);
            Debug.Log("��������ȫ���䲥����ҽ��뷿��");
        }

        public void SendPlayerLeaveMsg(NetworkConnectionToClient conn)
        {
            #region ֪ͨ�������������뿪�������Ϣ

            UserInfo userInfo = DataManager.GetRoomPlayerUserInfo(conn);

            if (userInfo != null)
            {
                RpcSendPlayerLeaveRoomMsg(userInfo.uuid, userInfo.name);
            }

            #endregion
        }

        #region ��ҽ���������Ϣ

        [ClientRpc]
        public void RpcSendPlayerEnterRoomMsg(string uuid, string name)
        {
            Debug.Log($"���_{uuid}_{name}���뷿��");
        }
        [ClientRpc]
        public void RpcSendPlayerLeaveRoomMsg(string uuid, string name)
        {
            Debug.Log($"���_{uuid}_{name}�뿪����");
        }

        #endregion
    }
}