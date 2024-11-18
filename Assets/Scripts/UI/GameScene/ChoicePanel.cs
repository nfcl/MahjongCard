using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static MainSceneUI.PropPanel;

namespace GameSceneUI
{
    public class ChoicePanel : MonoBehaviour
    {
        [SerializeField]
        private ChoiceSpriteLoad[] choiceSpriteLoads;

        public CanvasGroup canvasGroup;

        public static int perRowItemNum = 3;
        public static Vector2 itemDistance = new Vector2(-250, 120);

        public ChoiceItem itemPrefab;
        public List<ChoiceItem> items;

        private void Awake()
        {
            Clear();
        }

        public void Open()
        {
            canvasGroup.Open();
        }
        public void Close()
        {
            canvasGroup.Close();
        }
        public ChoiceItem GenerateItem(ChoiceKind kind)
        {
            ChoiceItem newItem = GameObject.Instantiate(itemPrefab, this.transform);
            newItem.icon.sprite = choiceSpriteLoads.Where(_ => _.kind == kind).First().icon;
            newItem.icon.SetNativeSize();
            return newItem;
        }
        public void Clear()
        {
            items = new List<ChoiceItem>();
            while(this.transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
        public void AddChoice(ChoiceKind kind, D_Void_Void callback, int index)
        {
            ChoiceItem item = GenerateItem(kind);
            int col = index % perRowItemNum;
            int row = index / perRowItemNum;
            item.transform.localPosition = new Vector2(col * itemDistance.x, row * itemDistance.y);
            item.button.onClick.AddListener(() => callback?.Invoke());
            items.Add(item);
        }
        public void Init((ChoiceKind kind, D_Void_Void callback)[] data)
        {
            Clear();
            AddChoice(ChoiceKind.Skip, Close, 0);
            data.Foreach((_, index) => AddChoice(_.kind, _.callback, index + 1));
        }

        public enum ChoiceKind
        {
            LiZhi,
            Peng,
            Gang,
            Chi,
            He, 
            ZiMo,
            Skip,
            JiuZhongJiuPai,
            None
        }
        [Serializable]
        private class ChoiceSpriteLoad
        {
            public ChoiceKind kind;
            public Sprite icon;
        }

//#if UNITY_EDITOR

//        private void OnValidate()
//        {
//            if (choiceSpriteLoads == null || choiceSpriteLoads.Length != (int)ChoiceKind.None + 1)
//            {
//                ChoiceSpriteLoad[] temp = new ChoiceSpriteLoad[(int)ChoiceKind.None + 1];
//                choiceSpriteLoads.CopyTo(temp, 0);
//                for (int i = 0; i < temp.Length; i++)
//                {
//                    if (choiceSpriteLoads[i] == null)
//                    {
//                        choiceSpriteLoads[i] = new ChoiceSpriteLoad
//                        {
//                            kind = (ChoiceKind)i
//                        };
//                    }
//                    else
//                    {
//                        choiceSpriteLoads[i].kind = (ChoiceKind)i;
//                    }
//                }
//            }
//        }

//#endif
    }
}