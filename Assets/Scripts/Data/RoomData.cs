using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomData : ScriptableObject
{
    public string Id => name;
    public string DisplayName;
    public Sprite RealSprite;
    public Sprite SoulSprite;
    public List<SoulData> Souls;
}
