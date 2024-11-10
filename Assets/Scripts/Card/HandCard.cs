using UnityEngine;

namespace Card
{
    public class HandCard : MonoBehaviour
    {
        private void Awake()
        {
            DestroyImmediate(gameObject.GetComponent<HandCardDebuger>());
            while(transform.childCount> 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}