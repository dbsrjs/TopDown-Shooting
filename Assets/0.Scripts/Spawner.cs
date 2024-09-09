using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private bool devMode;
    LivingEntity playerEntity;
    Transform playerT;

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;    //남아있는 스폰해야할 적
    int enemiesRemaningAlive;       //살아 있는 적의 수
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;  //얼마나 자주 존버를 체크할 것인가.
    float campThresholdDistance = 1.5f;  //존버한걸로 간주되지 않기 위해서 플레이어가 캠핑 체크 사이에 움직여야할 최소 거리
    float nextCampCheckTime;             //다음 존버를 체크하는 시간. 
    Vector3 campPositionOld;             //가장 최근에 존버 체크를 했을 때 플레이어가 있었던 장소
    bool isCamping;                      //존버 여부     true : 존버중

    bool isDisabled;        //플레이어가 죽었을 때 플레이어 관련 기능들을 비활성활 시켜줌.

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>(); 
        NextWave();
    }

    private void Update()
    {
        if(!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }

        if(devMode)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StopCoroutine("SpawnEnemy");    //적 스폰 중지
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())  //적 삭제
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
        float tileFlashSpeed = 4;   //파일이 반짝거리는 속도

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        { 
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;

        Color initialColor = Color.white; //기본 색
        Color flashColor = Color.red;       //플레쉬 색
        float spawnTimer = 0; 

        while(spawnTimer < spawnDelay) 
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));   //적이 스폰하는 위치의 타일을 깜빡거리게 해서 적이 스폰할 위치를 알려줌.

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
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
        enemiesRemaningAlive--;

        if(enemiesRemaningAlive == 0)
            NextWave();
    }

    /// <summary>
    /// 새로운 웨이브가 시작 될 때마다 플레이어의 위치 초기화
    /// </summary>
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2.5f;
    }

    void NextWave()
    {
        currentWaveNumber++;

        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemaningAlive = enemiesRemainingToSpawn;

            if(OnNewWave != null)
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
        public int hitsToKillPlayer;
        public float enemyHealth;       //적 HP
        public Color skinColor;         //색상
    }
}