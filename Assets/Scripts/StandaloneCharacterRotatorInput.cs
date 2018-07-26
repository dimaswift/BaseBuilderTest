using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterRotator))]
public class StandaloneCharacterRotatorInput : MonoBehaviour
{
    CharacterRotator rotator;

    private void Awake()
    {
        rotator = GetComponent<CharacterRotator>();
    }

    private void FixedUpdate()
    {
        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");
        rotator.Rotate(new Vector2(x, y));
    }
}
