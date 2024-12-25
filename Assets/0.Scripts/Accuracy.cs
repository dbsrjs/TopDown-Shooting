using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accuracy : MonoBehaviour
{
    public static Accuracy Instance;

    private int hits = 0;     //¸íÁß È½¼ö
    private int shots = 0;    //¹ß»ç È½¼ö

    private float accuracy = 0;

    public int GetHits()
    {
        return hits;
    }

    public void SetHits(int _hits)
    {
        hits = _hits;
    }

    public int GetShots()
    {
        return shots;
    }

    public void SetShots(int _shots)
    {
        shots = _shots;
    }

    public float GetAccuracy()
    {
        return accuracy;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    /// <summary>
    /// ¹ß»ç·ü Á¶Àý
    /// </summary>
    public void IncrementShots()
    {
        shots++;
        UpdateAccuracy();
    }

    /// <summary>
    /// ¸íÁß·ü Á¶Àý
    /// </summary>
    public void IncrementHits()
    {
        hits++;
        UpdateAccuracy();
    }

    /// <summary>
    /// ¸íÁß·ü °è»ê
    /// </summary>
    public void UpdateAccuracy()
    {
        if (shots == 0)
            accuracy = 0;
        else
            accuracy = (float)hits / shots * 100f;
    }
}
