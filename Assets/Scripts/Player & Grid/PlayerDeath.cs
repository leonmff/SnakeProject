using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField]
    float _sizeMultiplier = 0f;
    [SerializeField]
    float _durationFadeOut = 0f;
    [SerializeField]
    float _delayBetweenTiles = 0f;

    SOTileColorsConfig _soTileColorConfig;
    PlayerTailController _playerTailControl;

    SpriteRenderer _sr;

    public static UnityAction OnPlayerDeath;

    private void Awake()
    {
        _soTileColorConfig = Resources.Load<SOTileColorsConfig>("TileColorsConfig");
        _playerTailControl = GetComponent<PlayerTailController>();

        _sr = GetComponent<SpriteRenderer>();
    }

    public void KillPlayer() => StartCoroutine(CoKillPlayer());

    IEnumerator CoKillPlayer()
    {
        StartCoroutine(PlayerWaveSizeIncrease(_delayBetweenTiles));

        StartCoroutine(PlayerWaveColorChange(_soTileColorConfig.Black, _delayBetweenTiles));
        yield return new WaitForSeconds(_durationFadeOut);
        yield return StartCoroutine(PlayerWaveColorChange(Color.clear, _delayBetweenTiles * 1.5f));

        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(0);
    }

    IEnumerator PlayerWaveSizeIncrease(float pDelay)
    {
        StartCoroutine(IncreaseSize(transform, _sizeMultiplier, _durationFadeOut));
        yield return new WaitForSeconds(pDelay);

        for (int index = 0; index < _playerTailControl.ListTail.Count; index++)
        {
            StartCoroutine(IncreaseSize(_playerTailControl.ListTail[index], _sizeMultiplier, _durationFadeOut));
            yield return new WaitForSeconds(pDelay);
        }
    }

    IEnumerator PlayerWaveColorChange(Color pTargetColor, float pDelay)
    {
        StartCoroutine(LerpColor(_sr, pTargetColor, _durationFadeOut));
        yield return new WaitForSeconds(pDelay);

        for (int index = 0; index < _playerTailControl.ListTail.Count; index++)
        {
            SpriteRenderer t_spriteRenderer = _playerTailControl.ListTail[index].GetComponent<SpriteRenderer>();
            StartCoroutine(LerpColor(t_spriteRenderer, pTargetColor, _durationFadeOut));
            yield return new WaitForSeconds(pDelay);
        }
    }

    IEnumerator LerpColor(SpriteRenderer pSpriteRenderer, Color pFinalColor, float pDuration)
    {
        Color t_initialColor = pSpriteRenderer.color;
        float t_elapsedTime = 0f;

        while (t_elapsedTime < pDuration)
        {
            pSpriteRenderer.color = Color.Lerp(t_initialColor, pFinalColor, t_elapsedTime / pDuration);
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }

        pSpriteRenderer.color = pFinalColor;
    }

    IEnumerator IncreaseSize(Transform pTransform, float pIncreaseMultiplier, float pDuration)
    {
        Vector2 t_initialScale = pTransform.localScale;
        Vector2 t_finalScale = t_initialScale * pIncreaseMultiplier;
        float t_elapsedTime = 0f;

        while (t_elapsedTime < pDuration)
        {
            pTransform.localScale = Vector2.Lerp(t_initialScale, t_finalScale, t_elapsedTime / pDuration);
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
