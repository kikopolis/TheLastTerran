using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestPlayMode {
    public class PlayerMovementTest {
        private const float SLOW_TEST_WAIT = 1f;
        private const float FAST_TEST_WAIT = 0.2f;
        private const float MOVEMENT_SPEED = 5f;
        private const float SPRINT_SPEED = 9f;
        private const float WALK_SPEED = 2f;
        private const float CROUCH_SPEED = 1f;
        private readonly float dashSpeed = 12f;
        private readonly float dashDistance = 5f;
        private const float CROUCH_HEIGHT = 0.5f;
        private readonly Vector2 forward = new(0, 1);
        private readonly Vector2 backward = new(0, -1);
        private readonly Vector2 left = new(-1, 0);
        private readonly Vector2 right = new(1, 0);
        private GameObject player;
        private PlayerController controller;
        private InputManager inputManager;
        private Vector3 oldPosition;
        private Vector3 newPosition;

        [ UnityTest ]
        public IEnumerator player_can_move_forward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.move = forward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(MOVEMENT_SPEED, controller.getCurrentSpeed());
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Greater(newPosition.z, oldPosition.z);
        }

        [ UnityTest ]
        public IEnumerator player_can_move_backward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.move = backward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(MOVEMENT_SPEED, controller.getCurrentSpeed());
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Less(newPosition.z, oldPosition.z);
        }

        [ UnityTest ]
        public IEnumerator player_can_move_left() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.move = left;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(MOVEMENT_SPEED, controller.getCurrentSpeed());
            Assert.Less(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
        }

        [ UnityTest ]
        public IEnumerator player_can_move_right() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.move = right;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(MOVEMENT_SPEED, controller.getCurrentSpeed());
            Assert.Greater(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
        }

        [ UnityTest ]
        public IEnumerator player_can_sprint_forward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.sprint = true;
            inputManager.move = forward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(SPRINT_SPEED, controller.getCurrentSpeed());
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Greater(newPosition.z, oldPosition.z);
        }

        [ UnityTest ]
        public IEnumerator player_can_sprint_backward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.sprint = true;
            inputManager.move = backward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(SPRINT_SPEED, controller.getCurrentSpeed());
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Less(newPosition.z, oldPosition.z);
        }

        [ UnityTest ]
        public IEnumerator player_can_sprint_left() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.sprint = true;
            inputManager.move = left;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(SPRINT_SPEED, controller.getCurrentSpeed());
            Assert.Less(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
        }

        [ UnityTest ]
        public IEnumerator player_can_sprint_right() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.sprint = true;
            inputManager.move = right;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(SPRINT_SPEED, controller.getCurrentSpeed());
            Assert.Greater(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
        }

        [ UnityTest ]
        public IEnumerator player_can_walk_forward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.walk = true;
            inputManager.move = forward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(WALK_SPEED, controller.getCurrentSpeed());
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Greater(newPosition.z, oldPosition.z);
        }

        [ UnityTest ]
        public IEnumerator player_can_walk_backward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.walk = true;
            inputManager.move = backward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(WALK_SPEED, controller.getCurrentSpeed());
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Less(newPosition.z, oldPosition.z);
        }

        [ UnityTest ]
        public IEnumerator player_can_walk_left() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.walk = true;
            inputManager.move = left;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(WALK_SPEED, controller.getCurrentSpeed());
            Assert.Less(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
        }

        [ UnityTest ]
        public IEnumerator player_can_walk_right() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.walk = true;
            inputManager.move = right;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(WALK_SPEED, controller.getCurrentSpeed());
            Assert.Greater(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
        }

        [ UnityTest ]
        public IEnumerator player_can_crouch() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            var playerHeight = player.GetComponent<Transform>();
            yield return null;
            inputManager.crouch = true;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.AreEqual(CROUCH_HEIGHT, playerHeight.localScale.y);
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
            Assert.Less(newPosition.y, oldPosition.y);
        }

        [ UnityTest ]
        public IEnumerator player_moves_at_crouch_speed_moving_forward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.crouch = true;
            inputManager.move = forward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.AreEqual(controller.getCurrentSpeed(), CROUCH_SPEED);
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Greater(newPosition.z, oldPosition.z);
            Assert.Less(newPosition.y, oldPosition.y);
        }

        [ UnityTest ]
        public IEnumerator player_moves_at_crouch_speed_moving_backward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.crouch = true;
            inputManager.move = backward;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.AreEqual(controller.getCurrentSpeed(), CROUCH_SPEED);
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.Less(newPosition.z, oldPosition.z);
            Assert.Less(newPosition.y, oldPosition.y);
        }

        [ UnityTest ]
        public IEnumerator player_moves_at_crouch_speed_moving_left() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.crouch = true;
            inputManager.move = left;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.AreEqual(controller.getCurrentSpeed(), CROUCH_SPEED);
            Assert.Less(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
            Assert.Less(newPosition.y, oldPosition.y);
        }

        [ UnityTest ]
        public IEnumerator player_moves_at_crouch_speed_moving_right() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.crouch = true;
            inputManager.move = right;
            yield return new WaitForSeconds(SLOW_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.AreEqual(controller.getCurrentSpeed(), CROUCH_SPEED);
            Assert.Greater(newPosition.x, oldPosition.x);
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
            Assert.Less(newPosition.y, oldPosition.y);
        }

        [ UnityTest ]
        public IEnumerator player_can_jump() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            MoveTestSetup();
            yield return null;
            inputManager.jump = true;
            yield return new WaitForSeconds(FAST_TEST_WAIT);
            newPosition = GetPlayerTransformPosition();
            yield return null;
            Assert.Greater(newPosition.y, oldPosition.y);
            Assert.True(Mathf.Approximately(newPosition.x, oldPosition.x));
            Assert.True(Mathf.Approximately(newPosition.z, oldPosition.z));
        }

        private void MoveTestSetup() {
            player = GameObject.FindGameObjectWithTag("Player");
            controller = player.GetComponent<PlayerController>();
            // reset player rotation
            player.transform.rotation = Quaternion.identity;
            oldPosition = GetPlayerTransformPosition();
            inputManager = player.GetComponent<InputManager>();
        }

        private Vector3 GetPlayerTransformPosition() {
            var position = player.transform.position;
            return new Vector3(position.x, position.y, position.z);
        }
    }
}