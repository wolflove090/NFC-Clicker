using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // アタッチするGameObject
    public float fallSpeed = 5f;    // 落下速度
    public LayerMask groundLayer;   // 地面のレイヤーマスク
    float fixedYPosition = 10f; // 固定するY座標
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

        this._counter = initCount;
        this._isInit = true;
    }

    void Update()
    {
        if (this._isInit == false)
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

        // y座標を固定
        spawnPosition.y = fixedYPosition;

        // オブジェクトを生成
        float randomYRotation = Random.Range(0f, 360f);
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.Euler(0, randomYRotation, 0));
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
        float randomDepth = Random.Range(minDepth, maxDepth);
        spawnPosition = ray.GetPoint(randomDepth);

        // y座標を固定
        spawnPosition.y = fixedYPosition;

        // オブジェクトを生成
        float randomYRotation = Random.Range(0f, 360f);
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.Euler(0, randomYRotation, 0));
        // 落下処理を開始するスクリプトをアタッチ
        spawnedObject.AddComponent<ObjectFall>().Initialize(fallSpeed, groundLayer);

        this._counter++;
        this._onSpawned?.Invoke(this._counter);

        SoundManager.PlaySound("Sound/buun");
    }
}
