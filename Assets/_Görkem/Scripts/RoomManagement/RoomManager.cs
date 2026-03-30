using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
public class RoomManager : MonoBehaviour
{
    public RoomBase currentRoom;

    public string currentRoomName;

    public Dictionary<string, RoomBase> roomMap = new Dictionary<string, RoomBase>();

    public float elapsedTime;

    public CinemachineCamera roomCamera;

    public void OnEnable()
    {
        RoomEvents.OnSwitchToNewRoom += RoomEvents_OnSwitchToNewRoom;
    }

    public void OnDisable()
    {
        RoomEvents.OnSwitchToNewRoom -= RoomEvents_OnSwitchToNewRoom;
    }

    public void Start() 
    {

        roomMap.Clear();
        currentRoom = null;

        RoomBase[] rooms = GameObject.FindObjectsByType<RoomBase>();

        foreach(RoomBase room in rooms)
        {
            roomMap.Add(room.roomName, room);
            if(room.gameObject.tag == "StartingRoom")
            {
                currentRoom = room;
                room.gameObject.SetActive(true);
                continue;
            }

            room.gameObject.SetActive(false);
        }

        

    }
    public void SwitchToNewRoom(string newRoom)
    {
        if (roomMap.ContainsKey(currentRoom.roomName) && currentRoom.roomName != newRoom)
        {
            currentRoom.gameObject.SetActive(false);

            currentRoom = roomMap[newRoom];

            currentRoom.gameObject.SetActive(true);

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            //Teleport player to spawn point.
            playerObj.transform.position = currentRoom.playerSpawnPoint.position;


            switch (currentRoom.transitionSettings.baseCameraBehaviour)
            {
                case CameraManager.CameraBehaviour.PlayerFollowCamera:

                    CameraEvents.SwitchToPlayerFollowCamera();

                break;

                case CameraManager.CameraBehaviour.RoomCamera:

                    CameraEvents.SwitchToRoomCamera(currentRoom);

                break;

            }

        }
    }
    private void RoomEvents_OnSwitchToNewRoom(object sender, string newRoomName)
    {
        if (newRoomName == null) return;

        SwitchToNewRoom(newRoomName);

    }
    public void ReturnToBase()
    {

    }

}
