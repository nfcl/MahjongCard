using Card;
using Data;
using GameLogic;
using GameSceneUI;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DesktopManager : MonoBehaviour
{
    public static DesktopManager instance;

    #region 中间的盘子

    public DesktopPlayerInfoItem[] playerInfoItems;
    public TextMesh changInfoText;
    public TextMesh lastCardText;

    #endregion

    #region 实体牌管理

    public RealCard prefab;
    public PaiHe[] paiHes;
    public HandCard[] handCards;

    #endregion

    public void OnPlayerPlayCard(int playerIndex, CardKind card, bool isZhi)
    {
        int relativeIndex = IGameLogicManager.instance.GetAbsolutePlayerIndex(playerIndex);
        paiHes[relativeIndex].AddCard(handCards[relativeIndex].PlayCard(card), isZhi);
    }
    public void OnPlayerRound(int index)
    {
        int relativeIndex = IGameLogicManager.instance.GetAbsolutePlayerIndex(index);
        playerInfoItems.Foreach((_, index) => _.SetWaitBarState(index == relativeIndex));
    }
    public void ClearDesktop()
    {
        for (int i = 0; i < paiHes.Length; ++i)
        {
            paiHes[i].Clear();
        }
        for (int i = 0; i < handCards.Length; ++i)
        {
            handCards[i].Clear();
        }
    }
    public void SyncScore(int[] messages)
    {
        for (int i = 0; i < messages.Length; ++i)
        {
            playerInfoItems[i].ShowRes(messages[IGameLogicManager.instance.GetRelativeplayerIndex(i)]);
        }
    }
    public void SyncRound(FengKind feng, int ju)
    {
        int zhuang = IGameLogicManager.instance.GetZhuangPlayerIndex();
        int zhuangRelative = IGameLogicManager.instance.GetAbsolutePlayerIndex(zhuang);
        for (int i = 0; i < DataManager.playerNum; ++i)
        {
            int temp = (zhuangRelative + i) % DataManager.playerNum;
            playerInfoItems[temp].SetFeng((FengKind)i);
        }
        changInfoText.text = $"{feng.toString()}{ju}局";
    }

    private void Awake()
    {
        instance = this;
    }
}
