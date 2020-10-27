using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieGraph : MonoBehaviour
{
    public float[] values;
    public Color[] wedgeColors;
    public GameObject wedgePrefab;
    int k;
    int v;


    //Start is called before the first frame update
    public void Start()
    {
    }

    public void MakeGraph(int  k1, int v1){
      k = k1;
      v = v1;
       float [] values = new float [2];
       values[0] =k;
       Debug.Log(values[0]);
       values[1] = v;

       Debug.Log(values[1]);
       float zRotation = 0f;

       for (int i = 0 ; i< values.Length; i++){
         GameObject newWedge = Instantiate(wedgePrefab, transform.position, Quaternion.identity);
         Image newWedgeImage = newWedge.GetComponent<Image>();
         newWedgeImage.transform.SetParent(transform);
         newWedgeImage.color = wedgeColors[i];
         newWedgeImage.fillAmount = values[i]/values[0];
         newWedge.transform.rotation = Quaternion.Euler (new Vector3 (0,0, zRotation));
         zRotation -= newWedgeImage.fillAmount *360f;
         newWedgeImage.transform.localScale = wedgePrefab.transform.localScale;

       }
    }

}
