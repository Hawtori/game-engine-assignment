using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;

    public GameObject player;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }
}
