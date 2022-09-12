using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    //this is so camera movement is smooth and nice
    //putting camera as child of player makes it jittery and not nice

    public Transform player;

    private void Update()
    {
        transform.position = player.transform.position;
    }
}
