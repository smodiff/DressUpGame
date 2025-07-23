using UnityEngine;

public class Brush : MonoBehaviour
{
    public Transform _workBrushPosition;
    public Transform _brushBookPosition;
    public Vector3 _brushPositionInHand;

    [SerializeField] private HandController _handController;

    public void TakeBrush()
    {
        _handController.TakeBrush(this, null);
    }
}
