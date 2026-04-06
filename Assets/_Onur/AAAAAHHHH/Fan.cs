using UnityEngine;

public class Fan : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(
            30 * Time.deltaTime,  // X
            50 * Time.deltaTime,  // Y
            20 * Time.deltaTime   // Z
        );
    }
}
