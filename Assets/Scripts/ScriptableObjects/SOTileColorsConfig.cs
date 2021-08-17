using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileColorsConfig", menuName = "Tiles/Tile Color Config")]
public class SOTileColorsConfig : ScriptableObject
{
    [ColorUsage(true, true)]
    public Color White = Color.white;
    [ColorUsage(true, true)]
    public Color Red = Color.white;
    [ColorUsage(true, true)]
    public Color Blue = Color.white;
    [ColorUsage(true, true)]
    public Color Green = Color.white;
    [ColorUsage(true, true)]
    public Color Black = Color.white;
    [ColorUsage(true, true)]
    public Color Magenta = Color.white;
}
