namespace Network
{
    public class NetworkRoomManager : Mirror.NetworkRoomManager
    {
        public static NetworkRoomManager instance;

        private new void Awake()
        {
            base.Awake();
            if (instance != null)
            {
                DestroyImmediate(this.gameObject);
            }
            instance = this;
        }
    }
}