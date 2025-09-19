using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    [SerializeField] private bool devMode;  //Dev mode
    LivingEntity playerEntity;
    Transform playerT;

    public Wave[] waves;
    public Enemy enemy;             //적 프리팹

    Wave currentWave;                                        //현재 웨이브 데이터
    [HideInInspector] public int currentWaveNumber;          //현재 웨이브 번호

    int enemiesRemainingToSpawn;    //남아있는 스폰해야할 적
    [HideInInspector] public int enemiesRemaningAlive;       //살아 있는 적의 수
    float nextSpawnTime;

    MapGenerator map;               //맵 제너레이터 참조

    [Header("Random Enemy Settings")]
    [HideInInspector] public bool useRandomEnemyStats = false;
    public RandomEnemySettings randomEnemySettings;

    float timeBetweenCampingChecks = 2;  //얼마나 자주 존버를 체크할 것인가.
    float campThresholdDistance = 1.5f;  //존버로 간주되지 않으려면 이동해야 할 최소 거리
    float nextCampCheckTime;             //다음 존버를 체크하는 시간.
    Vector3 campPositionOld;             //마지막 존버 체크 시 플레이어 위치
    bool isCamping;                      //존버 여부     true: 존버중

    bool isDisabled;                     //플레이어가 죽었을 때 플레이어 관련 기능들을 비활성활 시켜줌.

    public event System.Action<int> OnNewWave;  //새로운 웨이브 시작 시 호출되는 이벤트

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();

        // 맵 제너레이터가 랜덤 모드일 때 적 스탯도 랜덤으로 설정
        if (map != null && map.useRandomMaps)
        {
            useRandomEnemyStats = true;
        }

        NextWave(); // 첫 웨이브 시작
    }

    private void Update()
    {
        if (!isDisabled) //플레이어가 살아 있을 때
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;   //다음 캠핑 체크 시간

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);  // 플레이어가 이동했는지 확인
                campPositionOld = playerT.position; // 현재 위치를 다음 체크 시의 위치로 저장
            }

            // 적 스폰 처리
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;  // 스폰 예정인 적 감소
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;  // 다음 스폰 시간 설정

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StopCoroutine("SpawnEnemy");    //적 스폰 중지
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())  //모든 적 제거
                    Destroy(enemy.gameObject);

                NextWave(); //다음 웨이브로 이동
            }
        }
    }

    /// <summary>
    /// 적 스폰
    /// </summary>
    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;       //스폰 예고 시간
        float tileFlashSpeed = 4;   //타일이 깜빡거리는 속도

        // 랜덤한 열린 타일을 선택
        Transform spawnTile = map.GetRandomOpenTile();

        if (isCamping)  //플레이어가 캠핑 상태라면 플레이어 위치에 스폰
            spawnTile = map.GetTileFromPosition(playerT.position);

        Material tileMat = spawnTile.GetComponent<Renderer>().material;

        Color initialColor = Color.white;   //기본 색
        Color flashColor = Color.red;       //타일이 변할 때의 색
        float spawnTimer = 0;

        // 스폰 예고 및 타일 깜빡임
        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));   //적이 스폰되는 위치의 타일을 빨갛게 깜빡거려서 적이 스폰될 위치를 알려줌.

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        // 적 생성
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;// 적 생성
        spawnedEnemy.OnDeath += OnEnemyDeath;   // 적이 죽었을 때 호출될 메서드 등록
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor); // 적 속성 설정
    }

    /// <summary>
    /// 플레이어가 죽었을 때
    /// </summary>
    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    /// <summary>
    /// 적이 죽었을 때
    /// </summary>
    void OnEnemyDeath()
    {
        enemiesRemaningAlive--; // 살아있는 적 수 감소

        if (enemiesRemaningAlive == 0)  // 모든 적이 죽었다면 다음 웨이브로 이동
            NextWave();
    }

    /// <summary>
    /// 새로운 웨이브가 시작 시 플레이어의 위치 초기화
    /// </summary>
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2.5f;
    }

    /// <summary>
    /// 무한 웨이브(난이도 조정됨)
    /// </summary>
    void InfiniteWave()
    {
        timeBetweenCampingChecks = 1f;  //얼마나 자주 존버를 체크할 것인가.  (기존 값: 2f)
        campThresholdDistance = 2f;   //존버로 간주되지 않으려면 이동해야 할 최소 거리  (기존 값: 1.5f)
    }

    /// <summary>
    /// 다음 웨이브로 이동하는 메서드
    /// </summary>
    void NextWave()
    {
        if (currentWaveNumber > 0)  //웨이브 클리어했다면 웨이브 완료 사운드 재생
            AudioManager.instance.PlaySound2D("Level Complete");

        currentWaveNumber++;     // 웨이브 번호 증가

        // 랜덤 모드일 때는 무한히 웨이브 계속 생성
        bool isRandomMode = useRandomEnemyStats && map.useRandomMaps;

        if (currentWaveNumber - 1 < waves.Length || isRandomMode)   //현재 웨이브가 있거나 랜덤 모드라면
        {
            // 랜덤 맵이 활성화되었을 때 웨이브 생성
            if (isRandomMode)
            {
                currentWave = GenerateRandomWave();
                // 랜덤 모드에서는 웨이브가 진행될수록 난이도 증가
                AdjustDifficultyForWave(currentWaveNumber);
            }
            else
            {
                currentWave = waves[currentWaveNumber - 1];         // 기존 웨이브 데이터를 사용
            }

            enemiesRemainingToSpawn = currentWave.enemyCount;   // 이번 웨이브에서 스폰할 적 수
            enemiesRemaningAlive = enemiesRemainingToSpawn;     // 이번 웨이브에서 살아 있을 적 수

            if (OnNewWave != null)  // 새로운 웨이브 이벤트 호출
                OnNewWave(currentWaveNumber);

            if(currentWave.infinite == true)
                InfiniteWave();

            ResetPlayerPosition();
        }
    }

    /// <summary>
    /// 랜덤 웨이브 생성 함수
    /// </summary>
    Wave GenerateRandomWave()
    {
        Wave randomWave = new Wave();
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond + currentWaveNumber);

        // 적의 수 랜덤 생성
        randomWave.enemyCount = rand.Next(randomEnemySettings.minEnemyCount, randomEnemySettings.maxEnemyCount + 1);

        // 스폰 간격 랜덤 생성
        randomWave.timeBetweenSpawns = randomEnemySettings.minTimeBetweenSpawns +
            (float)rand.NextDouble() * (randomEnemySettings.maxTimeBetweenSpawns - randomEnemySettings.minTimeBetweenSpawns);

        // 이동 속도 랜덤 생성
        randomWave.moveSpeed = randomEnemySettings.minMoveSpeed +
            (float)rand.NextDouble() * (randomEnemySettings.maxMoveSpeed - randomEnemySettings.minMoveSpeed);

        // 플레이어 처치에 필요한 공격 횟수 랜덤 생성
        randomWave.hitsToKillPlayer = rand.Next(randomEnemySettings.minHitsToKillPlayer, randomEnemySettings.maxHitsToKillPlayer + 1);

        // 적 HP 랜덤 생성
        randomWave.enemyHealth = randomEnemySettings.minEnemyHealth +
            (float)rand.NextDouble() * (randomEnemySettings.maxEnemyHealth - randomEnemySettings.minEnemyHealth);

        // 적의 색상 랜덤 생성
        randomWave.skinColor = GenerateRandomEnemyColor(rand);

        // 무한 웨이브 설정 (기본 false)
        randomWave.infinite = false;

        return randomWave;
    }

    /// <summary>
    /// 랜덤 적 색상 생성
    /// </summary>
    Color GenerateRandomEnemyColor(System.Random rand)
    {
        float hue = (float)rand.NextDouble();
        float saturation = 0.6f + (float)rand.NextDouble() * 0.4f; // 0.6 ~ 1.0
        float value = 0.4f + (float)rand.NextDouble() * 0.6f; // 0.4 ~ 1.0

        return Color.HSVToRGB(hue, saturation, value);
    }

    /// <summary>
    /// 웨이브에 따른 난이도 조정
    /// </summary>
    void AdjustDifficultyForWave(int waveNumber)
    {
        // 웨이브가 진행될수록 난이도 증가 (최대 20% 증가)
        float difficultyMultiplier = 1f + Mathf.Min(waveNumber * 0.05f, 0.2f);

        // 적의 수 증가
        currentWave.enemyCount = Mathf.RoundToInt(currentWave.enemyCount * difficultyMultiplier);

        // 적 체력 증가
        currentWave.enemyHealth *= difficultyMultiplier;

        // 적 이동 속도 증가
        currentWave.moveSpeed *= (1f + Mathf.Min(waveNumber * 0.02f, 0.2f));

        // 스폰 간격 감소 (더 빠르게 스폰)
        currentWave.timeBetweenSpawns *= (1f - Mathf.Min(waveNumber * 0.01f, 0.3f));

        Debug.Log($"웨이브 {waveNumber} 난이도 조정: 적 수={currentWave.enemyCount}, 체력={currentWave.enemyHealth:F1}, 속도={currentWave.moveSpeed:F1}, 스폰간격={currentWave.timeBetweenSpawns:F1}");
    }

    [System.Serializable]
    public class RandomEnemySettings
    {
        [Header("Enemy Count Range")]
        public int minEnemyCount = 3;
        public int maxEnemyCount = 15;

        [Header("Spawn Time Range")]
        public float minTimeBetweenSpawns = 0.5f;
        public float maxTimeBetweenSpawns = 3f;

        [Header("Movement Speed Range")]
        public float minMoveSpeed = 2f;
        public float maxMoveSpeed = 8f;

        [Header("Player Damage Range")]
        public int minHitsToKillPlayer = 2;
        public int maxHitsToKillPlayer = 6;

        [Header("Enemy Health Range")]
        public float minEnemyHealth = 1f;
        public float maxEnemyHealth = 5f;
    }

    [System.Serializable]   //Inspector 창에 보이게 해주기
    public class Wave
    {
        public bool infinite;           //무한 웨이브가 활성화인가?
        public int enemyCount;          //적의 수
        public float timeBetweenSpawns; //스폰 간격
        public float moveSpeed;         //이동 속도
        public int hitsToKillPlayer;    //플레이어 처치 횟수
        public float enemyHealth;       //적 HP
        public Color skinColor;         //색상
    }
}