using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Scriptable Objects/BaseStats")]
public class BaseStats : ScriptableObject
{
    [SerializeField] private int primaryIntStat;
    public int PrimaryIntStat => primaryIntStat;
    [SerializeField] private int secondaryIntStat;
    public int SecondaryIntStat => secondaryIntStat;
    [SerializeField] private float primaryFloatStat;
    public float PrimaryFloatStat => primaryFloatStat;
    [SerializeField] private float secondaryFloatStat;
    public float SecondaryFloatStat => secondaryFloatStat;
}
