using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SetResolution : MonoBehaviour
{
    Dropdown dropdown;
    Resolution[] resolutions;
    List<string> options;

    public Toggle fullScreen_toggle;

    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        resolutions = Screen.resolutions;
        options = new List<string>();

        foreach (Resolution r in resolutions)
        {
            string s = $" {r.width} x {r.height} @ {r.refreshRate} ";
            options.Add(s);
        }

        dropdown.AddOptions(options);
    }


    public void OnValueChange(int index)
    {
        var res = resolutions[index];
        Screen.SetResolution(res.width, res.height, fullScreen_toggle.isOn);
    }


    public void OnValueChange_Fullscreen(bool val)
    {
        var res = Screen.currentResolution;
        Screen.SetResolution(res.width, res.height, val);
    }
}
