using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwapper : MonoBehaviour
{
    [SerializeField] Sprite m_onSprite;
    [SerializeField] Sprite m_offSprite;
    [SerializeField] Image m_targetGraphic;

    public void SwapSprites(bool isOn)
    {
        if (isOn)
        {
            m_targetGraphic.sprite = m_onSprite;
        }
        else
        {
            m_targetGraphic.sprite = m_offSprite;
        }
    }
}
