using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject GameOverUI;

    public RectTransform newWaveBanner;  //새로운 웨이브가 시작 될 때 올라오는 배너
    public Text newWaveTitle;            //배너에 올라올 새로운 웨이브
    public Text newWaveEnemyCount;       //현재 웨이브에서 생성되는 적 수
    public Text scoreText;               //점수
    public Text enemyCountText;          //남은 적 수
    public Text accuracyText;            //정확도
    public Text ammoText;                //탄약 정보
    public Text criticalAccuracyText;    //치명타 명중률
    public Text gameoverScoreText;       //게임 종료 점수
    public RectTransform healthBar;      //HP  Bar

    Spawner spawner;
    Player player;
    GunController gunController;

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
        gunController = FindObjectOfType<GunController>();
    }

    private void Update()
    {
        // 게임이 일시정지 상태라면 UI 업데이트 중단
        if (Time.timeScale == 0f)
            return;

        if (ScoreKeeper.score < 999999)
            scoreText.text = ScoreKeeper.score.ToString("D6");
        else
            scoreText.text = ScoreKeeper.score.ToString();

        float healthPercent = 0;
        if (player != null)
            healthPercent = player.health / player.startHealth;

        healthBar.localScale = new Vector3(healthPercent, 1, 1);

        EnemyCount();
        AccuracyCount();
        CriticalAccuracyCount();
        AmmoCount();
    }

    /// <summary>
    /// 웨이브 시작
    /// </summary>
    void OnNewWave(int waveNumber)
    {
        // 랜덤 모드인지 확인
        bool isRandomMode = GameSettings.Instance != null && GameSettings.Instance.IsRandomMapMode();

        //웨이브 타이틀
        if (isRandomMode)
        {
            // 랜덤 모드에서는 숫자로 표시
            newWaveTitle.text = $"- Wave {waveNumber} -";
        }
        else
        {
            // 일반 모드에서는 기존 방식 사용
            string[] numbers = { "One", "Two", "Three", "Four", "Five" };
            if (waveNumber <= numbers.Length)
                newWaveTitle.text = $"- Wave {numbers[waveNumber - 1]} -";
            else
                newWaveTitle.text = $"- Wave {waveNumber} -";
        }

        //생성될 적 수
        string enemyCountString;
        if (isRandomMode)
        {
            // 랜덤 모드에서는 현재 웨이브의 적 수 표시
            enemyCountString = spawner.enemiesRemaningAlive.ToString();
        }
        else
        {
            // 일반 모드에서는 기존 방식 사용
            if (waveNumber <= spawner.waves.Length)
                enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
            else
                enemyCountString = "Infinite";
        }

        newWaveEnemyCount.text = $"Enemies: {enemyCountString}";

        //애니메이션
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    /// <summary>
    /// 배너 애니메이션
    /// </summary>
    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 2f;
        float speed = 2.4f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                    dir = -1;
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-290, 0, animatePercent);
            yield return null;
        }
    }

    /// <summary>
    /// 죽었을 때 Fade 효과
    /// </summary>
    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.95f), 1));
        gameoverScoreText.text = scoreText.text;
        scoreText.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        GameOverUI.SetActive(true);
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// 메뉴로 돌아가기
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// 남은 적 표시
    /// </summary>
    public void EnemyCount()
    {
        // 랜덤 모드에서는 무한 웨이브 표시 안함
        bool isRandomMode = GameSettings.Instance != null && GameSettings.Instance.IsRandomMapMode();

        if (!isRandomMode && Spawner.Instance.currentWaveNumber == 5)
        {
            enemyCountText.text = $"Enemies: ∞";
            return;
        }

        enemyCountText.text = $"Enemies: {Spawner.Instance.enemiesRemaningAlive}";
    }

    /// <summary>
    /// 정확도 표시
    /// </summary>
    public void AccuracyCount()
    {
        accuracyText.text = $"Accuracy: {Mathf.RoundToInt(Accuracy.Instance.accuracy)}%";
    }

    /// <summary>
    /// 치명타 명중률 표시
    /// </summary>
    public void CriticalAccuracyCount()
    {
        if (criticalAccuracyText != null)
        {
            criticalAccuracyText.text = $"Critical: {Mathf.RoundToInt(Accuracy.Instance.criticalAccuracy)}%";
        }
    }

    /// <summary>
    /// 탄약 정보 표시
    /// </summary>
    public void AmmoCount()
    {
        if (ammoText != null && gunController != null && gunController.EquippedGun != null)
        {
            Gun gun = gunController.EquippedGun;
            ammoText.text = $"Ammo: {gun.bulletRemainingInMag}/{gun.bulletPerMag}";
        }
    }
}