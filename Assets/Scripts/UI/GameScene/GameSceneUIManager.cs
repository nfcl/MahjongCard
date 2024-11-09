using UnityEngine;

namespace GameSceneUI
{
    public class GameSceneUIManager : MonoBehaviour
    {
        public static GameSceneUIManager instance;

        public EnterPanel enterPanel;

        private void Awake()
        {
            instance = this;
        }
    }
}