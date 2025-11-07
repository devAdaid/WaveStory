using System.Collections.Generic;
using UnityEngine;

public enum WaveType
{
    Sin,
    Square,
    PingPong,

    Count,
}

public class WaveRenderer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private float lineWidth = 0.05f;

    [SerializeField]
    private float width = 2 * Mathf.PI;

    [SerializeField]
    [Tooltip("파동 하나당 포인트 수 (높을수록 부드러움)")]
    private int pointsPerWave = 50;

    [Header("Wave Parameters")]
    [SerializeField]
    [Tooltip("파형의 진폭 (높이)")]
    private float amplitude = 1f;

    [SerializeField]
    [Tooltip("파형의 주파수 (파동의 개수)")]
    private float frequency = 1f;

    [SerializeField]
    [Tooltip("파형의 이동 속도 배율 (frequency와 곱해져서 실제 속도가 됨)")]
    private float speedMultiplier = 1f;

    [SerializeField]
    private WaveType waveType = WaveType.Sin;

    [SerializeField]
    private int pointCount;
    private List<Vector3> linePoints = new List<Vector3>();

    private bool isPause = false;
    private float pausedTime = 0f;
    private float timeOffset = 0f;

    // 실제 파동 속도 계산 (frequency * speedMultiplier)
    private float ActualSpeed => frequency * speedMultiplier;

    // 현재 시간 (일시정지 상태 고려)
    private float CurrentTime => isPause ? pausedTime : (Time.timeSinceLevelLoad - timeOffset);

    public void SetPause(bool pause)
    {
        // 같은 상태면 무시
        if (isPause == pause) return;

        if (pause)
        {
            // 일시정지 시작: 현재 유효 시간 저장
            pausedTime = Time.timeSinceLevelLoad - timeOffset;
            isPause = true;
        }
        else
        {
            // 일시정지 해제: 멈춰있던 시간만큼 오프셋 증가
            timeOffset = Time.timeSinceLevelLoad - pausedTime;
            isPause = false;
        }
    }

    public void SetParameter(WaveType type, float amp, float freq)
    {
        waveType = type;
        amplitude = amp;
        frequency = freq;
    }

    private void Awake()
    {
        var totalWaves = (width / (2f * Mathf.PI)) * frequency;
        var calculatedPoints = Mathf.CeilToInt(totalWaves * pointsPerWave);
        pointCount = Mathf.Max(calculatedPoints, 2);
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    private void Update()
    {
        DrawWave();
    }

    private void DrawWave()
    {
        var xStart = -width / 2f + transform.position.x;
        var xEnd = width / 2f + transform.position.x;
        var yDiff = transform.position.y;

        if (waveType == WaveType.Sin)
        {
            DrawSinWave(xStart, xEnd, yDiff);
        }
        else if (waveType == WaveType.Square)
        {
            DrawSquareWave(xStart, xEnd, yDiff);
        }
        else if (waveType == WaveType.PingPong)
        {
            DrawPingPongWave(xStart, xEnd, yDiff);
        }
    }

    private void DrawSinWave(float xStart, float xEnd, float yDiff)
    {
        lineRenderer.positionCount = pointCount;

        for (var currentPoint = 0; currentPoint < pointCount; currentPoint++)
        {
            var progress = (float)currentPoint / (pointCount - 1);
            var x = Mathf.Lerp(xStart, xEnd, progress);
            var y = yDiff + WaveLogic.GetWaveY(WaveType.Sin, x, amplitude, frequency, ActualSpeed, CurrentTime);
            lineRenderer.SetPosition(currentPoint, new Vector3(x, y, 0f));
        }
    }

    private void DrawSquareWave(float xStart, float xEnd, float yDiff)
    {
        linePoints.Clear();
        var time = CurrentTime;
        var actualSpeed = ActualSpeed;

        // 시작점
        var startY = yDiff + WaveLogic.GetWaveY(WaveType.Square, xStart, amplitude, frequency, actualSpeed, time);
        linePoints.Add(new Vector3(xStart, startY, 0f));

        // Square 파형은 sin이 0을 교차할 때 변경됨
        // sin(frequency * x + actualSpeed * time) = 0
        // frequency * x + actualSpeed * time = n * PI
        // x = (n * PI - actualSpeed * time) / frequency

        var phase = actualSpeed * time;
        var period = Mathf.PI / frequency; // 반주기

        // 첫 번째 전환점 찾기
        var firstTransition = Mathf.Ceil((frequency * xStart + phase) / Mathf.PI);
        var numTransitions = Mathf.CeilToInt((frequency * xEnd + phase) / Mathf.PI) - firstTransition + 1;

        for (int i = 0; i < numTransitions; i++)
        {
            var n = firstTransition + i;
            var transitionX = (n * Mathf.PI - phase) / frequency;

            if (transitionX > xStart && transitionX < xEnd)
            {
                // 전환 직전 점 (현재 y값 유지)
                var currentY = yDiff + WaveLogic.GetWaveY(WaveType.Square, transitionX - 0.001f, amplitude, frequency, actualSpeed, time);
                linePoints.Add(new Vector3(transitionX, currentY, 0f));

                // 전환 직후 점 (새로운 y값)
                var newY = yDiff + WaveLogic.GetWaveY(WaveType.Square, transitionX + 0.001f, amplitude, frequency, actualSpeed, time);
                linePoints.Add(new Vector3(transitionX, newY, 0f));
            }
        }

        // 끝점
        var endY = yDiff + WaveLogic.GetWaveY(WaveType.Square, xEnd, amplitude, frequency, actualSpeed, time);
        linePoints.Add(new Vector3(xEnd, endY, 0f));

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }

    private void DrawPingPongWave(float xStart, float xEnd, float yDiff)
    {
        linePoints.Clear();
        var time = CurrentTime;
        var actualSpeed = ActualSpeed;

        // 시작점
        var startY = yDiff + WaveLogic.GetWaveY(WaveType.PingPong, xStart, amplitude, frequency, actualSpeed, time);
        linePoints.Add(new Vector3(xStart, startY, 0f));

        // PingPong은 t = n (정수)일 때 꼭짓점
        // t = (frequency * x + actualSpeed * time) / PI = n
        // x = (n * PI - actualSpeed * time) / frequency

        var phase = actualSpeed * time;

        // 첫 번째 꼭짓점 찾기
        var firstPeak = Mathf.Ceil((frequency * xStart + phase) / Mathf.PI);
        var numPeaks = Mathf.CeilToInt((frequency * xEnd + phase) / Mathf.PI) - firstPeak + 1;

        for (int i = 0; i < numPeaks; i++)
        {
            var n = firstPeak + i;
            var peakX = (n * Mathf.PI - phase) / frequency;

            if (peakX > xStart && peakX < xEnd)
            {
                var y = yDiff + WaveLogic.GetWaveY(WaveType.PingPong, peakX, amplitude, frequency, actualSpeed, time);
                linePoints.Add(new Vector3(peakX, y, 0f));
            }
        }

        // 끝점
        var endY = yDiff + WaveLogic.GetWaveY(WaveType.PingPong, xEnd, amplitude, frequency, actualSpeed, time);
        linePoints.Add(new Vector3(xEnd, endY, 0f));

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }
}