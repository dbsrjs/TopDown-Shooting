using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighlightColor;
    Color originalDotColor;

    [HideInInspector] public bool isTargetingEnemy = false; // 적을 조준하고 있는지 여부

    private void Start()
    {
        Cursor.visible = false; //마우스 커서를 숨김
        originalDotColor = dot.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    /// <summary>
    /// 타겟 감지
    /// </summary>
    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
            isTargetingEnemy = true; // 적을 조준 중
        }
        else
        {
            dot.color = originalDotColor;
            isTargetingEnemy = false; // 적을 조준하지 않음
        }
    }
}