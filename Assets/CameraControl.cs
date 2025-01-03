using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 5f; // 回転速度（調整済み）

    private Vector2 lastMousePosition;
    private Vector2 lastTouchPosition;
    private bool isHorizontalDrag = true; // ドラッグ方向を判定

    void Update()
    {
        // マウス操作
        if (Input.GetMouseButton(1)) // 右クリックが押されている場合
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector2 currentMousePosition = Input.mousePosition;
                Vector2 delta = currentMousePosition - lastMousePosition;

                // ドラッグ方向の判定
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    isHorizontalDrag = true;
                }
                else
                {
                    isHorizontalDrag = false;
                }

                if (isHorizontalDrag)
                {
                    float yRotation = delta.x * rotationSpeed * Time.deltaTime;
                    transform.Rotate(0, yRotation, 0, Space.World);
                }
                else
                {
                    float xRotation = -delta.y * rotationSpeed * Time.deltaTime;
                    transform.Rotate(xRotation, 0, 0, Space.Self);
                }

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
                isHorizontalDrag = true; // 初期化
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 currentTouchPosition = touch.position;
                Vector2 delta = currentTouchPosition - lastTouchPosition;

                // ドラッグ方向の判定
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    isHorizontalDrag = true;
                }
                else
                {
                    isHorizontalDrag = false;
                }

                if (isHorizontalDrag)
                {
                    float yRotation = delta.x * rotationSpeed * Time.deltaTime;
                    transform.Rotate(0, yRotation, 0, Space.World);
                }
                else
                {
                    float xRotation = -delta.y * rotationSpeed * Time.deltaTime;
                    transform.Rotate(xRotation, 0, 0, Space.Self);
                }

                lastTouchPosition = currentTouchPosition;
            }
        }
    }
}
