using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCinemachine : MonoBehaviour {
    public CharacterController playerController;
    public float moveSpeed;
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        var f = playerController.transform.localRotation * new Vector3(h, 0, v);
        playerController.Move(f * Time.deltaTime * moveSpeed);

        if (Input.GetKey(KeyCode.Z)) {
            playerController.transform.Rotate(playerController.transform.forward * rotateSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.C)) {
            playerController.transform.Rotate(-playerController.transform.forward * rotateSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) {
            if (playerController.transform.localEulerAngles.x > 0.01f) {
                Quaternion q = Quaternion.Euler(0, playerController.transform.localEulerAngles.y, 0);
                q = Quaternion.Lerp(playerController.transform.localRotation, q, 0.1f);
                playerController.transform.localRotation = q;
                return;
            } else {
                playerController.transform.localRotation = Quaternion.Euler(0, playerController.transform.localEulerAngles.y, 0);
            }
        }

        if (Input.GetKey(KeyCode.Q)) {
            playerController.transform.Rotate(-playerController.transform.up * rotateSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.E)) {
            playerController.transform.Rotate(playerController.transform.up * rotateSpeed * Time.deltaTime, Space.World);
        }
    }
}
