using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPos;
    public static CameraShake _instance;
    public Tilemap board;
    public int height, width;

    void Awake()
    {
        Vector3 cellSize = board.cellSize;
        Camera.main.orthographicSize = cellSize.y * height / 2;

        Transform tmp = Camera.main.GetComponent<Transform>();
        tmp.position = new Vector3(width * cellSize.x / 2f, height * cellSize.y / 2f, -10);

        _originalPos = transform.localPosition;
        _instance = this;
    }

    public static void Shake(float duration, float amount)
    {
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.cShake(duration, amount));
    }

    public IEnumerator cShake(float duration, float amount)
    {
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

            duration -= Time.deltaTime;

            yield return null;
        }

        transform.localPosition = _originalPos;
    }
}
