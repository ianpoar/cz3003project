using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fading3DText : MonoBehaviour
{
    public float speed;
    public float fadespeed;
    public TextMeshPro script;
    float alpha = 1;

    // Update is called once per frame
    void Update()
    {
        alpha -= fadespeed * Time.deltaTime;
        this.transform.Translate(new Vector2(0, Time.deltaTime * speed));
        script.color = new Color(script.color.r, script.color.g, script.color.b, alpha);

        if (alpha <= 0)
            Destroy(this.gameObject);
    }
}
