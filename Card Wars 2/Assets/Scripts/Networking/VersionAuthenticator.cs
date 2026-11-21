using System.Collections;
using Mirror;
using Networking;
using UnityEngine;

namespace GameManagement
{
    public struct VersionRequestMessage : NetworkMessage
    {
        public string version;
    }

    public struct VersionResponseMessage : NetworkMessage
    {
        public bool accepted;
        public string hostVersion;
    }

    public class VersionAuthenticator : NetworkAuthenticator
    {
        /// <summary>Set on a rejected joiner, so the menu can explain why they were kicked.</summary>
        public static string LastRejectReason;

        // ---------- Server ----------

        public override void OnStartServer()
            => NetworkServer.RegisterHandler<VersionRequestMessage>(OnVersionRequest, false);

        public override void OnStopServer()
            => NetworkServer.UnregisterHandler<VersionRequestMessage>();

        private void OnVersionRequest(NetworkConnectionToClient conn, VersionRequestMessage msg)
        {
            bool match = msg.version == Application.version;

            conn.Send(new VersionResponseMessage
            {
                accepted = match,
                hostVersion = Application.version
            });

            if (match)
            {
                ServerAccept(conn);
            }
            else
            {
                DisconnectNotice.Set($"Rejected client: version {msg.version} != host {Application.version}");
                // Debug.LogWarning($"Rejected client: version {msg.version} != host {Application.version}");
                conn.isAuthenticated = false;
                StartCoroutine(DelayedDisconnect(conn, 1f));   // let the response arrive first
            }
        }

        private IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float wait)
        {
            yield return new WaitForSeconds(wait);
            conn.Disconnect();
        }

        // ---------- Client ----------

        public override void OnStartClient()
            => NetworkClient.RegisterHandler<VersionResponseMessage>(OnVersionResponse, false);

        public override void OnStopClient()
            => NetworkClient.UnregisterHandler<VersionResponseMessage>();

        public override void OnClientAuthenticate()
        {
            LastRejectReason = null;
            NetworkClient.Send(new VersionRequestMessage { version = Application.version });
        }

        private void OnVersionResponse(VersionResponseMessage msg)
        {
            if (msg.accepted)
            {
                ClientAccept();
            }
            else
            {
                LastRejectReason =
                    $"Version mismatch. You have v{Application.version}, host has v{msg.hostVersion}.";
                DisconnectNotice.Set(LastRejectReason);
                ClientReject();
            }
        }
    }
}