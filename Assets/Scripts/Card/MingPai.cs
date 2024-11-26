using Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Card
{
    public class MingPai : MonoBehaviour
    {
        public List<MingPaiGroup> groups;

        private void Awake()
        {
            groups = new List<MingPaiGroup>();
            while(transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        public void AddGroup(MingPaiKind kind, RealCard[] cards, int fromPeople = -1)
        {
            MingPaiGroup newGroup = GameObject.Instantiate(DesktopManager.instance.mingGroupPrefab, this.transform);

            float width = newGroup.Init(kind, cards, fromPeople);

            if (groups.Count > 0)
            {
                newGroup.transform.localPosition = new Vector3(groups.Last().transform.localPosition.x - width, 0, 0);
            }
            else
            {
                newGroup.transform.localPosition = new Vector3(-width, 0, 0);
            }

            groups.Add(newGroup);
        }

        public void JiaGang(RealCard jiaGang)
        {
            groups.First(_ => _.kind == MingPaiKind.Peng && CardKind.LogicEqualityComparer.Equals(_.cards.First().faceKind, jiaGang.faceKind)).JiaGang(jiaGang);
        }
    }
}