using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public static Vector2 worldPosition = new Vector2();
    Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        worldPosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
    }
}
