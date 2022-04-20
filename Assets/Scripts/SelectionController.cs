using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class SelectionController : MonoBehaviour
{
    [Range(0, 1)]
    public float selectionAlpha = .25f;
    public LayerMask characterLayer;

    [SerializeField] List<Hero> _selectedUnits;
    Vector2 _rectStartPoint;
    Polygon _selectionPolygon;

    public KeyCode[] controlGroupKeys = new KeyCode[10];
    [SerializeField] List<Hero>[] _controlGroups;

    private void Awake()
    {
        _controlGroups = new List<Hero>[10];
        for (int i = 0; i < 10; i++) _controlGroups[i] = new List<Hero>();
        _selectionPolygon = GetComponent<Polygon>();
    }

    private void Start()
    {
        ShowSelectionBox(false);
    }

    private void Update()
    {
        UnitSelection();
        ControlGroups();
    }

    void UnitSelection()
    {
        Vector2 mousePosWorld = Mouse.worldPosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (!Input.GetKey(KeyCode.LeftShift)) _selectedUnits.Clear();
            //get rect start point
            _rectStartPoint = mousePosWorld;
        }
        if (Input.GetMouseButton(0))
        {
            //draw rect
            if (mousePosWorld.x != _rectStartPoint.x && mousePosWorld.y != _rectStartPoint.y) 
            {
                ShowSelectionBox(true);
                _selectionPolygon.SetPointPosition(0, _rectStartPoint);                                   //bottom left
                _selectionPolygon.SetPointPosition(1, new Vector2(mousePosWorld.x, _rectStartPoint.y));   //bottom left
                _selectionPolygon.SetPointPosition(2, mousePosWorld);                                    //bottom left
                _selectionPolygon.SetPointPosition(3, new Vector2(_rectStartPoint.x, mousePosWorld.y));   //bottom left
            }
            else
            {
                ShowSelectionBox(false);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            ShowSelectionBox(false);

            var origin = new Vector2((_rectStartPoint.x + mousePosWorld.x) / 2, (_rectStartPoint.y + mousePosWorld.y) / 2);
            var size = new Vector2(Mathf.Abs(_rectStartPoint.x - mousePosWorld.x), Mathf.Abs(_rectStartPoint.y - mousePosWorld.y));

            if (mousePosWorld == _rectStartPoint || mousePosWorld.x == _rectStartPoint.x || mousePosWorld.y == _rectStartPoint.y)
            {
                //raycast
                var character = Physics2D.BoxCast(mousePosWorld, Vector2.one * .1f, 0, Vector2.zero, 0, characterLayer.value);
                if (character.transform)
                {
                    var add = character.transform.gameObject.GetComponent<Hero>();

                    if (add.role != HeroRole.Unassigned)
                        _selectedUnits.Add(add);
                }
            }
            else
            {
                var characters = Physics2D.BoxCastAll(origin, size, 0, Vector2.zero, 0, characterLayer.value);

                if (characters.Length > 0)
                {
                    foreach (RaycastHit2D c in characters)
                    {
                        var add = c.transform.gameObject.GetComponent<Hero>();

                        if (add.role != HeroRole.Unassigned)
                            _selectedUnits.Add(add);
                    }
                }
            }
            Player.AssignSelectedCharacters(_selectedUnits);
        }
    }

    void ControlGroups()
    {
        //add to existing group
        foreach (KeyCode key in controlGroupKeys)
        {
            if (Input.GetKeyDown(key))
            {
                int conntrolGroupPosition = (int)key - (int)KeyCode.Alpha0;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _controlGroups[conntrolGroupPosition].AddUnique(_selectedUnits);
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    _controlGroups[conntrolGroupPosition].Clear();
                    _controlGroups[conntrolGroupPosition].AddRange(_selectedUnits);
                }
                else
                {
                    _selectedUnits.Clear();
                    _selectedUnits.AddUnique(_controlGroups[conntrolGroupPosition]);
                    Player.AssignSelectedCharacters(_controlGroups[conntrolGroupPosition]);
                }
            }
        }
    }

    void ShowSelectionBox(bool show)
    {
        var color = _selectionPolygon.Color;
        _selectionPolygon.Color = new Color(color.r, color.g, color.b, show ? selectionAlpha : 0);
    }

    /*
    private void OnDrawGizmos()
    {
        var origin = new Vector2((_rectStartPoint.x + Player.mousePosWorld.x) / 2, (_rectStartPoint.y + Player.mousePosWorld.y) / 2);
        var size = new Vector2(Mathf.Abs(_rectStartPoint.x - Player.mousePosWorld.x), Mathf.Abs(_rectStartPoint.y - Player.mousePosWorld.y));

        Gizmos.DrawCube(new Vector3(origin.x, origin.y, 0), new Vector3(size.x, size.y, 1));
    }
    */

}

