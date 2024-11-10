using Card;
using GameSceneUI;
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

    private void Awake()
    {
        instance = this;
    }
}
