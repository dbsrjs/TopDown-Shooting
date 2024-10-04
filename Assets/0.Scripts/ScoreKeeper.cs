using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper instance;
    public static long score { get; private set; }
    float lastEnemyKillTime;    //마지막으로 적을 죽인 시간
    int streakCount;            //적을 연속으로 죽인 횟수
    float streakExpiryTime = 0.6f; //연속 킬이 유지되는 시간

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled()
    {
        if(Time.time < lastEnemyKillTime + streakExpiryTime)    // 만약 적을 죽인 시간이 연속 킬 시간(streakExpiryTime) 내라면 스트릭 카운트를 증가
            streakCount++;
        else
            streakCount = 0;

        lastEnemyKillTime = Time.time;

        // 점수 계산: 기본 점수(5)에 스트릭 보너스를 추가 (2의 스트릭 카운트 제곱을 더함)
        score += 5 + (long)Mathf.Pow(1.5f, streakCount);
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }

    /// <summary>
    /// 스코어 초기화
    /// </summary>
    public void ReScore()
    {
        score = 0;
    }
}
