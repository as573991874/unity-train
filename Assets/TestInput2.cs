using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput2 : MonoBehaviour {
    public InputAction move;
    public InputAction jump;

    // Start is called before the first frame update
    void Start() {
        jump.performed += ctx => {
            Debug.Log("jump");
        };
    }

    // Update is called once per frame
    void Update() {
        var moveAmout = move.ReadValue<Vector2>();
        Debug.Log(moveAmout);
    }

    void OnEnable() {
        move.Enable();
        jump.Enable();
    }

    void OnDisable() {
        move.Disable();
        jump.Disable();
    }
}
