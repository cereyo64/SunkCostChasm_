using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public RoomBase currentRoom;

    public string currentRoomName;

    public Dictionary<string, RoomBase> roomMap = new Dictionary<string, RoomBase>();

    public float elapsedTime;

    public Camera roomCamera;

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

            playerObj.transform.position = currentRoom.playerSpawnPoint.position;

            StartCoroutine(MoveCameraInTime(currentRoom));
            // Camera.main.transform.position = currentRoom.transform.TransformPoint(currentRoom.roomCameraOffset);
        }
    }

    public IEnumerator MoveCameraInTime(RoomBase roomBase)
    {
        Vector3 A = roomCamera.transform.position;
        Vector3 B = roomBase.transform.TransformPoint(roomBase.transitionSettings.roomCameraOffset);

        float transitionTime = roomBase.transitionSettings.transitionTime;

        if (transitionTime <= 0f)
        {
            roomCamera.transform.position = B;
            yield break;
        }

        elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / transitionTime);

            roomCamera.transform.position = Vector3.Lerp(A, B, progress);

            yield return null;
        }

        //roomCamera.transform.position = B;
       // elapsedTime = 0f;
        Debug.Log("Reached the camera point for this room !");
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
