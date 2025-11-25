using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomData : ScriptableObject
{
    public string Id => name;
    public LocalizedString DisplayName;
    public int Floor;
    public Sprite RealSprite;
    public Sprite SoulSprite;
    public List<SoulData> Souls;
}
