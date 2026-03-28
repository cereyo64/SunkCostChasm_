using UnityEngine;

public class LayerExtruder : MonoBehaviour
{

    public Camera cam;

    public GameObject TilemapToDuplicate;

    public int layeramount = 1;


    [SerializeField, Range(0,1)]
     float layerspacing = 1;
    [SerializeField, Range(0, 10)]
    int CamRotation = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        cam.transform.Rotate(new Vector3(CamRotation, 0, 0));


        GameObject currentlayer = TilemapToDuplicate;

        for (int i = 0; i < layeramount; i++)
        {
            Vector3 locationtoinstantiate = currentlayer.transform.position + new Vector3(0 , 0 , layerspacing);

            GameObject instantiated = Instantiate(TilemapToDuplicate , locationtoinstantiate , Quaternion.identity , TilemapToDuplicate.transform);

            currentlayer = instantiated;

        }
        }

        // Update is called once per frame
     
    }
