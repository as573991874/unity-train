using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput3 : MonoBehaviour {
    public InputActionAsset inputActions;
    public MyInputAction inputActions2;

    // Start is called before the first frame update
    void Start() {
        inputActions.FindActionMap("My").FindAction("Jump").performed += ctx => {
            Debug.Log("jump");
        };

        inputActions2 = new MyInputAction();
        inputActions2.My.Jump.performed += ctx => {
            Debug.Log("jump2");
        };
    }

    // Update is called once per frame
    void Update() {
    }
}
