using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlarmUI : UIBase
{
    [SerializeField]
    private CanvasGroup alarmGroup;
    [SerializeField]
    private TMP_Text alarmText;
    [SerializeField]
    private Button alarmButton;
    [SerializeField]
    private float yMove = 50f;
    [SerializeField]
    private float fadeInDuration = 0.3f;
    [SerializeField]
    private float stayDuration = 1.5f;
    [SerializeField]
    private float fadeOutDuration = 0.3f;

    private Vector3 initialPosition;
    private Coroutine currentAlarmCoroutine;

    protected override void InitializeInternal()
    {
        initialPosition = alarmGroup.transform.localPosition;
        alarmGroup.alpha = 0f;
        alarmButton.onClick.AddListener(OnClick);
        Hide();
    }

    public void ShowAlarm(string text)
    {
        if (currentAlarmCoroutine != null)
        {
            StopCoroutine(currentAlarmCoroutine);
        }

        currentAlarmCoroutine = StartCoroutine(AlarmAnimation(text));
    }

    public void OnClick()
    {
        if (currentAlarmCoroutine != null)
        {
            StopCoroutine(currentAlarmCoroutine);
        }

        currentAlarmCoroutine = null;

        Hide();
    }

    private IEnumerator AlarmAnimation(string text)
    {
        Show();
        alarmText.text = text;
        alarmGroup.transform.localPosition = initialPosition;

        // 페이드 인 + 위로 이동
        float elapsed = 0f;
        Vector3 startPos = initialPosition;
        Vector3 endPos = initialPosition + Vector3.up * yMove;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;

            alarmGroup.alpha = Mathf.Lerp(0f, 1f, t);
            alarmGroup.transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        alarmGroup.alpha = 1f;
        alarmGroup.transform.localPosition = endPos;

        // 대기
        yield return new WaitForSeconds(stayDuration);

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;

            alarmGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        alarmGroup.alpha = 0f;
        currentAlarmCoroutine = null;
        Hide();
    }
}