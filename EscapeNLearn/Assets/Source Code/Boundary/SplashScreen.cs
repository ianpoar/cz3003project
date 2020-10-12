using System.Collections;
using UnityEngine;

/// <summary>
/// UI Boundary Class for the Splash scene, handles all UI-related events in the scene.
/// </summary>
public class SplashScreen : Screen
{
    /// <summary>
    /// Start of the Splash screen.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        StartCoroutine(SplashRoutine());
    }

    /// <summary>
    /// A coroutine for the splash animation sequence.
    /// </summary>
    private IEnumerator SplashRoutine()
    {
        TransitMgr.Instance.Emerge(); // emerge screen
        yield return new WaitForSeconds(2); // wait for 2 seconds
        TransitMgr.Instance.FadeToScene("Login");
    }
}
