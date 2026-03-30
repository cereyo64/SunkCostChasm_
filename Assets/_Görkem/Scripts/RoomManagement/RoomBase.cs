using System;
using UnityEngine;

public class RoomBase : MonoBehaviour
{
    public string roomName;

    public Transform playerSpawnPoint;

    public CameraTransitionSettings transitionSettings;
   
    [Serializable]
    public struct CameraTransitionSettings
    {
       public Vector3 roomCameraOffset;

       public float transitionTime;

       
    }
    public void OnDrawGizmosSelected()
    {

       // Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.orange;
        Gizmos.DrawWireCube(transform.TransformPoint(transitionSettings.roomCameraOffset),Vector3.one);


    }



}
