using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Shapes;

public class LavaPlatform : MonoBehaviour
{
    float _lowerTime = 10;
    float _maxLowerTime = 10;

    public bool safe = true;
    public bool destroyed = false;
    Rectangle _rectangle;
    Enemy _boss;

    public List<Hero> heroesInside = new List<Hero>();

    private void Awake()
    {
        
        _lowerTime = _maxLowerTime = ((Andy)BossMechanics.instance.bossController).platformlowerTime;
        _rectangle = GetComponent<Rectangle>();
    }

    private void FixedUpdate()
    {
        if (!destroyed)
        {
            _lowerTime = Mathf.Clamp(_lowerTime - (_boss ? Time.fixedDeltaTime : -Time.fixedDeltaTime), 0, _maxLowerTime);

            if (_lowerTime == 0 && _rectangle.Color.a == 1 && safe)
                DOTween.To(
                    () => _rectangle.Color,
                    x => _rectangle.Color = x,
                    new Color(_rectangle.Color.r, _rectangle.Color.g, _rectangle.Color.b, 0),
                    5).OnComplete(() => ToggleSafety(false));
            if (_lowerTime == _maxLowerTime && _rectangle.Color.a == 0 && !safe)
                DOTween.To(
                    () => _rectangle.Color,
                    x => _rectangle.Color = x,
                    new Color(_rectangle.Color.r, _rectangle.Color.g, _rectangle.Color.b, 1),
                    1).OnComplete(() => ToggleSafety(true)); ;
        }
        else
        {
            if (safe)
                DOTween.To(
                    () => _rectangle.Color,
                    x => _rectangle.Color = x,
                    new Color(_rectangle.Color.r, _rectangle.Color.g, _rectangle.Color.b, 0),
                    5).OnComplete(() => ToggleSafety(false));
        }
    }

    void ToggleSafety(bool enable)
    {
        safe = enable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (Player.instance.bosses.Contains(enemy)) _boss = enemy;
        }
        if (other.TryGetComponent(out Hero hero))
        {
            if (!heroesInside.Contains(hero)) heroesInside.Add(hero);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            _boss = null;
        }
        if (other.TryGetComponent(out Hero hero))
        {
            heroesInside.Remove(hero);
        }
    }
}
