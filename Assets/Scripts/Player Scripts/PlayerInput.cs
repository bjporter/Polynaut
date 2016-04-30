using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;
using Kino;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {


    ///////////////////////
    /// Player Components
    ///////////////////////
    private GameObject player;
    private FirstPersonController fpsController;
    private CharacterController characterController;

    ///////////////////////
    /// FX Components on Player
    ///////////////////////
    private AnalogGlitch analogGlitch;
    private DigitalGlitch digitalGlitch;
    private TiltShift tiltShift;
    private Ramp ramp;
    private NoiseAndGrain noiseAndGrain;
    private BloomAndFlares bloomAndFlares;
    private CameraShake cameraShake;


    ///////////////////////
    /// External FX
    ///////////////////////
    [SerializeField]
    private GameObject puffOfSmoke;
    private Detonator puffOfSmokeDetonator;


    ///////////////////////
    /// Constants
    ///////////////////////
    private const float ONE_FRAME_60FPS = 0.0167f;


    ///////////////////////
    /// Game States
    /// Notes: Use camera parent y position for height checks (height right now is 25.8594y)
    ///////////////////////

    /// <summary>
    /// Main Menu closed transition.
    /// Player is out of the start menu, and in the game
    /// </summary>
    private bool gameStarted = false;
    private float secondsOfTransitionAfterGameStarted = 5f;

    /// <summary>
    /// Falling Will now Start.
    /// Initial transition into the game is done, and we're now looking down on the map ready to fall out of the big ship
    /// Start at x0, Rotate: to x-26 in 1.5 seconds
    /// </summary>
    private bool fallingOutOfBigShipStarted = false;
    private float secondsOfTransitionAfterfallingStarted = 1.5f;

    /// <summary>
    /// Falling 1.
    /// Rotate: x-26 to x50 in 1.2 seconds
    /// </summary>
    private bool fallingActionOneDone = false;

    /// <summary>
    /// Falling 2.
    /// </summary>
    private bool fallingActionTwoDone = false;

    /// <summary>
    /// Falling 3.
    /// Rotate: x50 to x87 in 1.5 seconds WHILE z-5.7 rotates continously until ground is hit
    /// </summary>
    private bool fallingActionThreeDone = false;

    /// <summary>
    /// Ground / Done Falling.
    /// Player has hit the ground for the first time, explosion starts
    /// </summary>
    private bool hitGroundForFirstTime = false;
    
    /*
    private bool isEnding1; // teleport out
    private bool isEnding2; // falling
    private bool isEnding3; // power
    */

    [SerializeField]
    private Text title_start_text_layer1;

    [SerializeField]
    private Text title_start_text_layer2;

    [SerializeField]
    private Text title_start_text_layer3;

    private float text1X = 0f;
    private float text2X = 0f;
    private float text3X = 0f;

    void Awake () {
        fpsController = gameObject.GetComponentInParent<FirstPersonController>();
        characterController = gameObject.GetComponentInParent<CharacterController>();
        analogGlitch = gameObject.GetComponent<AnalogGlitch>();
        digitalGlitch = gameObject.GetComponent<DigitalGlitch>();
        tiltShift = gameObject.GetComponent<TiltShift>();
        noiseAndGrain = gameObject.GetComponent<NoiseAndGrain>();
        bloomAndFlares = gameObject.GetComponent<BloomAndFlares>();
        ramp = gameObject.GetComponents<Ramp>()[1];
        puffOfSmokeDetonator = puffOfSmoke.GetComponent<Detonator>();
        cameraShake = gameObject.GetComponent<CameraShake>();

        //Initialization
        digitalGlitch.intensity = .2f;
        analogGlitch.scanLineJitter = .414f;
        analogGlitch.verticalJump = .02f;
        analogGlitch.horizontalShake = .011f;                                                          
        analogGlitch.colorDrift = .11f;

        text1X = title_start_text_layer1.transform.position.x;
        text2X = title_start_text_layer2.transform.position.x;
        text3X = title_start_text_layer3.transform.position.x;

        //tiltShift.enabled = true;
        //bloomAndFlares.enabled = true;
    }

    void Start() {
        //lock player mouse test
    }

    void Update () {
        //Debug.Log("game object info: " + gameObject.GetComponentInParent<FirstPersonController>().isActiveAndEnabled);

        /*
            GAME STARTED
        */
        if (Input.anyKey && !gameObject.GetComponentInParent<FirstPersonController>().isActiveAndEnabled) {
            gameObject.GetComponentInParent<FirstPersonController>().enabled = true;
            gameStarted = true;
            fpsController.LockMouseLook();
            fpsController.LockKeyboardMove();
        }

        if (gameStarted && Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        //Debug.Log("Delta time: " + Time.deltaTime);

        if(digitalGlitch.intensity <= 0) {
            if (tiltShift.isActiveAndEnabled) {
                tiltShift.enabled = false;
                bloomAndFlares.enabled = false;
                ramp.enabled = false;
                analogGlitch.enabled = false;
                digitalGlitch.enabled = false;
            }
        }

        if (gameStarted) {
            /////////////////////////////////////////////////
            /// Hit Ground After falling out of big ship
            /////////////////////////////////////////////////
            if (characterController.isGrounded) {
                if(!hitGroundForFirstTime) {
                    fpsController.UnlockMouseLook();
                    fpsController.UnlockKeyboardMove();
                    cameraShake.shakeDuration = 2;
                    puffOfSmokeDetonator.Explode();
                    hitGroundForFirstTime = true;
                }
            }

            if (Time.deltaTime >= ONE_FRAME_60FPS) { // or 1/60th a frame
                float intensityIncrement = (Time.deltaTime / secondsOfTransitionAfterGameStarted); // over 3 seconds get %, and multiply by max intensity of 95
                digitalGlitch.intensity = digitalGlitch.intensity - intensityIncrement < 0 ? 0 : digitalGlitch.intensity - intensityIncrement;
                analogGlitch.scanLineJitter = analogGlitch.scanLineJitter - intensityIncrement < 0 ? 0 : analogGlitch.scanLineJitter - intensityIncrement;
                analogGlitch.verticalJump = analogGlitch.verticalJump - intensityIncrement < 0 ? 0 : analogGlitch.verticalJump - intensityIncrement;
                analogGlitch.horizontalShake = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;
                analogGlitch.colorDrift = analogGlitch.colorDrift  - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement;
                noiseAndGrain.intensityMultiplier = (analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement)/7f;
                tiltShift.blurArea = (analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement)/15;
                bloomAndFlares.bloomIntensity = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;
                ramp.opacity = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;

                Color c = title_start_text_layer1.color;
                c.a = analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement;
                title_start_text_layer1.color = c;

                c = title_start_text_layer2.color;
                c.a = analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement;
                title_start_text_layer2.color = c;

                c = title_start_text_layer3.color;
                c.a = analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement;
                title_start_text_layer3.color = c;
            }
        } else {
            if (Time.deltaTime >= ONE_FRAME_60FPS) { // or 1/60th a frame
                float textRange = 2.5f;
                float outOfBoundsX = 5f;

                float randomStart = Random.Range(-textRange, textRange);
                Vector3 t = title_start_text_layer1.transform.position;
                t.x += randomStart;

                if (Mathf.Abs(title_start_text_layer1.transform.position.x - text1X) > outOfBoundsX) {
                    t.x = text1X;
                }

                title_start_text_layer1.transform.position = t;


                randomStart = Random.Range(-textRange, textRange);
                t = title_start_text_layer2.transform.position;
                t.x += randomStart;

                if(Mathf.Abs(title_start_text_layer2.transform.position.x - text2X) > outOfBoundsX) {
                    t.x = text2X;
                }

                title_start_text_layer2.transform.position = t;

                randomStart = Random.Range(-textRange, textRange);
                t = title_start_text_layer3.transform.position;
                t.x += randomStart;


                if (Mathf.Abs(title_start_text_layer3.transform.position.x - text3X) > outOfBoundsX) {
                    t.x = text3X;
                }

                title_start_text_layer3.transform.position = t;

            }
        } //If game started, else you're in the menu
    } //Update()
}
