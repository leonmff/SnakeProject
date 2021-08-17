using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public enum Direction { Up, Down, Right, Left }

    [SerializeField, Header("Player's Settings")]
    float _movementInterval = 0f;
    [SerializeField]
    Direction _currentDirection = Direction.Up;
    [SerializeField]
    bool _loopThroughGrid = false;
    [SerializeField]
    bool _ignoreMovingBack = false;

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
    }

    IEnumerator MovePlayer()
    {
        while (_isAlive)
        {
            Vector3 t_newPosition = Vector3.zero;

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

            yield return StartCoroutine(ValidadeCurrentPosition(t_newPosition));
            yield return new WaitForSeconds(_movementInterval);
        }
    }

    bool IsPlayerMovingBack()
    {
        if (_playerTailControl.ListTail.Count == 0)
            return false;

        bool t_movedBackHorizontal = (_previousDirection == Direction.Right && _currentDirection == Direction.Left) || (_previousDirection == Direction.Left && _currentDirection == Direction.Right);
        bool t_movedBackVertical = (_previousDirection == Direction.Up && _currentDirection == Direction.Down) || (_previousDirection == Direction.Down && _currentDirection == Direction.Up);

        return t_movedBackHorizontal || t_movedBackVertical;
    }

    IEnumerator ValidadeCurrentPosition(Vector3 pNewPosition)
    {
        Vector2 t_previousPosition = transform.position;
        pNewPosition = transform.position + pNewPosition;

        if (!_loopThroughGrid)
        {
            if (IsPlayerWithinGrid(pNewPosition))
            {
                transform.position = pNewPosition;
                _playerTailControl.MoveTail(t_previousPosition);
            }
            else
            {
                _isAlive = false;
                _playerDeath.KillPlayer();
            }
        }
        else
        {
            // Loop through grid

            if (pNewPosition.x > ((_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset))
                pNewPosition.x = 0f;
            else if (pNewPosition.x < 0f)
                pNewPosition.x = (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset;

            if (pNewPosition.y > ((_soGridConfig.GridSize.y - 1f) * _soGridConfig.GridOffset))
                pNewPosition.y = 0f;
            else if (pNewPosition.y < 0f)
                pNewPosition.y = (_soGridConfig.GridSize.y - 1f) * _soGridConfig.GridOffset;

            transform.position = pNewPosition;
            _playerTailControl.MoveTail(t_previousPosition);
        }

        yield break;
    }

    bool IsPlayerWithinGrid(Vector3 pNewPosition)
    {
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
