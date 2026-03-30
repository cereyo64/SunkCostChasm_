using System;
using UnityEngine;

public class RoomEvents
{

    public static event EventHandler<string> OnSwitchToNewRoom;
    public static event EventHandler OnSwitchToPlayerFollowCam;
    public static event EventHandler OnSwitchToRoomCamPoint;
    public static void SwitchToNewRoom(string roomName)
    {
        OnSwitchToNewRoom?.Invoke(null,roomName);
    }


    public static void SwitchCameraToFollowPlayer()
    {
        OnSwitchToPlayerFollowCam?.Invoke(null, EventArgs.Empty);
    }

    public static void SwitchCameraToRoomPoint()
    {

        OnSwitchToRoomCamPoint?.Invoke(null, EventArgs.Empty);

    }


}
