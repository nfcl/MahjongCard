using Card;
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

    private void Awake()
    {
        instance = this;
    }
}
