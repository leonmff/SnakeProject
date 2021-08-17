using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

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

    CinemachineTargetGroup _tgCam;
    CinemachineVirtualCamera _vCam;

    Transform _targetGroupFirst;
    Transform _targetGroupLast;

    private void OnValidate() => FetchGridConfigAndUpdateVars();

    void FetchGridConfigAndUpdateVars()
    {
        if (!_soGridConfig)
            _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");

        if (!_tgCam)
            _tgCam = Camera.main.transform.parent.GetComponentInChildren<CinemachineTargetGroup>();

        if (!_vCam)
            _vCam = Camera.main.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>();

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

        SetCamera();
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

    void SetCamera()
    {
        _tgCam.RemoveMember(_targetGroupFirst);
        _tgCam.RemoveMember(_targetGroupLast);

        _targetGroupFirst = _listGridTiles[0].transform;
        _targetGroupLast = _listGridTiles[_listGridTiles.Count - 1].transform;

        _tgCam.AddMember(_targetGroupFirst, 1f, 0f);
        _tgCam.AddMember(_targetGroupLast, 1f, 0f);

        float t_biggestSize = _soGridConfig.GridSize.x > _soGridConfig.GridSize.y ? _soGridConfig.GridSize.x : _soGridConfig.GridSize.y;
        _vCam.m_Lens.OrthographicSize = Mathf.Lerp(_soGridConfig.OrthographicSize.x, _soGridConfig.OrthographicSize.y, t_biggestSize / _soGridConfig.MaxSize);
    }
}
