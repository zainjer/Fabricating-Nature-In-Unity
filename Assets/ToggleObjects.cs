using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjects : MonoBehaviour
{

    public List<GameObject> List1;
    public List<GameObject> List2;
    public List<GameObject> List3;
    public List<GameObject> List4;
    public List<GameObject> List5;
    public List<GameObject> List6;
    public List<GameObject> List7;
    public List<GameObject> List8;
    public List<GameObject> List9;
    public List<GameObject> List0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach(var g in List1)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (var g in List2)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (var g in List3)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (var g in List4)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            foreach (var g in List5)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            foreach (var g in List6)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            foreach (var g in List7)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            foreach (var g in List8)
                g.SetActive(!g.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            foreach (var g in List9)
                g.SetActive(!g.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            foreach (var g in List0)
                g.SetActive(!g.activeSelf);
        }
       
    }
}
