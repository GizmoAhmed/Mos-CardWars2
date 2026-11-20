using Mirror;
using Mirror.Discovery;
using UnityEngine;

namespace GameManagement
{
    public class LanDiscovery : MonoBehaviour
    {
        [SerializeField] private NetworkDiscovery discovery;

        private bool _joining;

        private void Awake()
        {
            discovery.OnServerFound.AddListener(OnServerFound);
        }

        // host: call after StartHost()
        public void Advertise() => discovery.AdvertiseServer();

        // joiner: "Find Game" button
        public void Search()
        {
            _joining = false;
            discovery.StartDiscovery();
        }

        // call when the game starts or the lobby closes
        public void Stop() => discovery.StopDiscovery();

        private void OnServerFound(ServerResponse response)
        {
            if (_joining) return;     // ignore later replies
            _joining = true;

            discovery.StopDiscovery();
            NetworkManager.singleton.StartClient(response.uri);
        }
    }
}