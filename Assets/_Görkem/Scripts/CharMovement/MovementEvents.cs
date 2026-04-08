using System;
using UnityEngine;

public class MovementEvents
{

    public static event EventHandler<Ladder> OnGrabLadder;
    public static void GrabLadder(Ladder ladder)
    {
        OnGrabLadder?.Invoke(null, ladder);
    }




}
