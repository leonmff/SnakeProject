using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TailColor : MonoBehaviour
{
    public enum TailType { Red, Green, Black }

    [SerializeField]
    TailType _type = TailType.Red;
    

    SOTileColorsConfig _soTileColorsConfig;
    SpriteRenderer _sr;

    private void OnValidate()
    {
        if (!_sr)
            _sr = GetComponent<SpriteRenderer>();

        if (!_soTileColorsConfig)
            _soTileColorsConfig = Resources.Load<SOTileColorsConfig>("TileColorsConfig");

        ChangeColor(_type);
    }

    void ChangeColor(TailType pType)
    {
        switch (pType)
        {
            case TailType.Red:
                _sr.color = _soTileColorsConfig.Red;
                break;
            case TailType.Green:
                _sr.color = _soTileColorsConfig.Green;
                break;
            case TailType.Black:
                _sr.color = _soTileColorsConfig.Black;
                break;
        }
    }
}
