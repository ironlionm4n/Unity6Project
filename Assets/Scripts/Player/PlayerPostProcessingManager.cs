using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerPostProcessingManager : MonoBehaviour
{
    [SerializeField] PlayerAttack playerAttack;
    private Volume _volume;
    private Bloom _bloom;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _volume = GetComponentInChildren<Volume>();   
        if(_volume.profile.TryGet<Bloom>(out var bloom))
        {
            _bloom = bloom;
        }
        else
        {
            Debug.LogError("Bloom not found in volume profile");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_bloom != null)
        {
            // adjust the bloom from 1 to 10 based on the players ratio of stamina to max stamina
            _bloom.intensity.value = 1 + 9 * playerAttack.GetRatioOfMaxStamina();
        }
    }
}
