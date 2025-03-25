using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accuracy : MonoBehaviour
{
    public static Accuracy Instance;

    private int hits { get; set; } = 0;     //¸íÁß È½¼ö
    private int shots { get; set; } = 0;    //¹ß»ç È½¼ö

    public float accuracy { get; set; } = 0;


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
