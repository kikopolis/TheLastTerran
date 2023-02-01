using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestPlayMode {
    public class player_movement_test {
        private float movementSpeed = 5f;
        private float sprintSpeed = 9f;

        [ UnityTest ]
        public IEnumerator player_can_move_forward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            var player = GameObject.FindGameObjectWithTag("Player");
            var controller = player.GetComponent<PlayerController>();
            // reset player rotation
            player.transform.rotation = Quaternion.identity;
            var oldPosition = new Vector3(player.transform.position.x,
                                          player.transform.position.y,
                                          player.transform.position.z);
            yield return null;
            var inputManager = player.GetComponent<InputManager>();
            inputManager.move = new Vector2(0, 1);
            yield return new WaitForSeconds(1f);
            var newPosition = new Vector3(player.transform.position.x,
                                          player.transform.position.y,
                                          player.transform.position.z);
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(movementSpeed, controller.getCurrentSpeed());
            Assert.AreEqual(newPosition.x, oldPosition.x);
            Assert.AreEqual(newPosition.z, oldPosition.z);
            SceneManager.UnloadSceneAsync("SampleScene");
        }

        [ UnityTest ]
        public IEnumerator player_can_sprint_forward() {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;
            var player = GameObject.FindGameObjectWithTag("Player");
            var controller = player.GetComponent<PlayerController>();
            // reset player rotation
            player.transform.rotation = Quaternion.identity;
            var oldPosition = new Vector3(player.transform.position.x,
                                          player.transform.position.y,
                                          player.transform.position.z);
            yield return null;
            var inputManager = player.GetComponent<InputManager>();
            inputManager.sprint = true;
            inputManager.move = new Vector2(0, 1);
            yield return new WaitForSeconds(1f);
            var newPosition = new Vector3(player.transform.position.x,
                                          player.transform.position.y,
                                          player.transform.position.z);
            yield return null;
            Assert.Less(newPosition.y, oldPosition.y);
            Assert.AreEqual(sprintSpeed, controller.getCurrentSpeed());
            Assert.AreEqual(newPosition.x, oldPosition.x);
            Assert.AreEqual(newPosition.z, oldPosition.z);
            SceneManager.UnloadSceneAsync("SampleScene");
        }
    }
}