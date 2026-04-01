using UnityEngine;

public class CameraBehaviourSwitch : MonoBehaviour
{

    [SerializeField] private Collider2D collision;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private CameraManager.CameraBehaviour modifyBehaviour;

    public void ModifyCameraBehaviour()
    {
        switch (modifyBehaviour)
        {
           case CameraManager.CameraBehaviour.RoomCamera:

              // CameraEvents.SwitchToRoomCamera();

           break;

           case CameraManager.CameraBehaviour.PlayerFollowCamera:

                CameraEvents.SwitchToPlayerFollowCamera();

           break;


        }



        //CameraEvents.
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }


    
}
