using UnityEngine;

[CreateAssetMenu(fileName = "SoulData", menuName = "Scriptable Objects/SoulData")]
public class SoulData : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public WaveParameter WaveParameter;
    public string WordId1;
    public string WordId2;
}
