using UnityEngine;
using System.Collections;

public class TeleportCollision : MonoBehaviour {

    [SerializeField]
    private GameObject fpsTeleportGun;

    void OnTriggerEnter(Collider target) {
        //if (target.gameObject.tag == "Player") {
          //  Debug.Log("collision with player!");

            //gameObject.SetActive(false);
            //fpsTeleportGun.SetActive(true);
            PlayerLogic.instance.PickedUpTeleportGun();
        //}
    }
}
