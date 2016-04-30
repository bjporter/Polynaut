using UnityEngine;
using System.Collections;

public class TeleportCollision : MonoBehaviour {

    [SerializeField]
    private GameObject fpsTeleportGun;

    void OnCollisionEnter(Collision target) {
        //Debug.Log("A collision happened!");
        if (target.gameObject.tag == "Player") {
            //Debug.Log("We picked up the teleport gun!");
            gameObject.SetActive(false);
            fpsTeleportGun.SetActive(true);
        }
    }

    void Start () {
	
	}
	
	void Update () {
	
	}
}
