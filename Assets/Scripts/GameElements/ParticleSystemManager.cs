using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleSystemManager : MonoBehaviour
{
    private static ParticleSystemManager _instance;
    public static ParticleSystemManager Instance => _instance;

    [SerializeField] private GameObject spiderBombParticlePrefab;
    [SerializeField] private BaseStats baseStats;
    
    private ObjectPool<GameObject> _spiderBombParticlePool;

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

    private void Start()
    {
        _spiderBombParticlePool =
            new ObjectPool<GameObject>(HandleOnCreate, HandleOnGet, HandleOnRelease, null,
                true, 10, 20);
    }

    private GameObject HandleOnCreate()
    {
        var particle = Instantiate(spiderBombParticlePrefab, transform);
        particle.transform.position = Vector3.zero;
        particle.SetActive(false);
        return particle;
    }

    private void HandleOnRelease(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.position = Vector3.zero;
    }

    private void HandleOnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void SpawnSpiderBombParticle(Vector3 position)
    {
        var particle = _spiderBombParticlePool.Get();
        particle.transform.position = position;
        StartCoroutine(ReturnSpiderBombParticle(particle));
    }

    private IEnumerator ReturnSpiderBombParticle(GameObject particle)
    {
        yield return new WaitForSeconds(baseStats.PrimaryFloatStat);
        _spiderBombParticlePool.Release(particle);
    }
}