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
}
