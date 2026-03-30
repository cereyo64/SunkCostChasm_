using System;
using UnityEngine;

public class RoomEvents
{

    public static event EventHandler<string> OnSwitchToNewRoom;

    public static void SwitchToNewRoom(string roomName)
    {
        OnSwitchToNewRoom?.Invoke(null,roomName);
    }




}
