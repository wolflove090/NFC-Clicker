using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 10f; // 回転速度

    private Vector2 lastMousePosition;
    private Vector2 lastTouchDelta;

    void Update()
    {
        // マウス操作
        if (Input.GetMouseButton(1)) // 右クリックが押されている場合
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
                transform.Rotate(Vector3.up, scrollDelta * rotationSpeed, Space.World);
            }

            if (Input.GetMouseButtonDown(1))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector2 currentMousePosition = Input.mousePosition;
                Vector2 delta = currentMousePosition - lastMousePosition;

                float yRotation = delta.x * rotationSpeed * Time.deltaTime;

                // 横回転のみを許可（Y軸周りの回転）
                transform.Rotate(0, yRotation, 0, Space.World);

                lastMousePosition = currentMousePosition;
            }
        }

        // タッチ操作
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                Vector2 currentTouchDelta = touch1.position - touch2.position;
                if (lastTouchDelta != Vector2.zero)
                {
                    Vector2 delta = currentTouchDelta - lastTouchDelta;

                    float yRotation = delta.x * rotationSpeed * Time.deltaTime;

                    // 横回転のみを許可（Y軸周りの回転）
                    transform.Rotate(0, yRotation, 0, Space.World);
                }

                lastTouchDelta = currentTouchDelta;
            }
            else if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
            {
                lastTouchDelta = Vector2.zero;
            }
        }
    }
}
