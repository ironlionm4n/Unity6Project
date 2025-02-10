using System;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    private static ParticleSystemManager _instance;
    public static ParticleSystemManager Instance => _instance;
    
    [SerializeField] private GameObject spiderBombParticlePrefab;
    
    // TODO: Add more particle prefabs here
    
    // TODO: Make a pool for each particle system to avoid instantiating and destroying them every time

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SpawnSpiderBombParticle(Vector3 position)
    {
        Instantiate(spiderBombParticlePrefab, position, Quaternion.identity);
    }
}
