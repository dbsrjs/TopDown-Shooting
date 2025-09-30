using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float forceMin;
    public float forceMax;

    float lifeTime = 4;
    float fadeTime = 2;

    void Start()
    {
        float force  = Random.Range(forceMin, forceMax);
        rigidbody.AddForce(-transform.forward * force);
        rigidbody.AddTorque(Random.insideUnitSphere * force);   //날아가는 방향 랜덤.

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        float percent = 0;
        float fadeSpeed = 1 / percent;
        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
