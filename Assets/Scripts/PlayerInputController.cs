using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public PlayerAction inputAction;

    public static PlayerInputController _instance;

    private void OnEnable()
    {
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }

        inputAction = new PlayerAction();
    }
}
