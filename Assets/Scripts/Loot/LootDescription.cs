using UnityEngine;

[CreateAssetMenu]
public class LootDescription : ScriptableObject
{
    [SerializeField] private DropProbabilityPair[] drops;

    public void SetDrops(params DropProbabilityPair[] drops)
    {
        this.drops = drops;
    }

    public Drop SelectDropRandomly()
    {
        for (int i = 0; i < drops.Length; i++)
        {
            float rnd = Random.value;
            DropProbabilityPair pair = drops[i];

            if (rnd < pair.Probability)
            {
                return pair.Drop;
            }
        }
        //returns eighter Drop1 Gold or Drop2 Soul by a 50% chance if old Probability drop calculation would fail for all drops in drops[]
        return Random.value < 0.5 ? drops[0].Drop : drops[1].Drop;
        // we we would return null we spawn nothing
        //return null;
    }
}

[System.Serializable]
public struct DropProbabilityPair
{
    public Drop Drop;

    [Range(0, 1)]
    public float Probability;
}
