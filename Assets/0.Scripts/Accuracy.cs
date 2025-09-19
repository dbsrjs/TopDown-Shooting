using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accuracy : MonoBehaviour
{
    public static Accuracy Instance;

    private int hits { get; set; } = 0;           //명중 횟수
    private int shots { get; set; } = 0;          //발사 횟수
    private int criticalHits { get; set; } = 0;   //치명타 명중 횟수

    public float accuracy { get; set; } = 0;
    public float criticalAccuracy { get; set; } = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// 발사수 증가
    /// </summary>
    public void IncrementShots()
    {
        shots++;
        UpdateAccuracy();
    }

    /// <summary>
    /// 명중률 증가
    /// </summary>
    public void IncrementHits()
    {
        hits++;
        UpdateAccuracy();
    }

    /// <summary>
    /// 치명타 명중 증가
    /// </summary>
    public void IncrementCriticalHits()
    {
        criticalHits++;
        UpdateAccuracy();
    }

    /// <summary>
    /// 명중률 계산
    /// </summary>
    public void UpdateAccuracy()
    {
        if (shots == 0)
        {
            accuracy = 0;
            criticalAccuracy = 0;
        }
        else
        {
            accuracy = (float)hits / shots * 100f;
            criticalAccuracy = (float)criticalHits / shots * 100f;
        }
    }
}