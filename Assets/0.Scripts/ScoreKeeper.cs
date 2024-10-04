using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper instance;
    public static long score { get; private set; }
    float lastEnemyKillTime;    //���������� ���� ���� �ð�
    int streakCount;            //���� �������� ���� Ƚ��
    float streakExpiryTime = 0.6f; //���� ų�� �����Ǵ� �ð�

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
        if(Time.time < lastEnemyKillTime + streakExpiryTime)    // ���� ���� ���� �ð��� ���� ų �ð�(streakExpiryTime) ����� ��Ʈ�� ī��Ʈ�� ����
            streakCount++;
        else
            streakCount = 0;

        lastEnemyKillTime = Time.time;

        // ���� ���: �⺻ ����(5)�� ��Ʈ�� ���ʽ��� �߰� (2�� ��Ʈ�� ī��Ʈ ������ ����)
        score += 5 + (long)Mathf.Pow(1.5f, streakCount);
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }

    /// <summary>
    /// ���ھ� �ʱ�ȭ
    /// </summary>
    public void ReScore()
    {
        score = 0;
    }
}
