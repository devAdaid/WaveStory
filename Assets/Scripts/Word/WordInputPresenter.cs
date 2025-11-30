using UnityEngine;
using UnityEngine.Localization;

public class WordInputPresenter : IPresenter
{
    private RoomContext room;
    private WaveContext wave;
    private UnlockContext unlock;
    private WordInputUI ui;

    public WordInputPresenter(RoomContext room, WaveContext wave, UnlockContext unlock, WordInputUI ui)
    {
        this.room = room;
        this.wave = wave;
        this.unlock = unlock;
        this.ui = ui;
    }

    public void ProcessInput(string wordId1, string wordId2)
    {
        foreach (var soul in room.CurrentRoomData.Souls)
        {
            if (soul.WaveParameter == wave.WaveParameter
                && soul.Word1.Id == wordId1
                && soul.Word2.Id == wordId2)
            {
                if (soul.IsStaticSoul)
                {
                    continue;
                }

                if (unlock.IsUnlockedSoul(soul.Id))
                {
                    GM.I.UIHolder.AlarmUI.ShowAlarm(ui.SoulAlreadyUnlockedMessage);
                }
                else
                {
                    unlock.UnlockSoul(soul.Id);
                    AudioManager.I.PlaySfxOneShot("CorrectLoweredVolume");
                    GM.I.UIHolder.AlarmUI.ShowAlarm(ui.SoulUnlockedMessage);
                }
                return;
            }
        }

        AudioManager.I.PlaySfxOneShot("WrongLoweredVolume");

        if (CheckOtherRoomsForMatch(wordId1, wordId2))
        {
            GM.I.UIHolder.AlarmUI.ShowAlarm(ui.SoulInOtherRoomMessage);
        }
        else
        {
            GM.I.UIHolder.AlarmUI.ShowAlarm(ui.SoulNotMatchedMessage);
        }
    }

    private bool CheckOtherRoomsForMatch(string wordId1, string wordId2)
    {
        var roomMoveInteractables = Object.FindObjectsByType<RoomMoveInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

        foreach (var interactable in roomMoveInteractables)
        {
            var targetRoom = interactable.TargetRoomData;
            if (targetRoom == null || targetRoom == room.CurrentRoomData)
                continue;

            foreach (var soul in targetRoom.Souls)
            {
                if (soul.WaveParameter == wave.WaveParameter
                    && soul.Word1.Id == wordId1
                    && soul.Word2.Id == wordId2)
                {
                    if (!soul.IsStaticSoul && !unlock.IsUnlockedSoul(soul.Id))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
