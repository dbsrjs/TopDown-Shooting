using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject GameOverUI;

    public RectTransform newWaveBanner; //새로운 웨이브가 시작 할 때 뜨는 베너
    public Text newWaveTitle;           //베너에 뜨는 현재 웨이브
    public Text newWaveEnemyCount;      //현재 웨이브에서 스폰되는 적 수
    public Text scoreText;              //점수
    public Text gameoverScoreText;      //게임 오버 점수
    public RectTransform healthBar;     //HP  Bar

    Spawner spawner;
    Player player;

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Update()
    {
        scoreText.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if(player != null)
            healthPercent = player.health / player.startHealth;

        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    /// <summary>
    /// 베너 정보
    /// </summary>
    void OnNewWave(int waveNumber)
    {
        //현재 웨이브
        string[] numbers = { "One", "Two", "Tree", "Four", "Five" };
        newWaveTitle.text = $"- Wave {numbers[waveNumber - 1]} -";

        //스폰할 적 수
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = $"Enemies: {enemyCountString}";

        //애니메이션
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    /// <summary>
    /// 베너 애니메이션
    /// </summary>
    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 2f;
        float speed = 2.4f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if(animatePercent >= 1)
            {
                animatePercent = 1;
                if(Time.time > endDelayTime)
                    dir = -1;
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-290, 0, animatePercent);
            yield return null;
        }
    }

    /// <summary>
    /// 죽었을 때 Fade 올리기
    /// </summary>
    /// <returns></returns>
    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
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
}
