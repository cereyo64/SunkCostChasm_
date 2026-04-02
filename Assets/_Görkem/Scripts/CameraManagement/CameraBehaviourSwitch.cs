using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CameraBehaviourSwitch : MonoBehaviour
{

    [SerializeField] private Collider2D collision2D;

    [SerializeField] private LayerMask interactedLayers;

    [SerializeField] private CameraManager.CameraBehaviour modifyBehaviour;

    public bool isOneTimed;

    public bool isFired;

    public bool isSwitcher;

    private void Start()
    {

        if(collision2D == null)
        {
            collision2D = GetComponent<Collider2D>();
        }

        collision2D.includeLayers = interactedLayers;
        collision2D.isTrigger = true;
    }

    public void ModifyCameraBehaviour()
    {
        switch (modifyBehaviour)
        {
           case CameraManager.CameraBehaviour.RoomCamera:

                CameraEvents.SwitchToCurrentRoomCamera();

           break;

           case CameraManager.CameraBehaviour.PlayerFollowCamera:

                CameraEvents.SwitchToPlayerFollowCamera();

           break;


        }



        //CameraEvents.
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOneTimed &&  isFired != false )
        {
            ToggleTrigger(false);
            isFired = true;

            TriggerActivation();
        }

        if (isSwitcher)
        {
            SwitchCameraBehaviourModifier();
        }

        
    }

    private void SwitchCameraBehaviourModifier()
    {
       if(modifyBehaviour  == CameraManager.CameraBehaviour.RoomCamera)
        {
            modifyBehaviour = CameraManager.CameraBehaviour.PlayerFollowCamera;
        }
        else if (modifyBehaviour == CameraManager.CameraBehaviour.PlayerFollowCamera)
        {
            modifyBehaviour = CameraManager.CameraBehaviour.RoomCamera;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
    public void TriggerActivation()
    {
        Debug.Log("Yehooo!!");
        ModifyCameraBehaviour();
    }
    public void ToggleTrigger(bool toggle)
    {
        collision2D.enabled = toggle;
    }

    public void ResetTrigger()
    {
        isFired = false;
    }
}
