using System;
using UnityEngine;

public class Pickup : MonoBehaviour, IPickup
{
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider2D;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnPickup()
    {
        _spriteRenderer.enabled = false;
        _collider2D.enabled = false;
        _audioSource.Play();
    }

    public virtual void OnTriggerEnter2D(Collider2D other) { }
}

public interface IPickup
{
    void OnPickup();
}
