using UnityEngine;
[System.Serializable]
public class GridRandom
{
    const int MAX_SEED_VALUE = 100000;
    [Header("Leave seed at 0 for random value")]
    public int seed;
    public void GenerateSeed() {
        while (seed == 0) {
            seed = Random.Range(MAX_SEED_VALUE, MAX_SEED_VALUE*10);
            seed *= Random.Range(0,1)*2 - 1;
            Debug.Log("Generating Seed");
        }
    }
    public float GetRandomValue(Vector2Int position) {
        var oldState = Random.state;
        position += new Vector2Int(seed, seed);
        long hash = position.x;
        hash = hash + 0xabcd1234 + (hash << 15);
        hash = hash + 0x0987efab ^ (hash >> 11);
        hash ^= position.y;
        hash = hash + 0x46ac12fd + (hash << 7);
        hash = hash + 0xbe9730af ^ (hash << 11);
        Random.InitState((int)hash);
        float value = Random.value;
        Random.state = oldState;
        return value;
    }


}
