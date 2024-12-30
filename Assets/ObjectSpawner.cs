using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // アタッチするGameObject
    public LayerMask groundLayer;   // 地面のレイヤーマスク
    public float fallSpeed = 5f;    // 落下速度

    void Update()
    {
        // 画面クリックの検出
        if (Input.GetMouseButtonDown(0))
        {
            // カメラからタッチ座標を取得
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ヒットしなかった場合、奥行きをランダムに設定して生成
            const float minDepth = 5f;
            const float maxDepth = 20f;
            float randomDepth = Random.Range(minDepth, maxDepth);
            Vector3 spawnPosition = ray.GetPoint(randomDepth);

            // オブジェクトを生成
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            // 落下処理を開始するスクリプトをアタッチ
            spawnedObject.AddComponent<ObjectFall>().Initialize(fallSpeed, groundLayer);
        }
    }
}

public class ObjectFall : MonoBehaviour
{
    private float fallSpeed;    // 落下速度
    private LayerMask groundLayer; // 地面のレイヤーマスク
    private bool isFalling = true; // 落下中かどうか

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
            // 下方向に移動
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            // 地面に到達したかを確認
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f, groundLayer))
            {
                // 落下を停止
                isFalling = false;
                Debug.Log("Object landed on the ground!");
            }
        }
    }
}
