using System.Collections.Generic;

public class WaveControlPresenter : IPresenter
{
    public WaveParameter WaveParameter => wave.WaveParameter;

    private readonly WaveContext wave;
    private readonly RoomContext room;
    private readonly WaveControlUI waveUI;

    public WaveControlPresenter(WaveContext inputWave, RoomContext room, WaveControlUI waveUI)
    {
        this.wave = inputWave;
        this.room = room;
        this.waveUI = waveUI;
        inputWave.WaveChanged.AddListener(UpdateUI);
    }

    public void SetParamter(WaveParameter param)
    {
        wave.SetParameter(param);
    }

    public void UpdateUI()
    {
        var currentFloor = room.CurrentRoomData.Floor;
        var currentFloorSouls =  StaticDataHolder.I.GetSoulsInFloor(currentFloor);

        waveUI.Apply(wave.WaveParameter, currentFloorSouls);
    }
}
