using System;
using UnityEngine;

public class CameraEvents 
{

    public static event EventHandler<RoomBase> OnSwitchToRoomCamera;

    public static event EventHandler OnSwitchToPlayerFollowCamera;

    public static event EventHandler<bool> OnFollowCameraToggle;

    public static void SwitchToRoomCamera(RoomBase currentRoom)
    {
        OnSwitchToRoomCamera?.Invoke(null , currentRoom );
    }                                            

    public static void SwitchToPlayerFollowCamera()
    {
        OnSwitchToPlayerFollowCamera?.Invoke(null ,EventArgs.Empty );
    }

    public static void ToggleFollowPlayer(bool toggle)
    {
        OnFollowCameraToggle?.Invoke(null,toggle);
    }


}
