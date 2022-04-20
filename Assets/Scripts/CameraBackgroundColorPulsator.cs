using DG.Tweening;
using UnityEngine;

public class CameraBackgroundColorPulsator : MonoBehaviour
{
    public float interval;
    public Color colorOne, colorTwo;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Pulsate()
    {
        if (cam.backgroundColor != colorOne) cam.DOColor(colorOne, interval).From(colorTwo).OnComplete(() => Pulsate());
        else if (cam.backgroundColor != colorTwo) cam.DOColor(colorTwo, interval).From(colorOne).OnComplete(() => Pulsate());
    }
}
