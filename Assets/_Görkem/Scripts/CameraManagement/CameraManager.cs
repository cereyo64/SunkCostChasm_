using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Dictionary<string, CinemachineCamera> cameraDictionary = new Dictionary<string, CinemachineCamera>();


    public void RegisterCinemachineCamera(string cameraName, CinemachineCamera camera)
    {

        if(cameraName == string.Empty || camera == null)
        {
            Debug.LogWarning("Tried to assign an empty camera name or a null cinemachine Camera reference");
            return;
        }

        if (cameraDictionary.ContainsKey(cameraName))
        {
            if(cameraDictionary[cameraName] != null)
            {
                cameraDictionary[cameraName] = camera;
                Debug.Log($"Replaced Definition of {cameraName} with {camera.name}");
            }
        }
        else
        {
            if (cameraDictionary[cameraName] != null)
            {
                cameraDictionary.Add(cameraName, camera);
            }
        }
    }
    public void UnRegisterCinemachineCamera(string cameraName)
    {

        if (cameraName == string.Empty )
        {
            Debug.LogWarning("Tried to unregister an empty camera name" );
            return;
        }

        if (cameraDictionary.ContainsKey(cameraName))
        {
            cameraDictionary.Remove(cameraName);
        }
        else
        {
            Debug.LogWarning("There is no camera registered with that name, make sure you register the camera before");
            return;
        }
       
    }
    public CinemachineCamera GetCinemachineCamera(string cameraName)
    {

       if(cameraDictionary.ContainsKey(cameraName))
       {
            return cameraDictionary[cameraName];
       }
       else
       {
            Debug.LogWarning($"Camera {cameraName} does not exist in list ");
            return null;
       }

    }



    /// <summary>
    /// Gets or sets the Cinemachine camera used to render the current room view.
    /// </summary>
    public CinemachineCamera roomCamera;

    /// <summary>
    /// Gets or sets the Cinemachine virtual camera used to follow the player character.
    /// </summary>
    /// <remarks>Assign this property to specify which Cinemachine camera instance should track the player's
    /// movement. This is typically used to provide a dynamic third-person or follow camera in gameplay.</remarks>
    public CinemachineCamera playerFollowCamera;

    /// <summary>
    /// Gets or sets the elapsed time in seconds since the start of the operation or event.
    /// </summary>
    [SerializeField] private float elapsedTime = 0f;

    public CameraBehaviour currentCameraBehaviour = CameraBehaviour.RoomCamera;
    public enum CameraBehaviour
    {
        RoomCamera,
        PlayerFollowCamera,
    }

    public void Start()
    {

        if ( roomCamera == null )
        {
            roomCamera = GameObject.FindGameObjectWithTag("RoomCam").GetComponent<CinemachineCamera>();

        }
        if ( playerFollowCamera == null )
        {
            playerFollowCamera = GameObject.FindGameObjectWithTag("PlayerFollowCam").GetComponent<CinemachineCamera>();

        }

    }
    public void OnEnable()
    {
        CameraEvents.OnSwitchToRoomCamera += CameraEvents_OnSwitchToRoomCamera;
        CameraEvents.OnSwitchToPlayerFollowCamera += CameraEvents_OnSwitchToPlayerFollowCamera;
    }

    public void OnDisable()
    {
        CameraEvents.OnSwitchToRoomCamera -= CameraEvents_OnSwitchToRoomCamera;
        CameraEvents.OnSwitchToPlayerFollowCamera -= CameraEvents_OnSwitchToPlayerFollowCamera;
    }





    private void CameraEvents_OnSwitchToPlayerFollowCamera(object sender, System.EventArgs e)
    {
        playerFollowCamera.Priority = 1;
        roomCamera.Priority = 0;

        currentCameraBehaviour = CameraBehaviour.PlayerFollowCamera;
    }
    private void CameraEvents_OnSwitchToRoomCamera(object sender, RoomBase roomBase)
    {
        if (roomBase == null)
        {
            Debug.LogWarning("The room reference sent into ther Camera Event is null ");
            return;
        }

        roomCamera.Priority = 1;
        playerFollowCamera.Priority = 0;

        currentCameraBehaviour = CameraBehaviour.RoomCamera;

        StartCoroutine(TransitionCameraPosInTime(roomBase));

    }

    private IEnumerator TransitionCameraPosInTime(RoomBase roomBase)
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
   
 
}
