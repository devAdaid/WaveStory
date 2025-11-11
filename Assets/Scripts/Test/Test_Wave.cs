using UnityEngine;

public class Test_Wave : MonoBehaviour
{
    public WaveRenderer WR;
    public WaveController WC;

    private void Start()
    {
        WC = new WaveController(WR);
    }
}
