using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 10f; // 回転速度

    private Vector2 lastMousePosition;
    private Vector2 lastTouchPosition;

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

        // タッチ操作（1本指対応）
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 currentTouchPosition = touch.position;
                Vector2 delta = currentTouchPosition - lastTouchPosition;

                float yRotation = delta.x * rotationSpeed * Time.deltaTime;

                // 横回転のみを許可（Y軸周りの回転）
                transform.Rotate(0, yRotation, 0, Space.World);

                lastTouchPosition = currentTouchPosition;
            }
        }
    }
}
