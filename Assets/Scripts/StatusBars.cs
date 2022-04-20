using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class StatusBars : MonoBehaviour
{
    public Line healthBar;
    public Line manaBar;
    public Line castBar;

    public Line healthBackground;
    public Line manaBackground;
    public Line castBackground;

    float _lineSize;

    public void Initiate(float x, float y)
    {
        _lineSize = x;

        float thickness = healthBackground.Thickness;
        x = x / 2;
        y = y / 2 + 3 * thickness;

        healthBar.Start = new Vector3(-x, y, 0);
        healthBackground.Start = new Vector3(-x, y, 0);
        healthBar.End = new Vector3(x, y, 0);
        healthBackground.End = new Vector3(x, y, 0);

        manaBar.Start = new Vector3(-x, y - thickness, 0);
        manaBackground.Start = new Vector3(-x, y - thickness, 0);
        manaBar.End = new Vector3(x, y - thickness, 0);
        manaBackground.End = new Vector3(x, y - thickness, 0);

        castBar.Start = new Vector3(-x, y - thickness * 2, 0);
        castBackground.Start = new Vector3(-x, y - thickness * 2, 0);
        castBar.End = new Vector3(x, y - thickness * 2, 0);
        castBackground.End = new Vector3(x, y - thickness * 2, 0);
    }

    public void UpdateHealthBar(float size)
    {
        healthBar.End = healthBar.Start + Vector3.right * size * _lineSize;
    }
    public void UpdateManaBar(float size)
    {
        manaBar.End = manaBar.Start + Vector3.right * size * _lineSize;
    }
    public void UpdateCastBar(float size)
    {
        castBar.End = castBar.Start + Vector3.right * size * _lineSize;
    }

}
