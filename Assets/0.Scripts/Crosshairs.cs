using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighlightColor;
    Color originalDotColor;

    private void Start()
    {
        Cursor.visible = false; //���콺 ����Ʈ �����
        originalDotColor = dot.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    /// <summary>
    /// Ÿ�� ����
    /// </summary>
    public void DetectTargets(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}