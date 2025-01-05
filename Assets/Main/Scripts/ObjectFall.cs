using UnityEngine;

public class ObjectFall : MonoBehaviour
{
    private float fallSpeed;       // 落下速度
    private LayerMask groundLayer; // 地面のレイヤーマスク
    private bool isFalling = true; // 落下中かどうか
    private bool isMoving = false; // 移動中かどうか

    public float moveSpeed = 0.2f;     // 移動速度
    public float moveFrequency = 3f;  // 動く方向の変更頻度
    public float startMoveDelay = 1f; // 地面到達後に動き出すまでの遅延時間
    public float rotationSpeed = 10f; // 回転速度 (度/秒)

    private float nextMoveTime;       // 次に向きを変更する時間
    private float moveStartTime;      // 移動を開始する時間
    private Quaternion targetRotation; // 次の目標回転

    // 初期化メソッド
    public void Initialize(float speed, LayerMask layer)
    {
        fallSpeed = speed;
        groundLayer = layer;
    }

    void Update()
    {
        if (isFalling)
        {
            HandleFalling();
        }
        else if (isMoving)
        {
            HandleMoving();
        }
        else if (Time.time >= moveStartTime)
        {
            // 遅延後に移動を開始
            isMoving = true;
            nextMoveTime = Time.time + moveFrequency;
            targetRotation = transform.rotation; // 現在の回転を初期化
        }
    }

    // 落下処理
    private void HandleFalling()
    {
        // 下方向に移動
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 地面に到達したかを確認
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f, groundLayer))
        {
            // 落下を停止し、移動開始時間を設定
            isFalling = false;
            moveStartTime = Time.time + startMoveDelay;
            SoundManager.PlaySound("Sound/puyon");
        }
    }

    // 前方に進む処理
    private void HandleMoving()
    {
        // 前方に移動
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // 現在の回転を目標回転に向けて滑らかに補間
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 一定時間ごとに向きを変更
        if (Time.time >= nextMoveTime)
        {
            nextMoveTime = Time.time + moveFrequency;
            ChooseNewDirection();
        }
    }

    // 新しい移動方向を設定
    private void ChooseNewDirection()
    {
        // ランダムな方向に回転
        float randomAngle = Random.Range(0f, 360f);
        targetRotation = Quaternion.Euler(0f, randomAngle, 0f);
    }
}
