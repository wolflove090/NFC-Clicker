using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // アタッチするGameObject
    public float fallSpeed = 5f;    // 落下速度
    public LayerMask groundLayer;   // 地面のレイヤーマスク
    float minDepth = 10f;     // 最小奥行き
    float maxDepth = 50f;    // 最大奥行き

    bool _isInit;

    int _counter;
    public int Counter => this._counter;

    System.Action<int> _onSpawned = null;

    public void Init(int initCount, System.Action<int> onSpawned)
    {
        this._onSpawned = onSpawned;

        // 起動時に指定された数のオブジェクトを事前生成
        for (int i = 0; i < initCount; i++)
        {
            SpawnRandomObject();
        }

        this._isInit = true;
    }

    void Update()
    {
        if(this._isInit == false)
            return;

        // 画面クリックの検出
        if (Input.GetMouseButtonDown(0))
        {
            SpawnObjectAtClick();
        }
    }

    // 指定されたランダムな位置にオブジェクトを生成
    private void SpawnRandomObject()
    {
        // カメラ前方のランダムな位置を計算
        float randomDepth = Random.Range(minDepth, maxDepth);
        Vector2 screenPosition = new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 spawnPosition = ray.GetPoint(randomDepth);

        // オブジェクトを生成
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        // 落下処理を開始するスクリプトをアタッチ
        spawnedObject.AddComponent<ObjectFall>().Initialize(fallSpeed, groundLayer);
    }

    // クリック位置にオブジェクトを生成
    private void SpawnObjectAtClick()
    {
        // カメラからタッチ座標を取得
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 spawnPosition;
        if (Physics.Raycast(ray, out hit))
        {
            // ヒットした位置にオブジェクトを生成
            spawnPosition = hit.point;
        }
        else
        {
            // ヒットしなかった場合、奥行きをランダムに設定して生成
            float randomDepth = Random.Range(minDepth, maxDepth);
            spawnPosition = ray.GetPoint(randomDepth);
        }

        // オブジェクトを生成
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        // 落下処理を開始するスクリプトをアタッチ
        spawnedObject.AddComponent<ObjectFall>().Initialize(fallSpeed, groundLayer);

        this._counter++;

        this._onSpawned?.Invoke(this._counter);
    }
}

public class ObjectFall : MonoBehaviour
{
    private float fallSpeed;       // 落下速度
    private LayerMask groundLayer; // 地面のレイヤーマスク
    private bool isFalling = true; // 落下中かどうか
    private bool isMoving = false; // 移動中かどうか

    public float moveSpeed = 0.2f;     // 移動速度（デフォルト: ゆっくり）
    public float moveRange = 1f;      // 移動範囲
    public float moveFrequency = 3f;  // 動く方向の変更頻度（デフォルト: ゆっくり）
    public float startMoveDelay = 1f; // 地面到達後に動き出すまでの遅延時間

    private Vector3 moveDirection;    // 移動方向
    private float nextMoveTime;       // 次に移動方向を変更する時間
    private float moveStartTime;      // 移動を開始する時間

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
            ChooseNewDirection();
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
            Debug.Log("Object landed. It will start moving after a delay.");
        }
    }

    // ゆっくり動く処理
    private void HandleMoving()
    {
        // 移動
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 一定時間ごとに移動方向を変更
        if (Time.time >= nextMoveTime)
        {
            nextMoveTime = Time.time + moveFrequency;
            ChooseNewDirection();
        }
    }

    // 新しい移動方向を選択
    private void ChooseNewDirection()
    {
        // ランダムな方向を設定
        float randomAngle = Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)).normalized * moveRange;
    }
}


