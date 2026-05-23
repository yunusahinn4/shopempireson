using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 0.02f;

    public float minX = -10f;
    public float maxX = 10f;

    public float minY = -2f;
    public float maxY = 5f;

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;

            Vector3 newPosition = Camera.main.transform.position;

            newPosition -= new Vector3(
                delta.x * dragSpeed,
                delta.y * dragSpeed,
                0
            );

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            Camera.main.transform.position = newPosition;

            lastMousePosition = Input.mousePosition;
        }
    }
}