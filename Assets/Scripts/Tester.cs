using UnityEngine;

public class Tester : MonoBehaviour
{
    int avgFrameRate;

    public void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), avgFrameRate.ToString() + " FPS");
    }
}
