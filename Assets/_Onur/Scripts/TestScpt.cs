using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{


    InputAction Move_action;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Move_action = InputSystem.actions.FindAction("Move");


    }

    // Update is called once per frame
    void Update()
    {
       
       if(Move_action.WasReleasedThisFrame())
       { 
             Debug.Log("aaa");
        }
}
}
