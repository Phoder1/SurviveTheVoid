using UnityEngine;
using System;

[Serializable]
public class Noise
{
    const int MAX_SEED_VALUE = 100000;
    [Header("Leave seed at 0 for random value")]
    public int seed;
    [Range(3f, 50f)]
    public float resolution = 3;
    [Range(0f, 0.99f)]
    public float boolThreshold;
    public void GenerateSeed() {
        while (seed == 0) {
            seed = UnityEngine.Random.Range(-MAX_SEED_VALUE, MAX_SEED_VALUE);
            Debug.Log("Generating Seed");
        }
    }
    public float GetNoiseAtPosition(Vector2 position)
       => Mathf.PerlinNoise((position.x + seed) / resolution, (position.y + seed) / resolution);
    public bool CheckThreshold(Vector2 position) => GetNoiseAtPosition(position) > boolThreshold;
    public float GetRandomValue(Vector2 position) {
        //Vector2Int newPosition = Vector2Int.RoundToInt(position);
        UnityEngine.Random.InitState(Mathf.RoundToInt(GetNoiseAtPosition(position) * seed));
        return UnityEngine.Random.value;
    }
}
