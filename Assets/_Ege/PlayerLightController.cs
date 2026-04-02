using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerLightController : MonoBehaviour
{
    [Header("Light 2D Özellikleri")]
    public Light2D PlayerGlobalLight;
    public float LightIntensity;
    public int LightAngleMin, LightAngleMax;
    public Color LightColor;
    public float LightFalloffStrenght;
    public float LightRadius;

    [Header("LightVriable'larý")]
    public float StartedLightBatary;
    public float LightBatary;
    public float LightGoAndBackMinTime;
    public float LightGoAndBackMaxTime;
    public float LightDecraseRate;
    public bool SpendBatary = true;
    public float BateryDecraseSpeed;

    [Header("Light Movement")]
    public GameObject LightParent;
    public float LightRotationSpeed;
    public float floatAmplitudeX = 0.5f; // Ne kadar yukarý/aþaðý gidecek
    public float floatAmplitudeY = 0.5f;
    public float floatFrequencyX = 1f;    // Ne kadar sürede bir döngü tamamlanacak
    public float floatFrequencyY = 1f;
    private Vector3 startPos;

    bool LightHasGoes = true;
    float LightGoesTime;
    void Start()
    {
        PlayerGlobalLight.intensity = LightIntensity;
        PlayerGlobalLight.pointLightInnerAngle = LightAngleMin;
        PlayerGlobalLight.pointLightOuterAngle = LightAngleMax;
        PlayerGlobalLight.color = LightColor;
        PlayerGlobalLight.pointLightOuterRadius = LightRadius;
        PlayerGlobalLight.falloffIntensity = LightFalloffStrenght;

        LightBatary = StartedLightBatary;
    }


    void Update()
    {
        FloatingMovement();

        // 1. Önce pil ve parlaklýk hesaplamalarýný yap
        if (LightBatary > 0 && SpendBatary)
        {
            LightBatary -= Time.deltaTime * BateryDecraseSpeed;
            LightIntensity = LightBatary / StartedLightBatary;
            LightRadius = LightBatary / 8;
        }

        // 2. Titreme (Flicker) ve diðer durumlarý kontrol et
        LightGoAndReturn();

        // 3. EN SON: Tüm hesaplamalar bittikten sonra bileþene gönder
        LightComponentSetVariables();
    }



    public void LightGoAndReturn()
    {
        if (LightBatary <= 0)
        {
            DarkHandsAreComing();
        }
        else
        {
            if (LightHasGoes)
            {
                LightGoesTime = Random.Range(LightBatary / LightGoAndBackMinTime, LightBatary / LightGoAndBackMaxTime);
                LightHasGoes = false;
            }

            LightGoesTime -= Time.deltaTime * LightDecraseRate;

            if (LightGoesTime <= 0)
            {
                StartCoroutine(FlickerRoutine());
            }
        }
    }

    IEnumerator FlickerRoutine()
    {
        for (int i = 0; i < 2; i++)
        {
            float originalIntensity = LightIntensity;
            float targetIntensity = originalIntensity * 0.2f;
            float duration = Random.Range(0.1f, 0.3f); ; // Geçiþ süresi (saniye)
            float elapsed = 0f;

            // 1. Kararma Fazý
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration; // 0'dan 1'e yükselen oran
                LightIntensity = Mathf.Lerp(originalIntensity, targetIntensity, t);
                LightComponentSetVariables(); // Iþýðý güncelle
                yield return null; // Bir sonraki kareyi bekle
            }

            elapsed = 0f;

            // 2. Aydýnlanma Fazý
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                LightIntensity = Mathf.Lerp(targetIntensity, originalIntensity, t);
                LightComponentSetVariables(); // Iþýðý güncelle
                yield return null;
            }
        }

        LightHasGoes = true;
    }

    public void FloatingMovement()
    {
        startPos = transform.position;
        // Sinüs dalgasý kullanarak Y ekseninde deðiþim hesapla
        float newY = Mathf.Sin(Time.time * (Mathf.PI * 2 / floatFrequencyY)) * floatAmplitudeY;
        float newX = Mathf.Sin(Time.time * (Mathf.PI * 2 / floatFrequencyX)) * floatAmplitudeX;
        LightParent.transform.position = startPos + new Vector3(newX, newY, 0);
    }

    public void LightComponentSetVariables()
    {
        PlayerGlobalLight.intensity = LightIntensity;
        PlayerGlobalLight.pointLightInnerAngle = LightAngleMin;
        PlayerGlobalLight.pointLightOuterAngle = LightAngleMax;
        PlayerGlobalLight.color = LightColor;
        PlayerGlobalLight.pointLightOuterRadius = LightRadius;
        PlayerGlobalLight.falloffIntensity = LightFalloffStrenght;
    }
    public void DarkHandsAreComing()
    {
        print("Iþýlk bitti, ayva yenildi");
        GetComponentInParent<IHaveHealth>().TakeDamage(999);
    }
}
