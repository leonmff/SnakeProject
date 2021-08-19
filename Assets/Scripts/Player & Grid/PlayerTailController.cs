using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTailController : MonoBehaviour
{
    [SerializeField]
    GameObject _prefabTail = null;

    [SerializeField, InspectorReadOnly, Space(15)]
    List<Transform> _listTail;
    public List<Transform> ListTail { get => _listTail; }

    SOTileColorsConfig _soTileColorsConfig;
    PlayerMovement _playerMovement;

    Vector2 _tailEndPreviousPosition;

    private void Awake()
    {
        _soTileColorsConfig = Resources.Load<SOTileColorsConfig>("TileColorsConfig");
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _listTail = new List<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TailFood"))
        {
            IInteractable t_interactable = collision.GetComponent<IInteractable>();
            if (t_interactable != null)
                t_interactable.Interact();

            collision.gameObject.SetActive(false);
            AddTail();
            _playerMovement.IncreaseSpeed();
        }
    }

    void AddTail()
    {
        GameObject t_newTile = Instantiate(_prefabTail, transform.parent);
        SpriteRenderer t_sr = t_newTile.GetComponent<SpriteRenderer>();
        if (t_sr)
            t_sr.color = _soTileColorsConfig.Green;

        _listTail.Add(t_newTile.transform);
        t_newTile.transform.position = _tailEndPreviousPosition;
    }

    public void MoveTail(Vector2 pPlayersPreviousPosition, bool pSmooth = false) => StartCoroutine(MoveTails(pPlayersPreviousPosition, pSmooth));

    IEnumerator MoveTails(Vector2 pPlayersPreviousPosition, bool pSmooth)
    {
        UpdateTailEndPosition(pPlayersPreviousPosition);

        Vector2 t_previousMovedTail = pPlayersPreviousPosition;
        Transform t_currentTail;

        for (int index = 0; index < _listTail.Count; index++)
        {
            t_currentTail = _listTail[index];
            Vector3 t_newPosition = t_previousMovedTail;
            t_previousMovedTail = t_currentTail.position;

            if (pSmooth)
                StartCoroutine(MoveLerp(t_currentTail, t_newPosition));
            else
            {
                t_currentTail.position = t_newPosition;
                yield return null;
            }
        }

        yield break;
    }

    IEnumerator MoveLerp(Transform pTail, Vector2 pFinalPosition)
    {
        Vector2 t_initialPosition = pTail.position;
        float t_elapsedTime = 0f + (_playerMovement.MovementInterval * 0.04f);
        
        while (t_elapsedTime < _playerMovement.MovementInterval)
        {
            pTail.position = Vector2.Lerp(t_initialPosition, pFinalPosition, t_elapsedTime / _playerMovement.MovementInterval);
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = pFinalPosition;
    }

    void UpdateTailEndPosition(Vector2 pPlayersPreviousPosition)
    {
        if (_listTail.Count == 0)
            _tailEndPreviousPosition = pPlayersPreviousPosition;
        else
            _tailEndPreviousPosition = _listTail[_listTail.Count - 1].position;
    }
}
