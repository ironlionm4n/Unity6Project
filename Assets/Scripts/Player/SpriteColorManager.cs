using UnityEngine;

public class SpriteColorManager : MonoBehaviour
{
    [SerializeField] private Color emptyStaminaColor;
    [SerializeField] private Color fullStaminaColor;
    
    private PlayerAttack _playerAttack;
    private SpriteRenderer _spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerAttack = GetComponentInParent<PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerAttack == null) return;
        
        var ratio = _playerAttack.GetRatioOfMaxStamina();
        _spriteRenderer.color = Color.Lerp(emptyStaminaColor, fullStaminaColor, ratio);     
    }
}
