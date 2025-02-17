using System;
using Player;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Camera
{
    public class CinemachineManager : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Volume postProcessingVolume;
        [SerializeField] private float chromaticAberrationIntensity = 0.5f;
        [SerializeField] private float chromaticAberrationFadeSpeed = 3f;
        [SerializeField] private CinemachineCamera playerCinemachineCamera;
        [SerializeField] private PlayerAttack playerAttack;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private GameObject deathBG;
        [SerializeField] private GameObject deathCircle;
        [SerializeField] private CinemachineCamera playerDeathCinemachineCamera;
    
        private CinemachineCamera _cinemachineCamera;
        private ChromaticAberration _chromaticAberration;

        private void Start()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
            MakeMainCinemachinePriority();
            inputManager.OnAttackPressed += HandleAttack;
            playerAttack.OnSweepAttackStarted += HandleSweepAttackStarted;
            playerAttack.OnSweepAttackPerformed += HandleSweepAttackPerformed;
            playerAttack.OnSweepAttackCanceled += HandleSweepAttackCanceled;
            
            if(postProcessingVolume.profile.TryGet(out ChromaticAberration chromaticAberration))
            {
                _chromaticAberration = chromaticAberration;
                _chromaticAberration.intensity.overrideState = true;
                _chromaticAberration.intensity.value = 0f;
            }
            
            deathBG.SetActive(false);
            deathCircle.SetActive(false);
        }

        private void OnDisable()
        {
            inputManager.OnAttackPressed -= HandleAttack;
            playerAttack.OnSweepAttackStarted -= HandleSweepAttackStarted;
            playerAttack.OnSweepAttackPerformed -= HandleSweepAttackPerformed;
            playerAttack.OnSweepAttackCanceled -= HandleSweepAttackCanceled;
        }
        
        private void MakeMainCinemachinePriority()
        {
            _cinemachineCamera.Priority = 10;
            playerCinemachineCamera.Priority = 0;
            playerDeathCinemachineCamera.Priority = 0;
        }

        private void Update()
        {
            // Gradually fade out Chromatic Aberration
            if (_chromaticAberration.intensity.value > 0)
            {
                _chromaticAberration.intensity.value = Mathf.Lerp(
                    _chromaticAberration.intensity.value, 0f, Time.deltaTime * chromaticAberrationFadeSpeed
                );
            }
        }

        private void HandleSweepAttackCanceled()
        {
            Debug.Log("Cinemachine: Sweep Attack Canceled");
            MakeMainCinemachinePriority();
        }



        private void HandleSweepAttackPerformed()
        {
            Debug.Log("Cinemachine: Sweep Attack");
            
            MakeMainCinemachinePriority();
            // Apply Chromatic Aberration
            if (_chromaticAberration != null)
            {
                _chromaticAberration.intensity.value = chromaticAberrationIntensity;
            }
        }

        private void HandleSweepAttackStarted()
        {
            if(playerAttack.CheckAnimatorStatesForBasicAttacks()) return;
            
            Debug.Log("Cinemachine: Sweep Attack Started");
            MakePlayerCinemachinePriority();
        }

        private void MakePlayerCinemachinePriority()
        {
            playerCinemachineCamera.Priority = 10;
            _cinemachineCamera.Priority = 0;
        }

        private void HandleAttack()
        {
            // Maybe some camera shake or something for when player basic attacks
            Debug.Log("Cinemachine: Attack");
        }
        
        public void MakePlayerDeathCinemachinePriority()
        {
            playerDeathCinemachineCamera.Priority = 10;
            _cinemachineCamera.Priority = 0;
            
            deathBG.SetActive(true);
            deathCircle.SetActive(true);
        }
    }
}
