using UnityEngine;

public class ColorPick : MonoBehaviour
{
    public enum MakeUpType
    {
        Blush,
        EyeShadow,
        Lipstick,
        Cream
    }

    public MakeUpType ColorType;
    public Color Color;
    public GameObject CharacterAttribute;
}
