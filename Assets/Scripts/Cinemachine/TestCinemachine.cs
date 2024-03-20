using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCinemachine : MonoBehaviour {
    public List<CharacterController> playerControllers;
    public CharacterController playerController;
    public float moveSpeed;
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            playerController = playerControllers[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            playerController = playerControllers[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            playerController = playerControllers[2];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            playerController = playerControllers[3];
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            playerController = playerControllers[4];
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            playerController = playerControllers[5];
        }

        if (Input.GetKey(KeyCode.Space)) {
            playerController.transform.localRotation = Quaternion.Euler(0, playerController.transform.localEulerAngles.y, 0);
            // playerController.transform.position = new Vector3(playerController.transform.position.x, 0, playerController.transform.position.z);
        }

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0) {
            var f = playerController.transform.localRotation * new Vector3(h, 0, v);
            playerController.Move(f * Time.deltaTime * moveSpeed);
        }

        if (Input.GetKey(KeyCode.Z)) {
            playerController.transform.Rotate(playerController.transform.forward * rotateSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.C)) {
            playerController.transform.Rotate(-playerController.transform.forward * rotateSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.Q)) {
            playerController.transform.Rotate(-playerController.transform.up * rotateSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.E)) {
            playerController.transform.Rotate(playerController.transform.up * rotateSpeed * Time.deltaTime, Space.World);
        }
    }
}
