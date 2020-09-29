using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject TargetObject;
    public string message;

    private void OnTriggerEnter2D (Collider2D col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("trigger");
            TargetObject.SendMessage(message);
        }
    }
}
