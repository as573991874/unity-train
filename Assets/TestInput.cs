using UnityEngine;
using UnityEngine.InputSystem;  // 1. The Input System "using" statement

public class TestInput : MonoBehaviour {
    private void Start() {
    }

    void Update() {
        var gamepad = Gamepad.current;
        if (gamepad == null) {
            return;
        }

        if (gamepad.rightTrigger.wasPressedThisFrame) {
            // 'Use' code here
        }

        Vector2 move = gamepad.leftStick.ReadValue();
        {
            // 'Move' code here
        }

    }
}
