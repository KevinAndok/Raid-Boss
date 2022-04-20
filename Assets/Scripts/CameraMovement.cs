using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Range(1,10)] public int cameraSpeedDivider = 10;

    public Vector2 baseScreenScale = new Vector2(1920, 1080);

    float cameraSpeed = 1;
    float minCamSize = 1, maxCamSize = 20;

    Camera cam;
    Vector2 mousePosDelta;
    Vector2 cameraMoveThreshold;

    private void Awake()
    {
        cam = Camera.main;
        cameraMoveThreshold = new Vector2(Screen.width / 30, Screen.height / 30);
    }

    private void Update()
    {
        float moveX = 0, moveY = 0;
        cameraSpeed = (maxCamSize / cam.orthographicSize) * cameraSpeedDivider * 10;

        if (Input.GetMouseButton(2))
        {
            moveX = (mousePosDelta.x - Input.mousePosition.x) / cameraSpeed * (baseScreenScale.x / Screen.width);
            moveY = (mousePosDelta.y - Input.mousePosition.y) / cameraSpeed * (baseScreenScale.y / Screen.height);
        }

        mousePosDelta = Input.mousePosition;

        if (mousePosDelta.x < cameraMoveThreshold.x) moveX -= cameraSpeed * Time.deltaTime * (cam.orthographicSize / maxCamSize);
        else if (mousePosDelta.x > Screen.width - cameraMoveThreshold.x) moveX += cameraSpeed * Time.deltaTime * (cam.orthographicSize / maxCamSize);

        if (mousePosDelta.y < cameraMoveThreshold.y) moveY -= cameraSpeed * Time.deltaTime * (cam.orthographicSize / maxCamSize);
        else if (mousePosDelta.y > Screen.height - cameraMoveThreshold.y) moveY += cameraSpeed * Time.deltaTime * (cam.orthographicSize / maxCamSize);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x + moveX, -32, 32),
            Mathf.Clamp(transform.position.y + moveY, -20, 20),
            transform.position.z);

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y, minCamSize, maxCamSize);
    }
}
