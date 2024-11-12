using GameLogic;
using GameSceneUI;
using Manager;
using Mirror;
using UnityEngine;

namespace Network
{
    public class Player : NetworkBehaviour
    {

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isLocalPlayer)
            {
                GameSceneUIManager.instance.enterPanel.Open();
                Debug.Log("�ͻ��˼��س������");
                CmdClientSceneChanged();
            }
        }
        [Command]
        public void CmdClientSceneChanged()
        {

            DataManager.OnClientSceneChanged();

            Debug.Log($"�ͻ��˳������� : {DataManager.ClientSceneChangedNum}/{DataManager.playerNum}");

            RpcClientSceneChanged(DataManager.ClientSceneChangedNum, DataManager.playerNum);

            if (DataManager.playerNum == DataManager.ClientSceneChangedNum)
            {
                Debug.Log("����� : ���пͻ��˳����Ѽ������");

                RpcAllClientSceneChanged();

                IGameLogicManager.instance.GameStart();
            }
        }
        [ClientRpc]
        public void RpcClientSceneChanged(int clientSceneChangedNum, int totalPlayerNum)
        {
            GameSceneUIManager.instance.enterPanel.ProcessBarValue = 1.0f * clientSceneChangedNum / totalPlayerNum;
        }
        [ClientRpc]
        public void RpcAllClientSceneChanged()
        {
            Debug.Log("���пͻ��˳����Ѽ������");
        }
    }
}