using UnityEngine;


public class TextureBaker : MonoBehaviour
{
    public RenderTexture textureDostum;



    private void Start()
    {
        BakeTexture();
    }
    public void BakeTexture()
    {
        Graphics.Blit(null,textureDostum);
    }




}
