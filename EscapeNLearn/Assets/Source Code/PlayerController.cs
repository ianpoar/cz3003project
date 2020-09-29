using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameMgr GameManager;
    public Animator Player;
    public GameObject Projectile;
    private float walkingSpeed = 8;
    private bool attacking = false;
    private bool stopattack = false;
    private bool keydown = false;
    private bool usingKeyboard = false;

    public void ControlPlayer(Vector2 value)
    {
        if (value.magnitude > 0.5f)
            Player.SetBool("IsWalking", true);
        else
            Player.SetBool("IsWalking", false);

        if (value.x > 0.5f)
        {
            Player.SetInteger("Direction", 2);
            Player.transform.localPosition += new Vector3(Time.deltaTime * walkingSpeed, 0, 0);
        }
        else if (value.x < -0.5f)
        {
            Player.SetInteger("Direction", 3);
            Player.transform.localPosition += new Vector3(Time.deltaTime * -walkingSpeed, 0, 0);
        }

        else if (value.y > 0.5f)
        {
            Player.SetInteger("Direction", 0);
            Player.transform.localPosition += new Vector3(0, Time.deltaTime * walkingSpeed, 0);
        }
        else if (value.y < -0.5f)
        {
            Player.SetInteger("Direction", 1);
            Player.transform.localPosition += new Vector3(0, Time.deltaTime * -walkingSpeed, 0);
        }
    }

    private void Update()
    {
        
#if UNITY_EDITOR || UNITY_WINDOWS
        if (Input.GetKeyDown(KeyCode.A))
        {
            usingKeyboard = true;
            keydown = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            keydown = false;
        }
        if (usingKeyboard)
        {
            if (keydown)
                PlayerAttack(keydown);
            else if (!keydown && attacking)
            {
                usingKeyboard = false;
                PlayerAttack(false);
            }
        }
#endif
    }
    public void StopControllingPlayer(Vector2 vec)
    {
        Player.SetBool("IsWalking", false);
    }

    public void PlayerAttack(bool flag)
    {
        if (!attacking && flag)
        {
            attacking = true;
            stopattack = false;
            StartCoroutine(Sequence_Attack());
        }
        else if (attacking && !flag)
        {
            stopattack = true;
        }
    }

    IEnumerator Sequence_Attack()
    {
        bool lastAnimFinished = false;
        Player.SetBool("IsAttacking", true);

        while (!stopattack)
        {
            yield return new WaitForSeconds(0.2f);


            GameObject obj = Instantiate(Projectile, this.transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);
            Projectile proj = obj.GetComponent<Projectile>();
            proj.Init(Player.GetInteger("Direction"));
            Debug.Log("attack");

            yield return new WaitForSeconds(0.1f);
        }

        while (!lastAnimFinished)
        {
            if (Player.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Player.SetBool("IsAttacking", false);
                lastAnimFinished = true;
            }

            yield return new WaitForSeconds(0);
        }

        attacking = false;
    }

}
