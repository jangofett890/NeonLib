using NeonLib.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeonLib.Input {
    [RequireComponent(typeof(PlayerInput), typeof(HumanoidPhysicsBasedController))]
    public class PlayerInputForHumanoidEntity : MonoBehaviour {
        public PlayerInput playerInput;
        public Camera mainCamera;
        public HumanoidPhysicsBasedController humanoidController;
        InputAction jump;
        InputAction move;
        // Start is called before the first frame update
        void Start() {
            playerInput = GetComponent<PlayerInput>();
            humanoidController = GetComponent<HumanoidPhysicsBasedController>();
            mainCamera = Camera.main;
            playerInput.ActivateInput();

            move = playerInput.actions.FindAction("Move");
            jump = playerInput.actions.FindAction("Jump");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        private void Update() {
            if(jump.ReadValue<float>() > 0) {
                humanoidController.Jump();
            }
            Vector2 input = move.ReadValue<Vector2>();
            //Make the input relative to the camera via quaternion
            Vector3 movee = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * new Vector3(input.x, 0f, input.y);
            humanoidController.Move(movee);
        }

        private void OnMoveCancel(InputAction.CallbackContext obj) {
            humanoidController.Move(Vector3.zero);
        }

        private void OnMove(InputAction.CallbackContext obj) {

        }
    }
}


