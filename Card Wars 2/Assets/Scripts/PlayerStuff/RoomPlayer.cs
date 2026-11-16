using Mirror;

namespace GameManagement
{
    /// <summary>
    /// Lobby avatar for one connection. Only exists in the Lobby scene.
    /// Tells the lobby UI to redraw whenever someone joins, leaves, or changes ready state.
    /// </summary>
    public class RoomPlayer : NetworkRoomPlayer
    {
        // These are Mirror's built-in virtual hooks, so no base-call gymnastics are needed
        public override void OnClientEnterRoom()  => LobbyStatusUI.Refresh();
        public override void OnClientExitRoom()   => LobbyStatusUI.Refresh(except: this);
        public override void IndexChanged(int oldIndex, int newIndex) => LobbyStatusUI.Refresh();
        public override void ReadyStateChanged(bool oldReady, bool newReady) => LobbyStatusUI.Refresh();
    }
}