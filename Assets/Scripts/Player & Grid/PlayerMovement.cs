using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public enum Direction { Up, Down, Right, Left }

    [SerializeField, Header("Player's Settings")]
    float _movementInterval = 0f;
    public float MovementInterval { get => _movementInterval; }

    [SerializeField, Range(0.001f, 0.999f)]
    float _intervalMultiplierPerFood = 0f;
    [SerializeField]
    Direction _currentDirection = Direction.Up;
    
    [SerializeField, Space(15)]
    bool _loopThroughGrid = false;
    [SerializeField]
    bool _ignoreMovingBack = false;
    [SerializeField]
    bool _moveByBlock = true;

    SOTileColorsConfig _soTileColorsConfig;
    SOGridConfig _soGridConfig;
    PlayerTailController _playerTailControl;
    PlayerDeath _playerDeath;

    SpriteRenderer _sr;
    Direction _previousDirection;
    bool _isAlive;

    private void Awake()
    {
        _soTileColorsConfig = Resources.Load<SOTileColorsConfig>("TileColorsConfig");
        _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");
        _playerTailControl = GetComponent<PlayerTailController>();
        _playerDeath = GetComponent<PlayerDeath>();

        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _sr.color = _soTileColorsConfig.Green;
        _isAlive = true;
        StartCoroutine(MovePlayer());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("TailWall"))
        {
            _isAlive = false;
            _playerDeath.KillPlayer();
        }
    }

    private void Update()
    {
        GetDirection();
    }

    public void IncreaseSpeed() => _movementInterval *= _intervalMultiplierPerFood;

    IEnumerator MovePlayer()
    {
        while (_isAlive)
        {
            Vector3 t_newPosition = Vector3.zero;

            if (IsPlayerMovingBack())
            {
                if (_ignoreMovingBack)
                    _currentDirection = _previousDirection;
                else
                {
                    _isAlive = false;
                    _playerDeath.KillPlayer();
                }
            }

            switch (_currentDirection)
            {
                case Direction.Up:
                    t_newPosition = new Vector3(0f, _soGridConfig.GridOffset);
                    break;
                case Direction.Down:
                    t_newPosition = new Vector3(0f, -_soGridConfig.GridOffset);
                    break;
                case Direction.Right:
                    t_newPosition = new Vector3(_soGridConfig.GridOffset, 0f);
                    break;
                case Direction.Left:
                    t_newPosition = new Vector3(-_soGridConfig.GridOffset, 0f);
                    break;
            }

            Vector2 t_previousPosition = transform.position;
            bool t_outOfGrid = false;
            if (!ValidadeCurrentPosition(ref t_newPosition, ref t_outOfGrid))
            {
                _isAlive = false;
                _playerDeath.KillPlayer();
                yield break;
            }

            //Debug.Log($"<size=22><color=mangeta>_moveByBlock {_moveByBlock} | t_outOfGrid {t_outOfGrid}</color></size>");
            if (_moveByBlock || t_outOfGrid)
            {
                //Debug.Log($"<size=22><color=lime>By Block</color></size>");

                transform.position = t_newPosition;
                _playerTailControl.MoveTail(t_previousPosition);
                yield return new WaitForSeconds(_movementInterval);
            }
            else
            {
                //Debug.Log($"<size=22><color=aqua>Smooth</color></size>");

                _playerTailControl.MoveTail(t_previousPosition, true);
                yield return StartCoroutine(MoveSmooth(t_newPosition));
            }
        }
    }

    IEnumerator MoveSmooth(Vector2 pFinalPosition)
    {
        Vector2 t_initialPosition = transform.position;
        float t_elapsedTime = 0f;

        while (t_elapsedTime < _movementInterval)
        {         
            transform.position = Vector2.Lerp(t_initialPosition, pFinalPosition, t_elapsedTime / _movementInterval);
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = pFinalPosition;
    }

    bool IsPlayerMovingBack()
    {
        if (_playerTailControl.ListTail.Count == 0)
            return false;

        bool t_movedBackHorizontal = (_previousDirection == Direction.Right && _currentDirection == Direction.Left) || (_previousDirection == Direction.Left && _currentDirection == Direction.Right);
        bool t_movedBackVertical = (_previousDirection == Direction.Up && _currentDirection == Direction.Down) || (_previousDirection == Direction.Down && _currentDirection == Direction.Up);

        return t_movedBackHorizontal || t_movedBackVertical;
    }

    bool ValidadeCurrentPosition(ref Vector3 refNewPositionOffset, ref bool refOutOfGrid)
    {
        bool t_isPlayerWithinGrid = IsPlayerWithinGrid(transform.position + refNewPositionOffset);

        refOutOfGrid = !t_isPlayerWithinGrid;
        refNewPositionOffset = transform.position + refNewPositionOffset;

        if (!_loopThroughGrid)
            return t_isPlayerWithinGrid;
        else
        {
            float t_margin = 0.01f;

            if (refNewPositionOffset.x > ((_soGridConfig.GridSize.x - 1) * _soGridConfig.GridOffset) + t_margin)
                refNewPositionOffset.x = 0f;
            else if (refNewPositionOffset.x < -t_margin)
                refNewPositionOffset.x = (_soGridConfig.GridSize.x - 1) * _soGridConfig.GridOffset;

            if (refNewPositionOffset.y > ((_soGridConfig.GridSize.y - 1) * _soGridConfig.GridOffset) + t_margin)
                refNewPositionOffset.y = 0f;
            else if (refNewPositionOffset.y < -t_margin)
                refNewPositionOffset.y = (_soGridConfig.GridSize.y - 1) * _soGridConfig.GridOffset;

            return true;
        }
    }

    bool IsPlayerWithinGrid(Vector3 pNewPosition)
    {
        //Debug.Log($"<size=22><color=white>New Position: {pNewPosition}</color></size>");

        bool t_withinBoundrieX = (pNewPosition.x >= 0f && pNewPosition.x <= (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset);
        bool t_withinBoundrieY = (pNewPosition.y >= 0f && pNewPosition.y <= (_soGridConfig.GridSize.y - 1f) * _soGridConfig.GridOffset);

        return t_withinBoundrieX && t_withinBoundrieY;
    }

    void GetDirection()
    {
        _previousDirection = _currentDirection;

        if (Input.GetKeyDown(KeyCode.W))
            _currentDirection = Direction.Up;
        else if (Input.GetKeyDown(KeyCode.S))
            _currentDirection = Direction.Down;
        else if (Input.GetKeyDown(KeyCode.D))
            _currentDirection = Direction.Right;
        else if (Input.GetKeyDown(KeyCode.A))
            _currentDirection = Direction.Left;
    }
}
