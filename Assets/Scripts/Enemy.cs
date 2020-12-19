using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public EnemySO enemySO;

    enum EnemyState { Move, Attack, Idle, Death}
    EnemyState enemyState;
    // work on AI -> Move , attack , idle ,death  (Design patten - > State Machine)
}
