using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]   //Inspector
    public class Wave
    {
        public int enemyCount;          //���� ��
        public float timeBetweenSpawns; //���� ����
    }

    LivingEntity playerEntity;
    Transform playerT;

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;    //�����ִ� �����ؾ��� ��
    int enemiesRemaningAlive;       //��� �ִ� ���� ��
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2; //�󸶳� ���� ������ üũ�� ���ΰ�.
    float campThresholdDistance = 1.5f; //�����Ѱɷ� ���ֵ��� �ʱ� ���ؼ� �÷��̾ ķ�� üũ ���̿� ���������� �ּ� �Ÿ�
    float nextCampCheckTime;            //���� ������ üũ�ϴ� �ð�. 
    Vector3 campPositionOld;             //���� �ֱٿ� ���� üũ�� ���� �� �÷��̾ �־��� ���
    bool isCamping;                     //ķ�� ����     true : ������

    bool isDisabled;        //�÷��̾ �׾��� �� �÷��̾� ���� ��ɵ��� ��Ȱ��Ȱ ������.

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

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;       //���� ��� �ð�
        float tileFlashSpeed = 4;   //������ ��¦�Ÿ��� �ӵ�

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        { 
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;

        Color initialColor = tileMat.color; //�⺻ ��
        Color flashColor = Color.red;       //�÷��� ��
        float spawnTimer = 0; 

        while(spawnTimer < spawnDelay) 
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));   //���� �����ϴ� ��ġ�� Ÿ���� �����Ÿ��� �ؼ� ���� ������ ��ġ�� �˷���.

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    /// <summary>
    /// �÷��̾ �׾��� ��
    /// </summary>
    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    /// <summary>
    /// ���� �׾��� ��
    /// </summary>
    void OnEnemyDeath()
    {
        enemiesRemaningAlive--;

        if(enemiesRemaningAlive == 0)
            NextWave();
    }

    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemaningAlive = enemiesRemainingToSpawn;
        }
    }
}