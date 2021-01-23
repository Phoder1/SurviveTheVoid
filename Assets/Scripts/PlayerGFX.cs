using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    internal InputManager _inputManager;
    internal Animator _anim;
    public SpriteRenderer playerSprite;

    

    private void Start()
    {
        GameManager.DieEvent += Death;
        _inputManager = InputManager._instance;
        playerSprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        
    }
    public void UpdateAnim()
    {
       
   

    }
    public void Walk(bool DoWalk,Vector3? moveVector)
    {
        if (DoWalk)
        {
            if (moveVector.Value.x < 0)
            {
                playerSprite.flipX = true;
            }
            else
            {
                playerSprite.flipX = false;

            }


            _anim.SetBool("isWalking", true);
        }
        else
        {

            _anim.SetBool("isWalking", false);
        }
    }
    public void Death()
    {
        _anim.SetTrigger("Death");
    }

    public void TriggerAnim()
    {

    }
}
