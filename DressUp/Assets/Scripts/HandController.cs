using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HandController : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [SerializeField] private Transform _positionOutside;            // class controlls not only hand movement
                                                                    // but and make up of the character
    [SerializeField] private Transform _defaultPosition;
    [SerializeField] private Transform _handBrushPosition;
    [SerializeField] private Transform _lipstickWorkPosition;
    [SerializeField] private Transform _lipstickHandler;
    [SerializeField] private Transform _creamPosition;

    [SerializeField] private Brush _blushBrush;
    [SerializeField] private Brush _eyeBrush;

    [SerializeField] private Transform FaceBorder;
    [SerializeField] private Transform NeutralBorder;
    [SerializeField] private Transform BookBorder;

    private Transform _transform;
    private ColorPick _currentColorPick;
    private List<ColorPick> _aplliedColorPicks;
    private Brush _currentBrush;
    private BoxCollider2D _handCollider;
    private bool _isItemTaken;
    private Sequence _animatingSequence;
    private Vector3 _lipstickPosition;

    private void Start()
    {
        _transform = transform;
        _isItemTaken = false;
        _handCollider = GetComponent<BoxCollider2D>();
        _aplliedColorPicks = new List<ColorPick>();

        GameController.instance.ResetCharacterEvent += ResetAction;
    }

    private void TakeByItem(ColorPick color)
    {
        switch (color.ColorType)
        {
            case ColorPick.MakeUpType.Blush:
                TakeBrush(_blushBrush, color);
                break;
            case ColorPick.MakeUpType.EyeShadow:
                TakeBrush(_eyeBrush, color);
                break;
        }
    }

    public void ResetAction()
    {                         // method that signed on event in GameController class
        foreach (ColorPick appliedColorPick in _aplliedColorPicks.ToArray())
        {
            appliedColorPick.CharacterAttribute.SetActive(false);

            if (appliedColorPick.ColorType == ColorPick.MakeUpType.Cream)
                appliedColorPick.CharacterAttribute.SetActive(true);
        }
    }

    public void SelectColor(ColorPick color)
    {                                        // method invokes when user select color for brushes

        if (_animatingSequence == null || !_animatingSequence.IsPlaying())
        {
            if (_isItemTaken == false)
            {
                TakeByItem(color);
                return;
            }
            _currentColorPick = color;
            _handCollider.enabled = false;

            Vector3 targetPosition = color.transform.position + new Vector3(0, -3.5f, 0);
            _animatingSequence = DOTween.Sequence();

            _animatingSequence.Append(_transform.DOMove(targetPosition, .6f));
            _animatingSequence.Append(_transform.DOMove(targetPosition + new Vector3(.4f, 0, 0), .3f));
            _animatingSequence.Append(_transform.DOMove(targetPosition + new Vector3(-.8f, 0, 0), .3f));
            _animatingSequence.Append(_transform.DOMove(targetPosition + new Vector3(.8f, 0, 0), .3f));
            _animatingSequence.Append(_transform.DOMove(targetPosition + new Vector3(-.8f, 0, 0), .3f));
            _animatingSequence.Append(_transform.DOMove(targetPosition + new Vector3(.8f, 0, 0), .3f));
            _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f).OnComplete(() =>
            {
                _handCollider.enabled = true;
            }));

            _currentBrush.GetComponent<Renderer>().material.SetColor("_AlterColor", color.Color);
        }
    }

    public void SelectLipstick(ColorPick color)
    {                                               //same for the lipsticks
        if (_animatingSequence == null || !_animatingSequence.IsPlaying())
        {
            if (_currentColorPick != null)
            {
                DropBrush();
                return;
            }
            _currentColorPick = color;
            _handCollider.enabled = false;

            Vector3 targetPosition = color.transform.position + new Vector3(0, -3.5f, 0);
            _animatingSequence = DOTween.Sequence();

            _animatingSequence.Append(_transform.DOMove(targetPosition, .6f).OnComplete(() =>
            {
                _lipstickPosition = color.transform.position;
                color.transform.parent = _handBrushPosition;
                color.transform.localPosition = Vector3.zero;
                color.GetComponent<Button>().interactable = false;
            }));
            _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f).OnComplete(() =>
            {
                _handCollider.enabled = true;
            }));
        }
    }

    public void SelectCream(ColorPick color) // same for the cream
    {
        if (_animatingSequence == null || !_animatingSequence.IsPlaying())
        {
            if ((_isItemTaken && _currentColorPick == null) || (_currentColorPick != null && _currentColorPick.ColorType != ColorPick.MakeUpType.Cream))
            {
                DropBrush();
                return;
            }
            _currentColorPick = color;
            _handCollider.enabled = false;

            Vector3 targetPosition = color.transform.position + new Vector3(0, -3.5f, 0);
            _animatingSequence = DOTween.Sequence();

            _animatingSequence.Append(_transform.DOMove(targetPosition, .6f).OnComplete(() =>
            {
                color.transform.parent = _handBrushPosition;
                color.transform.localPosition = Vector3.zero;
                color.GetComponent<Button>().interactable = false;
            }));
            _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f).OnComplete(() =>
            {
                _handCollider.enabled = true;
            }));
        }
    }

    public void TakeBrush(Brush brush, ColorPick color = null) // method invokes when user press on the brushes
    {
        if (_animatingSequence == null || !_animatingSequence.IsPlaying())
        {
            if ((_isItemTaken && _currentColorPick == null) || (_currentColorPick != null && _currentColorPick.ColorType == ColorPick.MakeUpType.Cream))
            {
                DropBrush();
                return;
            }
            _currentBrush = brush;
            brush.GetComponent<Button>().interactable = false;
            _animatingSequence = DOTween.Sequence();
            _animatingSequence.Append(_transform.DOMove(new Vector3(brush.transform.position.x, brush.transform.position.y, _transform.position.z), .6f).OnComplete(() =>
            {
                brush.transform.parent = _handBrushPosition;
                brush.transform.localPosition = Vector3.zero;
                if (color != null)
                {
                    SelectColor(color);
                }
            }));

            if (color == null)
                _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f));
            _handCollider.enabled = true;
            _isItemTaken = true;
        }
    }

    public void DropBrush() //method that drop ALL ITEMS in hand 
    {
        DOTween.Kill(_transform);
        _animatingSequence.Kill();
        if (_animatingSequence == null || !_animatingSequence.IsPlaying())
        {
            if (_isItemTaken)
            {
                _animatingSequence = DOTween.Sequence();
                _animatingSequence.Append(_transform.DOMove(_currentBrush._brushBookPosition.position, .6f).OnComplete(() =>
                {
                    _currentBrush.transform.parent = _currentBrush._brushBookPosition;
                    _currentBrush.transform.localPosition = Vector3.zero;
                    _currentBrush.GetComponent<Button>().interactable = true;
                    _currentBrush = null;
                    _isItemTaken = false;
                }));
                _animatingSequence.Append(_transform.DOMove(_positionOutside.position, .6f));
                _handCollider.enabled = false;
                _currentBrush.GetComponent<Renderer>().material.SetColor("_AlterColor", UnityEngine.Color.white);
                _currentColorPick = null;
            }
            else if (_currentColorPick != null && _currentColorPick.ColorType == ColorPick.MakeUpType.Lipstick)
            {
                _animatingSequence = DOTween.Sequence();
                _animatingSequence.Append(_transform.DOMove(_lipstickPosition, .6f).OnComplete(() =>
                {
                    _currentColorPick.transform.parent = _lipstickHandler;
                    _currentColorPick.transform.position = _lipstickPosition;
                    _currentColorPick.GetComponent<Button>().interactable = true;
                    _currentColorPick = null;
                }));
                _animatingSequence.Append(_transform.DOMove(_positionOutside.position, .6f));
                _handCollider.enabled = false;
            }
            else if (_currentColorPick != null && _currentColorPick.ColorType == ColorPick.MakeUpType.Cream)
            {
                _animatingSequence = DOTween.Sequence();
                _animatingSequence.Append(_transform.DOMove(_creamPosition.position, .6f).OnComplete(() =>
                {
                    _currentColorPick.transform.parent = _creamPosition;
                    _currentColorPick.transform.localPosition = Vector3.zero;
                    _currentColorPick.GetComponent<Button>().interactable = true;
                    _currentColorPick = null;
                }));
                _animatingSequence.Append(_transform.DOMove(_positionOutside.position, .6f));
                _handCollider.enabled = false;
            }
        }
    }

    public void ApplyFaceMakeUp() // invoke when user stopped drag the brush around face area
    {
        if (_currentColorPick != null)
        {
            switch (_currentColorPick.ColorType)
            {
                case ColorPick.MakeUpType.Blush:
                    _animatingSequence = DOTween.Sequence();
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position, .6f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(.4f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f));
                    break;
                case ColorPick.MakeUpType.EyeShadow:
                    _animatingSequence = DOTween.Sequence();
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position, .6f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(.4f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_currentBrush._workBrushPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f));
                    break;
                case ColorPick.MakeUpType.Lipstick:
                    _animatingSequence = DOTween.Sequence();
                    _animatingSequence.Append(_transform.DOMove(_lipstickWorkPosition.position, .6f));
                    _animatingSequence.Append(_transform.DOMove(_lipstickWorkPosition.position + new Vector3(.4f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_lipstickWorkPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_lipstickWorkPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_lipstickWorkPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_lipstickWorkPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f));
                    break;
                case ColorPick.MakeUpType.Cream:
                    _animatingSequence = DOTween.Sequence();
                    _animatingSequence.Append(_transform.DOMove(_blushBrush._workBrushPosition.position, .6f));
                    _animatingSequence.Append(_transform.DOMove(_blushBrush._workBrushPosition.position + new Vector3(.4f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_blushBrush._workBrushPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_blushBrush._workBrushPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_blushBrush._workBrushPosition.position + new Vector3(-.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_blushBrush._workBrushPosition.position + new Vector3(.8f, 0, 0), .3f));
                    _animatingSequence.Append(_transform.DOMove(_defaultPosition.position, 0.6f));
                    break;
            }



            foreach (ColorPick appliedColorPick in _aplliedColorPicks.ToArray())
            {
                if (appliedColorPick.ColorType == _currentColorPick.ColorType)
                {
                    appliedColorPick.CharacterAttribute.SetActive(false);
                    _aplliedColorPicks.Remove(appliedColorPick);
                    break;
                }
            }
            _aplliedColorPicks.Add(_currentColorPick);
            _currentColorPick.CharacterAttribute.SetActive(true);


            if (_currentColorPick.ColorType == ColorPick.MakeUpType.Cream)
            {
                _currentColorPick.CharacterAttribute.SetActive(false);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(_transform.position).z;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        _transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 handScreenPosition = Camera.main.WorldToScreenPoint(_transform.position) + new Vector3(0, 300);
        Vector2 NeutralBorderPosition = Camera.main.WorldToScreenPoint(NeutralBorder.position);
        Vector2 FaceBorderPosition = Camera.main.WorldToScreenPoint(FaceBorder.position);
        Vector2 BookBorderPosition = Camera.main.WorldToScreenPoint(BookBorder.position);

        if (Vector2.Distance(handScreenPosition, NeutralBorderPosition) > Vector2.Distance(handScreenPosition, FaceBorderPosition))
        {
            ApplyFaceMakeUp();
        }
        else if (Vector2.Distance(handScreenPosition, NeutralBorderPosition) > Vector2.Distance(handScreenPosition, BookBorderPosition))
        {
            DropBrush();
        }
    }
}
