using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    internal InputManager _inputManager;
    internal Animator _anim;
    public SpriteRenderer playerSprite;
    float deathAnimLength;
    float walkAnimLength;

    public float GetDeathAnimLength => deathAnimLength;
    public float GetWalkAnimLength => walkAnimLength;

    private void Start() {
        GameManager.DeathEvent += Death;
        _inputManager = InputManager._instance;
        playerSprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        UpdateAnimClipTimes();
    }

    public void UpdateAnimClipTimes() {
        AnimationClip[] clips = _anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {

                case "isWalking":
                    walkAnimLength = clip.length;
                    break;
                case "Death":
                    deathAnimLength = clip.length;
                    break;
            }
        }
    }
    public void Walk(bool DoWalk, Vector3? moveVector) {
        _anim.speed = 1;
        if (DoWalk) {
            if (moveVector.Value.x < 0) {
                playerSprite.flipX = true;
            }
            else {
                playerSprite.flipX = false;

            }
            _anim.SetBool("isRunning", true);
        }
        else {

            _anim.SetBool("isRunning", false);
        }
    }
    public void Death() {
        _anim.speed = 1;
        _anim.SetTrigger("Death");
    }
    public void Reborn() {
        _anim.SetTrigger("Reborn");
    }
}
