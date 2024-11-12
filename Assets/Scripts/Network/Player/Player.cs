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
                Debug.Log("客户端加载场景完毕");
                CmdClientSceneChanged();
            }
        }
        [Command]
        public void CmdClientSceneChanged()
        {

            DataManager.OnClientSceneChanged();

            Debug.Log($"客户端场景加载 : {DataManager.ClientSceneChangedNum}/{DataManager.playerNum}");

            RpcClientSceneChanged(DataManager.ClientSceneChangedNum, DataManager.playerNum);

            if (DataManager.playerNum == DataManager.ClientSceneChangedNum)
            {
                Debug.Log("服务端 : 所有客户端场景已加载完毕");

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
            Debug.Log("所有客户端场景已加载完毕");
        }
    }
}