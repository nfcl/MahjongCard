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

            newGroup.Init(kind, cards, fromPeople);
        }

        public void JiaGang(RealCard jiaGang)
        {
            groups.First(_ => _.kind == MingPaiKind.Peng && CardKind.LogicEqualityComparer.Equals(_.cards.First().faceKind, jiaGang.faceKind)).JiaGang(jiaGang);
        }
    }
}