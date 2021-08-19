using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject _prefabTileFood = null;

    SOGridConfig _soGridConfig;

    List<Vector2> _listAvailablePositions;

    private void Awake() => _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");

    private void OnEnable() => TailPlayersFood.OnFoodTaken += SpawnFood;

    private void OnDisable() => TailPlayersFood.OnFoodTaken -= SpawnFood;

    private void Start()
    {
        FillInAvailablePositions();
        SpawnFood();
    }

    void FillInAvailablePositions()
    {
        _listAvailablePositions = new List<Vector2>();

        for (int column = 0; column < _soGridConfig.GridSize.x; column++)
        {
            for (int line = 0; line < _soGridConfig.GridSize.y; line++)
            {
                _listAvailablePositions.Add(new Vector2(column * _soGridConfig.GridOffset, line * _soGridConfig.GridOffset));
            }
        }
    }

    void SpawnFood()
    {
        Transform t_tile = Instantiate(_prefabTileFood, transform).transform;
        t_tile.GetComponent<TailPlayersFood>().SetFoodSpawner(this);

        while (true)
        {
            Vector2 t_newPosition = _listAvailablePositions[Random.Range(0, _listAvailablePositions.Count)];
            Collider2D t_collider = Physics2D.OverlapCircle(t_newPosition, _soGridConfig.GridOffset / 2f, LayerMask.GetMask("Default"));
            if (t_collider)
                _listAvailablePositions.RemoveAt(_listAvailablePositions.IndexOf(t_newPosition));
            else
            {
                t_tile.position = t_newPosition;
                break;
            }
        }
    }

    public void AddAvailablePosition(Vector2 pNewPosition) => _listAvailablePositions.Add(pNewPosition);
}
