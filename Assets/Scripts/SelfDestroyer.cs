using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{
    [SerializeField] private float detroyerCountdown = 1;
    void Start()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem!=null)
        {
            detroyerCountdown = particleSystem.duration;
        }
    }

    // Update is called once per frame
    void Update()
    {
        detroyerCountdown -= Time.deltaTime;
        if (detroyerCountdown<=0f)
        {
            Destroy(this);
        }
    }
}
