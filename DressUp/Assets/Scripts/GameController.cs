using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Action ResetCharacterEvent;

    private void Awake()
    {
        instance = this;
        ResetCharacterEvent = null;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void ResetCharacter() //invoke when user press on the loofah
    {
        ResetCharacterEvent.Invoke();
    }
}
