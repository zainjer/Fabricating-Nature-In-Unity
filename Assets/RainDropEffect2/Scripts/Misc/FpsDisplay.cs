using UnityEngine;
using System.Collections;

public class FpsDisplay : MonoBehaviour
{
    float interval = 0.2f;
    float startTime = 0f;
    float dt = 0f;
    int flameCnt = 0;
    int fps = 0;

    TMPro.TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    void LateUpdate()
    {
        dt = Time.time - startTime;
        flameCnt += 1;
        if (dt >= interval)
        {
            fps = (int)(flameCnt / dt);
            flameCnt = 0;
            startTime = Time.time;
        }

        text.text = fps.ToString();
    }

    
}
