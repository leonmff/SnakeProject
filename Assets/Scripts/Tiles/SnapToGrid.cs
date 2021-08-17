using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour
{
    SOGridConfig _soGridConfig;

    private void Update()
    {
        if (!Application.isPlaying && transform.hasChanged)
        {
            FetchGridConfig();
            Snap();
        }
    }

    void Snap()
    {
        Vector3 t_newPosition = new Vector3
        (
            Mathf.Round(transform.position.x / _soGridConfig.GridOffset) * _soGridConfig.GridOffset,
            Mathf.Round(transform.position.y / _soGridConfig.GridOffset) * _soGridConfig.GridOffset
        );

        t_newPosition = new Vector3
        (
            Mathf.Clamp(t_newPosition.x, 0f, (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset),
            Mathf.Clamp(t_newPosition.y, 0f, (_soGridConfig.GridSize.y - 1f) * _soGridConfig.GridOffset)
        );

        transform.position = t_newPosition;
    }

    void FetchGridConfig()
    {
        if (!_soGridConfig)
            _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");
    }
}
