using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Camera
{
    public class CinemachineManager : MonoBehaviour
    {
        [SerializeField] private float normalSize;
        [SerializeField] private float zoomedInSize;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private float zoomInDelta;
        [SerializeField] private float zoomOutDelta;
        [SerializeField] private Volume postProcessingVolume;
        [SerializeField] private float chromaticAberrationIntensity = 0.5f;
        [SerializeField] private float chromaticAberrationFadeSpeed = 3f;
    
        private float _targetSize;
        private CinemachineCamera _cinemachineCamera;
        private float _targetDelta;
        private ChromaticAberration _chromaticAberration;

        private void Start()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
            inputManager.OnAttackPressed += HandleAttack;
            inputManager.OnSweepAttackStarted += HandleSweepAttackStarted;
            inputManager.OnSweepAttackPerformed += HandleSweepAttackPerformed;
            inputManager.OnSweepAttackCanceled += HandleSweepAttackCanceled;
            
            if(postProcessingVolume.profile.TryGet(out ChromaticAberration chromaticAberration))
            {
                _chromaticAberration = chromaticAberration;
                _chromaticAberration.intensity.overrideState = true;
                _chromaticAberration.intensity.value = 0f;
            }
        }

        private void Update()
        {
            _cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(_cinemachineCamera.Lens.OrthographicSize, _targetSize, Time.deltaTime * _targetDelta);
            
            // Gradually fade out Chromatic Aberration
            if (_chromaticAberration != null && _chromaticAberration.intensity.value > 0)
            {
                _chromaticAberration.intensity.value = Mathf.Lerp(
                    _chromaticAberration.intensity.value, 0f, Time.deltaTime * chromaticAberrationFadeSpeed
                );
            }
        }

        private void HandleSweepAttackCanceled()
        {
            Debug.Log("Cinemachine: Sweep Attack Canceled");
            _targetSize = normalSize;
            _targetDelta = zoomOutDelta;
        }

        private void OnDisable()
        {
            inputManager.OnAttackPressed -= HandleAttack;
            inputManager.OnSweepAttackStarted -= HandleSweepAttackStarted;
            inputManager.OnSweepAttackPerformed -= HandleSweepAttackPerformed;
            inputManager.OnSweepAttackCanceled -= HandleSweepAttackCanceled;
        }

        private void HandleSweepAttackPerformed()
        {
            Debug.Log("Cinemachine: Sweep Attack");
            _targetSize = normalSize;
            _targetDelta = zoomOutDelta;
            
            // Apply Chromatic Aberration
            if (_chromaticAberration != null)
            {
                _chromaticAberration.intensity.value = chromaticAberrationIntensity;
            }
        }

        private void HandleSweepAttackStarted()
        {
            Debug.Log("Cinemachine: Sweep Attack Started");
            _targetSize = zoomedInSize;
            _targetDelta = zoomInDelta;
        }

        private void HandleAttack()
        {
            Debug.Log("Cinemachine: Attack");
        }
    }
}
