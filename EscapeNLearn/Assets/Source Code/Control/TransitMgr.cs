﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Transition Manager Subsystem Interface, a Control Class that handles all in-game transitions.
/// </summary>
public class TransitMgr : MonoBehaviour
{

    /// <summary>
    /// Singleton instance.
    /// </summary>
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

    // Private variables
    private float speed = 1.5f;

    /// <summary>
    /// Fades the current screen. Call this in UI classes to hide the current screen
    /// </summary>
    public void Fade(SimpleCallback cb = null)
    {
        StartCoroutine(Sequence_Transit(true, cb));
    }

    /// <summary>
    /// Fades the current screen and switches the scene. Call this in UI classes to transit to another scene
    /// </summary>
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(Sequence_Transit(true, delegate()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); // load scene
            AudioMgr.Instance.StopBGM(); // stop bgm
            Emerge(); // then emerge
        }));
    }

    /// <summary>
    /// Emerges the current screen. Call this in UI classes when you want to show the current screen
    /// </summary>
    public void Emerge(SimpleCallback cb = null)
    {
        StartCoroutine(Sequence_Transit(false, cb));
    }

    /// <summary>
    /// Transition coroutine.
    /// </summary>
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
            alpha -= Time.deltaTime * speed;
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
            alpha += Time.deltaTime * speed;
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
