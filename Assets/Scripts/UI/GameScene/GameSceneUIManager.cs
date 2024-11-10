using Card;
using UnityEngine;

namespace GameSceneUI
{
    public class GameSceneUIManager : MonoBehaviour
    {
        public static GameSceneUIManager instance;

        public EnterPanel enterPanel;
        public GamePanel gamePanel;

        private void Awake()
        {
            instance = this;
        }
    }
}