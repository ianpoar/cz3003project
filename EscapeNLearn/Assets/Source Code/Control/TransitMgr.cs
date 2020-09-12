using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitMgr : MonoBehaviour // Singleton
{
    // Singleton implementation
    public static TransitMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // References
    [SerializeField]
    private GameObject canvas_transit;
    [SerializeField]
    private Image bg;

    public void Fade(SimpleCallback cb = null)
    {
        StartCoroutine(Sequence_Transit(true, cb));
    }

    public void Emerge(SimpleCallback cb = null)
    {
        StartCoroutine(Sequence_Transit(false, cb));
    }

    IEnumerator Sequence_Transit(bool fade, SimpleCallback cb = null)
    {
        bool fading = false;
        bool emerging = true;
        float alpha = 1;

        if (fade)
        {
            alpha = 0;
            fading = true;
            emerging = false;
        }

        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, alpha);
        canvas_transit.SetActive(true);

        while (emerging) // mask alpha goes from 1 to 0
        {
            alpha -= Time.deltaTime * 3;
            if (alpha <= 0)
            {
                alpha = 0;
                emerging = false;
            }
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, alpha);

            yield return new WaitForSeconds(0);
        }

        while (fading) // mask alpha goes from 0 to 1
        {
            alpha += Time.deltaTime * 3;
            if (alpha >= 1)
            {
                alpha = 1;
                fading = false;
            }
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, alpha);

            yield return new WaitForSeconds(0);
        }

        if (!fade)
        {
            canvas_transit.SetActive(false);
        }

        cb?.Invoke();
    }
   
}
