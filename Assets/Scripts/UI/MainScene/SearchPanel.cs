using DG.Tweening;
using Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MainSceneUI
{
    public class SearchPanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public RectTransform itemContainer;
        public RoomInfoItem infoItemPrefab;
        public List<RoomInfoItem> items;
        public RoomInfoItem selectItem = null;
        private Sequence roomUpdateTime;

        private void Start()
        {
            roomUpdateTime = DOTween.Sequence().AppendCallback(UpdateAllRoomsTime).AppendInterval(0.1f).SetLoops(-1).Pause();
        }
        public void Open()
        {
            ClearInfos();
            canvasGroup.Open();
            roomUpdateTime.Play();
            NetworkDiscovery.instance.StartDiscovery();
        }
        public void Close()
        {
            canvasGroup.Close();
            roomUpdateTime.Pause();
            NetworkDiscovery.instance.StopDiscovery();
            ClearInfos();
        }
        public void ClearInfos()
        {
            items = new List<RoomInfoItem>();
            while (itemContainer.childCount != 0)
            {
                DestroyImmediate(itemContainer.GetChild(0).gameObject);
            }
        }
        public void AddRoomInfo(ServerResponse response)
        {
            RoomInfoItem newItem = GameObject.Instantiate(infoItemPrefab, itemContainer);
            items.Add(newItem);

            RectTransform rectTransform = newItem.GetComponent<RectTransform>();
            float itemHeight = rectTransform.rect.height;
            itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, items.Count * itemHeight);

            rectTransform.anchoredPosition = new Vector2(0, (items.Count - 1) * itemHeight);

            newItem.Init(response, delegate { SelectItem(newItem); });
        }
        public void UpdateRoomInfo(ServerResponse response)
        {
            RoomInfoItem updateItem = items.Where(_ => _.serverId == response.serverId).FirstOrDefault();
            if(updateItem == null)
            {
                AddRoomInfo(response);
            }
            else
            {
                updateItem.Init(response);
            }
        }
        public void SelectItem(RoomInfoItem item)
        {
            if(selectItem != null)
            {
                selectItem.SwitchSelect(false);
            }
            selectItem = item;
            selectItem.SwitchSelect(true);
        }
        public void Confirm()
        {
            try
            {
                MainSceneUIManager.instance.propPanel.Wait("加入房间ing...");
                NetworkRoomManager.instance.StartClient(
                    new System.Uri(selectItem.uriText.text),
                    delegate
                    {
                        MainSceneUIManager.instance.propPanel.Close();
                        MainSceneUIManager.instance.searchPanel.Close();
                        MainSceneUIManager.instance.roomPanel.Open();
                    },
                    delegate
                    {
                        MainSceneUIManager.instance.roomPanel.Close();
                        MainSceneUIManager.instance.searchPanel.Open();
                    }
                );
            }
            catch(System.Exception e)
            {
                NetworkRoomManager.instance.StopClient();
                MainSceneUIManager.instance.propPanel.Log(e.Message);
                Debug.LogException(e);
            }
        }
        public void Return()
        {
            MainSceneUIManager.instance.searchPanel.Close();
            MainSceneUIManager.instance.lobbyPanel.Open();
        }
        private void UpdateAllRoomsTime()
        {
            items.ForEach(_ => _.RefreshTime());
        }
    }
}