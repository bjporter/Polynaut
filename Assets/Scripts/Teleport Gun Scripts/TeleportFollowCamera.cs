using UnityEngine;
using System.Collections;

public class TeleportFollowCamera : MonoBehaviour {

    [SerializeField]
    float x_offset;

    [SerializeField]
    float y_offset;

    [SerializeField]
    float z_offset;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Camera mainCamera;

    private Transform mainCameraTransform;


    void Awake () {
        //mainCameraTran = player.GetComponentInChildren<Transform>();
        mainCamera.transform.localScale = new Vector3(1, 1, 1);
    }
	
    void Start() {
        //transform.position = new Vector3(0, 0, 0);
        //transform.position = new Vector3(x_offset, y_offset, z_offset);
    }

    void Update () {
        //transform.position = new Vector3(x_offset, y_offset, z_offset);

        //transform.localScale = new Vector3(1, 1, 1);
        //x:.9, y:-.2, z:1 
        //Set Main camera scale to 1,1,1


        /*
        //set rotation from aswd of player
        float x = player.transform.position.x;
        float y = player.transform.position.y;
        float z = player.transform.position.z;
        Vector3 positionTemp = new Vector3(x + x_offset, y + y_offset, z + z_offset);
        //Vector3 positionTemp = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        transform.position = positionTemp; //new Vector3(0,50,0); //positionTemp;

        //Set rotation from mouse of player
        Vector3 eulerTemp = player.transform.rotation.eulerAngles;
        eulerTemp.x = mainCamera.transform.rotation.eulerAngles.x;
        //Quaternion t = player.transform.rotation;
        //t. = mainCameraTransform.rotation.x;
        //transform.rotation = t;
        transform.rotation = Quaternion.Euler(eulerTemp.x, eulerTemp.y, eulerTemp.z);
        Debug.Log("quaternion: " + eulerTemp.x + ", " + eulerTemp.y + ", " + eulerTemp.z);
        Debug.Log("main cam rotation: " + mainCamera.transform.rotation.eulerAngles.x + ", " + mainCamera.transform.rotation.eulerAngles.y + ", " + mainCamera.transform.rotation.eulerAngles.z);
        Debug.Log("Player rotation: " + player.transform.rotation.eulerAngles.x + ", " + player.transform.rotation.eulerAngles.y + ", " + player.transform.rotation.eulerAngles.z);
        */

    }
}
