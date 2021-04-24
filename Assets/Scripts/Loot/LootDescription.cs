using System.Collections.Generic;
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
        if (drops.Length == 0)
        {
            Debug.LogError("No Drops assigned to lootable object");
            return null;
        }
        List<DropProbabilityPair> pairs = new List<DropProbabilityPair>();
        float rnd = Random.value;
        for (int i = 0; i < drops.Length; i++)
        {
            if (rnd < drops[i].Probability)
            {
                pairs.Add(drops[i]);

            }
        }
        if (pairs.Count==0)
        {
            return SelectDropRandomly();
        }
        if (pairs.Count == 1)
        {
            return pairs[0].Drop;
        }
        if (pairs.Count>=2)
        {
            return pairs[Random.Range(0, pairs.Count)].Drop;
        }
        Debug.LogWarning("Couldn't select a random Drop");
        return null;
    }
}

[System.Serializable]
public struct DropProbabilityPair
{
    public Drop Drop;

    [Range(0, 1)]
    public float Probability;
}
