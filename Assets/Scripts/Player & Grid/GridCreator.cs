using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridCreator : MonoBehaviour
{
    [SerializeField, Header("Default Grid Tile")]
    GameObject _prefabGridTile = null;

    [SerializeField, InspectorReadOnly, Header("Grid Settings"), Tooltip("Edit this file to change the configurations")]
    SOGridConfig _soGridConfig;
    [SerializeField, InspectorReadOnly, Tooltip("Informations gathered from the GridConfig scriptable object")]
    Vector2 _gridSize = Vector2.zero;
    [SerializeField, InspectorReadOnly, Tooltip("Informations gathered from the GridConfig scriptable object")]
    float _gridOffset = 0f;
    [SerializeField, InspectorReadOnly, Tooltip("Informations gathered from the GridConfig scriptable object")]
    float _gridTileSize = 0f;

    [SerializeField, InspectorReadOnly, Space(15)]
    List<GameObject> _listGridTiles;

    private void OnValidate() => FetchGridConfigAndUpdateVars();

    void FetchGridConfigAndUpdateVars()
    {
        if (!_soGridConfig)
            _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");

        _gridSize = _soGridConfig.GridSize;
        _gridOffset = _soGridConfig.GridOffset;
        _gridTileSize = _soGridConfig.TileSize;
    }

    public void CreateGrid()
    {
        FetchGridConfigAndUpdateVars();

        _listGridTiles = new List<GameObject>();

        for (int colunm = 0; colunm < _soGridConfig.GridSize.x; colunm++)
        {
            for (int line = 0; line < _soGridConfig.GridSize.y; line++)
            {
                GameObject t_gridTile = InstanceGridTile(new Vector2(colunm * _soGridConfig.GridOffset, line * _soGridConfig.GridOffset));
                _listGridTiles.Add(t_gridTile);
            }
        }       
    }

    public void RecreateGrid()
    {
        DeleteGrid();
        CreateGrid();
    }

    public void DeleteGrid()
    {
        for (int index = 0; index < _listGridTiles.Count; index++)
        {
#if UNITY_EDITOR
            DestroyImmediate(_listGridTiles[index]);
#endif
        }

        _listGridTiles.Clear();
    }

    GameObject InstanceGridTile(Vector2 pTilePosition)
    {
        GameObject t_gridTile = (GameObject)PrefabUtility.InstantiatePrefab(_prefabGridTile, transform); 
        t_gridTile.transform.position = pTilePosition;
        return t_gridTile;
    }
}
