using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    static List<Enemy> ActiveEnemyList;

    static EnemySpawner _intance;

    public EnemySpawner() {
        if (_intance == null)
        {
            _intance = this;
            ActiveEnemyList = new List<Enemy>();
        }
    }

    static void AddEnemyToList(Enemy enm) { }
    static void RemoveEnemyFromList(Enemy enm) { }
    public void SpawnEnemy() { }
    static void DestroyAllEnemys() { }
}
