using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerInfoItem : MonoBehaviour
{
    public Image background;
    public TMP_Text nameText;
    public Image hostIcon;
    public Image readyIcon;
    public static (Color32 hasPlayer, Color32 hasntPlayer) backgroundColors = (new Color32(200, 200, 200, 255), new Color32(255, 255, 255, 255));
    public static (Color32 isReady, Color32 notReady) readyIconColors = (new Color32(0, 255, 0, 255), new Color32(255, 0, 0, 255));
    public string uuid;

    public void Init(RoomPlayerInfo playerInfo)
    {
        uuid = playerInfo.uuid;
        hostIcon.gameObject.SetActive(playerInfo.isHost);
        readyIcon.color = readyIconColors.notReady;
        readyIcon.gameObject.SetActive(!playerInfo.isHost);
        nameText.text = playerInfo.name;
        background.color = backgroundColors.hasPlayer;
    }
    public void Clear()
    {
        uuid = "";
        readyIcon.gameObject.SetActive(false);
        hostIcon.gameObject.SetActive(false);
        nameText.text = "";
        background.color = backgroundColors.hasntPlayer;
    }
    public void ChangeReadyState(bool isReady)
    {
        readyIcon.color = isReady ? readyIconColors.isReady : readyIconColors.notReady;
    }
}
