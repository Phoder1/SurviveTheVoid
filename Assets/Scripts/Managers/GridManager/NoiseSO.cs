using UnityEngine;
[CreateAssetMenu(fileName = "New Noise", menuName = "SO/" + "Noise")]
public class NoiseSO : ScriptableObject
{
    [Range(3f, 100f)]
    [SerializeField] private float resolution = 3;
    public float GetResolution => resolution;
    [Range(0f, 0.99f)]
    [SerializeField] private float boolThreshold;
    public float GetBoolThreshold => boolThreshold;



}
[System.Serializable]
public class Noise
{
    const int MAX_SEED_VALUE = 100000;
    [Header("Leave seed at 0 for random value")]
    public int seed;
    [SerializeField] private NoiseSO noise;

    public float threshold => noise.GetBoolThreshold;
    public void GenerateSeed() {
        while (seed == 0) {
            seed = Random.Range(MAX_SEED_VALUE, MAX_SEED_VALUE*10);
            seed *= Random.Range(0,1)*2 - 1;
            Debug.Log("Generating Seed");
        }
    }
    public float GetNoiseAtPosition(Vector2 position)
        => Mathf.PerlinNoise((position.x + seed) / noise.GetResolution, (position.y + seed) / noise.GetResolution);
    public bool CheckThreshold(Vector2Int position, bool noise, out float value) 
        => (value = noise ? GetNoiseAtPosition(position):GetRandomValue(position)) > this.noise.GetBoolThreshold;
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
