using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMover))]
public class StandaloneMovementInput : MonoBehaviour
{
    CharacterMover mover;

    void Awake ()
    {
        mover = GetComponent<CharacterMover>();
	}
	

	void Update ()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        mover.MovementDirection = new Vector3(x, 0, y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            mover.Jump();
        }
	}
}
