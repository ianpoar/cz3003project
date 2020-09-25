using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : Screen
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(SplashRoutine());
    }

    private IEnumerator SplashRoutine()
    {
        TransitMgr.Instance.Emerge(); // emerge screen
        yield return new WaitForSeconds(2); // wait for 2 seconds
        TransitMgr.Instance.FadeToScene(SceneConstants.SCENE_LOGIN);
    }
}
