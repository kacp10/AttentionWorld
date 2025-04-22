using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardType;
    public SpriteRenderer front;
    public SpriteRenderer back;

    private bool isFlipped = false;
    private bool isMatched = false;

    public void Init(int type, Sprite frontSprite)
    {
        cardType = type;
        front.sprite = frontSprite;
        Hide();
    }

    public void Show()
    {
        if (isMatched) return;

        front.gameObject.SetActive(true);
        back.gameObject.SetActive(false);
        isFlipped = true;
    }

    public void Hide()
    {
        if (isMatched) return;

        front.gameObject.SetActive(false);
        back.gameObject.SetActive(true);
        isFlipped = false;
    }

    public void MarkAsMatched()
    {
        isMatched = true;
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public bool IsMatched()
    {
        return isMatched;
    }

    void OnMouseDown()
    {
        if (!GameManager.Instance.CanFlipCard(this)) return;
        Show();
        GameManager.Instance.OnCardFlipped(this);
    }
}

