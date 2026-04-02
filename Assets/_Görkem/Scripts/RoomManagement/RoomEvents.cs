using System;
using UnityEngine;

public class RoomEvents
{
    public static event EventHandler<OnSwitchedRoomEventArgs> OnSwitchToNewRoom;
    public static void SwitchToNewRoom(string SwitchedRoomName)
    {
        OnSwitchToNewRoom?.Invoke(null, new OnSwitchedRoomEventArgs {switchedRoomName = SwitchedRoomName});
    }

    public class OnSwitchedRoomEventArgs : EventArgs
    {
        public string switchedRoomName;

        public RoomBase switchedRoom;
    }
}
