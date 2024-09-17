using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private bool devMode;  //Dev mode
    LivingEntity playerEntity;
    Transform playerT;

    public Wave[] waves;
    public Enemy enemy;             //적 프리팹

    Wave currentWave;               //현재 웨이브 데이터
    int currentWaveNumber;          //현재 웨이브 번호

    int enemiesRemainingToSpawn;    //남아있는 스폰해야할 적
    int enemiesRemaningAlive;       //살아 있는 적의 수
    float nextSpawnTime;

    MapGenerator map;               //맵 생성기 참조

    float timeBetweenCampingChecks = 2;  //얼마나 자주 존버를 체크할 것인가.
    float campThresholdDistance = 1.5f;  //존버로 간주되지 않으려면 이동해야 할 최소 거리
    float nextCampCheckTime;             //다음 존버를 체크하는 시간. 
    Vector3 campPositionOld;             //마지막 존버 체크 시 플레이어 위치
    bool isCamping;                      //존버 여부     true : 존버중

    bool isDisabled;                     //플레이어가 죽었을 때 플레이어 관련 기능들을 비활성활 시켜줌.

    public event System.Action<int> OnNewWave;  //새로운 웨이브 시작 시 호출되는 이벤트

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave(); // 첫 웨이브 시작
    }

    private void Update()
    {
        if (!isDisabled) //플레이어가 죽지 않았을 때
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;   //다음 존버 체크 시간

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);  // 플레이어가 이동했는지 확인
                campPositionOld = playerT.position; // 현재 위치를 존버 체크 기준 위치로 저장
            }

            // 적 스폰 처리
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;  // 남은 스폰 수 감소
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;  // 다음 스폰 시간 설정

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StopCoroutine("SpawnEnemy");    //적 스폰 중지
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())  //모든 적 삭제
                    Destroy(enemy.gameObject);

                NextWave(); //다음 라운드로 이동
            }
        }
    }

    /// <summary>
    /// 적 생성
    /// </summary>
    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;       //생성 대기 시간
        float tileFlashSpeed = 4;   //타일이 반짝거리는 속도

        // 적이 스폰될 타일을 선택
        Transform spawnTile = map.GetRandomOpenTile();

        if (isCamping)  //플레이어가 존버 중이라면 플레이어 위치에 스폰
            spawnTile = map.GetTileFromPosition(playerT.position);

        Material tileMat = spawnTile.GetComponent<Renderer>().material;

        Color initialColor = Color.white;   //기본 색
        Color flashColor = Color.red;       //타일이 깜빡일 때 색
        float spawnTimer = 0;

        // 스폰 대기 중 타일이 깜빡임
        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));   //적이 스폰하는 위치의 타일을 깜빡거리게 해서 적이 스폰할 위치를 알려줌.

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        // 적 스폰
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

        if (enemiesRemaningAlive == 0)  // 모든 적이 죽었으면 다음 웨이브로 이동
            NextWave();
    }

    /// <summary>
    /// 새로운 웨이브가 시작 될 때마다 플레이어의 위치 초기화
    /// </summary>
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2.5f;
    }

    /// <summary>
    /// 다음 웨이브로 이동하는 메서드
    /// </summary>
    void NextWave()
    {
        if (currentWaveNumber > 0)  // 웨이브 완료 사운드 재생
            AudioManager.instance.PlaySound2D("Level Complete");

        currentWaveNumber++;     // 웨이브 번호 증가

        if (currentWaveNumber - 1 < waves.Length)   //다음 웨이브가 있다면..?
        {
            currentWave = waves[currentWaveNumber - 1]; // 현재 웨이브 데이터를 갱신

            enemiesRemainingToSpawn = currentWave.enemyCount;   // 이번 웨이브에서 스폰할 적 수
            enemiesRemaningAlive = enemiesRemainingToSpawn;     // 이번 웨이브에서 남은 적 수

            if (OnNewWave != null)  // 새로운 웨이브 이벤트 호출
                OnNewWave(currentWaveNumber);

            ResetPlayerPosition();
        }
    }

    [System.Serializable]   //Inspector
    public class Wave
    {
        public bool infinite;           //현재 웨이브가 무한한가?
        public int enemyCount;          //적의 수
        public float timeBetweenSpawns; //스폰 간격
        public float moveSpeed;         //이동 속도
        public int hitsToKillPlayer;    //플레이어 HP
        public float enemyHealth;       //적 HP
        public Color skinColor;         //색상
    }
}