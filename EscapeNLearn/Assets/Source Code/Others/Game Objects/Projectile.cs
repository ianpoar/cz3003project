using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 2;
    public float speed = 2;
    public void Init(int direction)
    {
        Vector3 vec = Vector3.zero;
        switch (direction)
        {
            case 0:
                vec = new Vector3(0, 1, 0);
                break;
            case 1:
                vec = new Vector3(0, -1, 0);
                this.transform.Rotate(new Vector3(0, 0, 180));
                break;
            case 2:
                vec = new Vector3(1, 0, 0);
                this.transform.Rotate(new Vector3(0, 0, -90));
                break;
            case 3:
                vec = new Vector3(-1, 0, 0);
                this.transform.Rotate(new Vector3(0, 0, 90));
                break;
            default:
                break;
        }
        StartCoroutine(Sequence_Fire(vec));
    }

    IEnumerator Sequence_Fire(Vector3 vector)
    {
        bool run = true;
        while (run)
        {
            this.transform.localPosition += vector * Time.deltaTime * speed;
            Debug.Log(vector);
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
                run = false;
            yield return new WaitForSeconds(0);
        }

        Destroy(this.gameObject);
    }
}
