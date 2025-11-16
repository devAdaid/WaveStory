using UnityEngine;

public class RoomControl : MonoBehaviour
{
    [SerializeField]
    public string RoomId;

    [SerializeField]
    private GameObject realLayer;

    [SerializeField]
    private GameObject soulLayer;

    public void SetSoulMode(bool isSoulMode)
    {
        realLayer.SetActive(!isSoulMode);
        soulLayer.SetActive(isSoulMode);
    }
}
