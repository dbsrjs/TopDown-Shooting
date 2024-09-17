using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private bool devMode;  //Dev mode
    LivingEntity playerEntity;
    Transform playerT;

    public Wave[] waves;
    public Enemy enemy;             //�� ������

    Wave currentWave;               //���� ���̺� ������
    int currentWaveNumber;          //���� ���̺� ��ȣ

    int enemiesRemainingToSpawn;    //�����ִ� �����ؾ��� ��
    int enemiesRemaningAlive;       //��� �ִ� ���� ��
    float nextSpawnTime;

    MapGenerator map;               //�� ������ ����

    float timeBetweenCampingChecks = 2;  //�󸶳� ���� ������ üũ�� ���ΰ�.
    float campThresholdDistance = 1.5f;  //������ ���ֵ��� �������� �̵��ؾ� �� �ּ� �Ÿ�
    float nextCampCheckTime;             //���� ������ üũ�ϴ� �ð�. 
    Vector3 campPositionOld;             //������ ���� üũ �� �÷��̾� ��ġ
    bool isCamping;                      //���� ����     true : ������

    bool isDisabled;                     //�÷��̾ �׾��� �� �÷��̾� ���� ��ɵ��� ��Ȱ��Ȱ ������.

    public event System.Action<int> OnNewWave;  //���ο� ���̺� ���� �� ȣ��Ǵ� �̺�Ʈ

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave(); // ù ���̺� ����
    }

    private void Update()
    {
        if (!isDisabled) //�÷��̾ ���� �ʾ��� ��
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;   //���� ���� üũ �ð�

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);  // �÷��̾ �̵��ߴ��� Ȯ��
                campPositionOld = playerT.position; // ���� ��ġ�� ���� üũ ���� ��ġ�� ����
            }

            // �� ���� ó��
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;  // ���� ���� �� ����
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;  // ���� ���� �ð� ����

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StopCoroutine("SpawnEnemy");    //�� ���� ����
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())  //��� �� ����
                    Destroy(enemy.gameObject);

                NextWave(); //���� ����� �̵�
            }
        }
    }

    /// <summary>
    /// �� ����
    /// </summary>
    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;       //���� ��� �ð�
        float tileFlashSpeed = 4;   //Ÿ���� ��¦�Ÿ��� �ӵ�

        // ���� ������ Ÿ���� ����
        Transform spawnTile = map.GetRandomOpenTile();

        if (isCamping)  //�÷��̾ ���� ���̶�� �÷��̾� ��ġ�� ����
            spawnTile = map.GetTileFromPosition(playerT.position);

        Material tileMat = spawnTile.GetComponent<Renderer>().material;

        Color initialColor = Color.white;   //�⺻ ��
        Color flashColor = Color.red;       //Ÿ���� ������ �� ��
        float spawnTimer = 0;

        // ���� ��� �� Ÿ���� ������
        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));   //���� �����ϴ� ��ġ�� Ÿ���� �����Ÿ��� �ؼ� ���� ������ ��ġ�� �˷���.

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        // �� ����
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;// �� ����
        spawnedEnemy.OnDeath += OnEnemyDeath;   // ���� �׾��� �� ȣ��� �޼��� ���
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor); // �� �Ӽ� ����
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
        enemiesRemaningAlive--; // ����ִ� �� �� ����

        if (enemiesRemaningAlive == 0)  // ��� ���� �׾����� ���� ���̺�� �̵�
            NextWave();
    }

    /// <summary>
    /// ���ο� ���̺갡 ���� �� ������ �÷��̾��� ��ġ �ʱ�ȭ
    /// </summary>
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2.5f;
    }

    /// <summary>
    /// ���� ���̺�� �̵��ϴ� �޼���
    /// </summary>
    void NextWave()
    {
        if (currentWaveNumber > 0)  // ���̺� �Ϸ� ���� ���
            AudioManager.instance.PlaySound2D("Level Complete");

        currentWaveNumber++;     // ���̺� ��ȣ ����

        if (currentWaveNumber - 1 < waves.Length)   //���� ���̺갡 �ִٸ�..?
        {
            currentWave = waves[currentWaveNumber - 1]; // ���� ���̺� �����͸� ����

            enemiesRemainingToSpawn = currentWave.enemyCount;   // �̹� ���̺꿡�� ������ �� ��
            enemiesRemaningAlive = enemiesRemainingToSpawn;     // �̹� ���̺꿡�� ���� �� ��

            if (OnNewWave != null)  // ���ο� ���̺� �̺�Ʈ ȣ��
                OnNewWave(currentWaveNumber);

            ResetPlayerPosition();
        }
    }

    [System.Serializable]   //Inspector
    public class Wave
    {
        public bool infinite;           //���� ���̺갡 �����Ѱ�?
        public int enemyCount;          //���� ��
        public float timeBetweenSpawns; //���� ����
        public float moveSpeed;         //�̵� �ӵ�
        public int hitsToKillPlayer;    //�÷��̾� HP
        public float enemyHealth;       //�� HP
        public Color skinColor;         //����
    }
}