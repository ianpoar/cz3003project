using UnityEngine;
using System.Collections;

using UnityEngine.UI;

/// <summary>
/// A UI helper class that provides floating UI animations.
/// </summary>
public class FloatingUIScript : MonoBehaviour
{
    /* ADJUSTABLE VARIABLES */
    [Header("Adjustable Variables")]
    public int m_Direction = 1;
    public bool m_ScrollFunctions;
    public float m_MaxSpeed = 0.08f;
    public float m_Acceleration = 0.15f;

    private Button m_Button = null;
    private float m_Speed;
    private bool m_Accelerating = false;

    void Awake()
    {
        if (m_ScrollFunctions)
        {
            m_Button = this.GetComponent<Button>();
        }

        m_Speed = m_MaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(0, m_Speed * Time.deltaTime * m_Direction, 0);

        if (m_Accelerating)
        {
            m_Speed += m_Acceleration * Time.deltaTime;

            if (m_Speed >= m_MaxSpeed)
            {
                m_Speed = m_MaxSpeed;
                m_Accelerating = false;
            }
        }
        else
        {
            m_Speed -= m_Acceleration * Time.deltaTime;

            if (m_Speed <= 0)
            {
                m_Speed = 0;
                m_Accelerating = true;
                m_Direction = -m_Direction;
            }
        }
    }

    public void Select()
    {
        if (!m_ScrollFunctions)
        {
            return;
        }

        StopAllCoroutines();
        m_Button.interactable = true;
        StartCoroutine(Enlarge(1.1f));
    }

    public void DeSelect()
    {
        if (!m_ScrollFunctions)
        {
            return;
        }

        StopAllCoroutines();
        m_Button.interactable = false;
        StartCoroutine(Shrink(0.6f));
    }

    IEnumerator Enlarge(float scale)
    {
        float speed = 2;
        bool done = false;

        while (!done)
        {
            this.transform.localScale += new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, 0);
            if (this.transform.localScale.x >= scale)
            {
                this.transform.localScale = new Vector3(scale, scale, 1);
                done = true;
            }

            yield return new WaitForSeconds(0);
        }
    }

    IEnumerator Shrink(float scale)
    {
        float speed = 5;
        bool done = false;

        while (!done)
        {
            this.transform.localScale -= new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, 0);
            if (this.transform.localScale.x <= scale)
            {
                this.transform.localScale = new Vector3(scale, scale, 1);
                done = true;
            }

            yield return new WaitForSeconds(0);
        }
    }
}
