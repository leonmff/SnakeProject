using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "Tiles/Grid Config")]
public class SOGridConfig : ScriptableObject
{
    [Header("Grid Settings")]
    public Vector2 GridSize;
    public float GridOffset;
    public float TileSize;

    [Space(7)]
    public int MaxSize;
    public int MinSize = 2;

    [Header("Camera Settings")]
    public Vector2 OrthographicSize;

    private void OnValidate()
    {
        GridSize = new Vector2(Mathf.Clamp(GridSize.x, MinSize, MaxSize), Mathf.Clamp(GridSize.y, MinSize, MaxSize));
    }
}
