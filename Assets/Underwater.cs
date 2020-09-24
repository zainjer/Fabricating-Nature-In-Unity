using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;

public class Underwater : MonoBehaviour
{
    public UnityStandardAssets.Water.Water waterUp;
    public UnityStandardAssets.Water.Water waterDown;
    public Color fog_Color;

    public RainCameraController waterSlpashIN;
    public RainCameraController waterSlpashOut;  

    private float underwaterLevel;
    private bool defaultFog = true;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private Material defaultSkybox;
    private float defaultStartDistance;
    private UnityStandardAssets.ImageEffects.BlurOptimized blurOptimized;
    private bool firstTimeIN = true;
    private bool firstTimeOut = false;

    

    void Start()
    {
        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultSkybox = RenderSettings.skybox;
        defaultStartDistance = RenderSettings.fogStartDistance;

        Camera.main.backgroundColor = new Color(0, 0.4f, 0.7f, 1);

        blurOptimized = GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>();

        underwaterLevel = waterUp.transform.position.y;

    }

    void Update()
    {
        if (transform.position.y <= underwaterLevel)
        {
            UnderWaterEffect();
            if(firstTimeIN)
                LensWetIN();
        }
        else
        {
            RevertChanges();
            if(firstTimeOut)
                LensWetOUT();
        }
    }

    private void LensWetOUT()
    {
        waterUp.gameObject.SetActive(true);
        waterDown.gameObject.SetActive(false);

        waterSlpashIN.StopImmidiate();
        waterSlpashOut.Play();
        firstTimeIN = true;
        firstTimeOut = false;
    }

    private void LensWetIN()
    {
        waterDown.gameObject.SetActive(true);
        waterUp.gameObject.SetActive(false);
        waterSlpashOut.StopImmidiate();
        waterSlpashIN.Play();
        firstTimeIN = false;
        firstTimeOut = true;
    }

    private void RevertChanges()
    {
        RenderSettings.fog = defaultFog;
        RenderSettings.fogColor = defaultFogColor;
        RenderSettings.fogDensity = defaultFogDensity;
        RenderSettings.skybox = defaultSkybox;
        RenderSettings.fogStartDistance = defaultStartDistance;

        blurOptimized.enabled = false;
    }

    void UnderWaterEffect()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = fog_Color;
        RenderSettings.fogDensity = 0.025f;
        RenderSettings.fogStartDistance = 0.0f;

        blurOptimized.enabled = true;
    }

}