using UnityEngine;
using System.Collections;

public class TeleportCollision : MonoBehaviour {

    [SerializeField]
    private GameObject fpsTeleportGun;

    void OnCollisionEnter(Collision target) {
        if (target.gameObject.tag == "Player") {
            gameObject.SetActive(false);
            fpsTeleportGun.SetActive(true);
        }
    }
}
