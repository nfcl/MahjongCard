using Card;
using Data;
using GameLogic;
using GameSceneUI;
using UnityEngine;

public class DesktopManager : MonoBehaviour
{
    public static DesktopManager instance;

    #region �м������

    public DesktopPlayerInfoItem[] playerInfoItems;
    public TextMesh changInfoText;
    public TextMesh lastCardText;

    #endregion

    #region ʵ���ƹ���

    public RealCard prefab;
    public PaiHe[] paiHes;
    public HandCard[] handCards;

    #endregion

    public void SyncScore(int[] messages)
    {
        for (int i = 0; i < messages.Length; ++i)
        {
            playerInfoItems[i].ShowRes(messages[IGameLogicManager.instance.GetplayerIndex(i)]);
        }
    }
    public void SyncRound(FengKind round, int ju)
    {
        int zhuang = IGameLogicManager.instance.GetZhuangPlayerIndex();
        for (int i = 0; i < DataManager.playerNum; ++i)
        {
            int feng = (zhuang + i) % DataManager.playerNum;
            playerInfoItems[feng].SetFeng((FengKind)i);
        }
        changInfoText.text = $"{round.toString()}{ju}��";
    }

    private void Awake()
    {
        instance = this;
    }
}
