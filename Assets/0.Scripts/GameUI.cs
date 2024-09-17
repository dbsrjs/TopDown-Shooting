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

    public RectTransform newWaveBanner; //���ο� ���̺갡 ���� �� �� �ߴ� ����
    public Text newWaveTitle;           //���ʿ� �ߴ� ���� ���̺�
    public Text newWaveEnemyCount;      //���� ���̺꿡�� �����Ǵ� �� ��
    public Text scoreText;              //����
    public Text gameoverScoreText;      //���� ���� ����
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
    /// ���� ����
    /// </summary>
    void OnNewWave(int waveNumber)
    {
        //���� ���̺�
        string[] numbers = { "One", "Two", "Tree", "Four", "Five" };
        newWaveTitle.text = $"- Wave {numbers[waveNumber - 1]} -";

        //������ �� ��
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = $"Enemies: {enemyCountString}";

        //�ִϸ��̼�
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    /// <summary>
    /// ���� �ִϸ��̼�
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
    /// �׾��� �� Fade �ø���
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
    /// ���� ����
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
    /// ���� �����
    /// </summary>
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// �޴��� ���ư���
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
