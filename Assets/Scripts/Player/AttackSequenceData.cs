using UnityEngine;

[CreateAssetMenu( fileName = "New Attack Sequence", menuName = "Attack Sequence" )]
public class AttackSequenceData : ScriptableObject
{
    public AttackType attackType;
    [Range(0,1)]
    public float staminaCost;
    public float damage;
}