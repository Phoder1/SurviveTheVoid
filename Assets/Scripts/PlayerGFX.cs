using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    Animator _animator;
    public  enum PlayerAnim { };
    public  PlayerAnim currentAnim;

    void Init() { }
    public static void PlayAnimation(PlayerAnim playerAnim) { }
}
