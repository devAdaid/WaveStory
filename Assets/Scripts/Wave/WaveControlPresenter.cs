using System.Collections.Generic;
using System.Linq;

public class WaveControlPresenter : IPresenter
{
    public WaveParameter WaveParameter => wave.WaveParameter;

    private readonly WaveContext wave;
    private readonly RoomContext room;
    private readonly UnlockContext unlock;
    private readonly WaveControlUI waveUI;

    public WaveControlPresenter(WaveContext inputWave, RoomContext room, UnlockContext unlock, WaveControlUI waveUI)
    {
        this.wave = inputWave;
        this.room = room;
        this.unlock = unlock;
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

        waveUI.Apply(wave.WaveParameter, currentFloorSouls.Where(x => !unlock.IsClearedSoul(x.Id)).ToList());
    }
}
