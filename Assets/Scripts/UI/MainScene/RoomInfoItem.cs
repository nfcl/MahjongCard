using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainSceneUI
{
    public class RoomInfoItem : MonoBehaviour
    {
        public Button background;
        public TMP_Text hostNameText;
        public TMP_Text uriText;
        public TMP_Text playerNumText;
        public TMP_Text lastUpdateText;
        public long serverId;
        private float lastUpdateTime;
        private bool isSelected = false;

        private static (Color connected, Color unConnected) backgroundColor = (new Color32(197, 255, 103, 255), new Color32(255, 119, 90, 255));

        public void Init(ServerResponse response, PropPanel.D_Void_Void selectCallback)
        {
            Init(response);
            background.onClick.RemoveAllListeners();
            background.onClick.AddListener(delegate { selectCallback?.Invoke(); });
        }
        public void Init(ServerResponse response)
        {
            hostNameText.text = response.hostName;
            uriText.text = response.uri.ToString();
            serverId = response.serverId;
            playerNumText.text = $"{response.playerNum} / {response.playerCapacity}";
            UpdateTime();
        }
        public void UpdateTime()
        {
            lastUpdateTime = Time.realtimeSinceStartup;
            RefreshTime();
        }
        public void RefreshTime()
        {
            int seconds = (int)(Time.realtimeSinceStartup - lastUpdateTime);
            if(seconds < 1)
            {
                lastUpdateText.text = "먼먼...";
            }
            else
            {
                lastUpdateText.text = $"{seconds}취품...";
            }
            RefreshBackground(seconds);
        }
        public void RefreshBackground(int seconds)
        {
            Color32 baseColor;
            if(seconds > 3)
            {
                baseColor = backgroundColor.unConnected;
            }
            else
            {
                baseColor = backgroundColor.connected;
            }
            if (isSelected)
            {
                background.image.color = Color.Lerp(baseColor, Color.black, 0.2f);
            }
            else
            {
                background.image.color = baseColor;
            }
        }
        public void SwitchSelect(bool isSelected)
        {
            this.isSelected = isSelected;
            RefreshBackground((int)(Time.realtimeSinceStartup - lastUpdateTime));
        }
    }
}