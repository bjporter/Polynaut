using UnityEngine;
using System.Collections;

public class BatteryCollision : MonoBehaviour {
    void OnCollisionEnter(Collision target) {
        if (target.gameObject.tag == "Player") {
            gameObject.SetActive(false);
        }
    }
}
