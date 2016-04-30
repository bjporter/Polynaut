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
    [SerializeField]
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
    private float gameStartedAt = 0;
    private bool setGameStartedAt = false;
    private float transitionAfterGameStartedSeconds = 5f;
    private float secondsOfAfterGameStarted = 0f;

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


    [SerializeField]
    private GameObject titlePanel;

    [SerializeField]
    private ShakyText mainMenuTitle;

    [SerializeField]
    private ShakyText mainMenuPresAnyToStart;

    [SerializeField]
    private GameObject mainMenuAsteroids;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject hudPanel;

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
    }

    void Start() {
    }

    void Update () {
        if(player.transform.position.y <= -50f) {
            fpsController.LockKeyboardMove();
            fpsController.LockMouseLook();
            hudPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        //Debug.Log("game object info: " + gameObject.GetComponentInParent<FirstPersonController>().isActiveAndEnabled);

        /*
            GAME STARTED
        */
        if (Input.anyKey && !gameObject.GetComponentInParent<FirstPersonController>().isActiveAndEnabled) {
            gameObject.GetComponentInParent<FirstPersonController>().enabled = true;
            gameStarted = true;
            //fpsController.LockMouseLook();
            //fpsController.LockKeyboardMove();
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
            if(!setGameStartedAt) {
                secondsOfAfterGameStarted = Time.time;
                setGameStartedAt = true;
            }

            if(Time.time > secondsOfAfterGameStarted + transitionAfterGameStartedSeconds) {
                titlePanel.SetActive(false);
            }

            mainMenuAsteroids.SetActive(false);

            /////////////////////////////////////////////////
            /// Hit Ground After falling out of big ship
            /////////////////////////////////////////////////
            if (characterController.isGrounded) {
                if(!hitGroundForFirstTime) {
                    fpsController.UnlockMouseLook();
                    fpsController.UnlockKeyboardMove();
                    cameraShake.shakeDuration = 2;
                    puffOfSmokeDetonator.Explode();
                    hudPanel.SetActive(true);
                    hitGroundForFirstTime = true;
                }
            }

            if (Time.deltaTime >= ONE_FRAME_60FPS) { // or 1/60th a frame
                float intensityIncrement = (Time.deltaTime / transitionAfterGameStartedSeconds); // over 3 seconds get %, and multiply by max intensity of 95
                digitalGlitch.intensity = digitalGlitch.intensity - intensityIncrement < 0 ? 0 : digitalGlitch.intensity - intensityIncrement;
                analogGlitch.scanLineJitter = analogGlitch.scanLineJitter - intensityIncrement < 0 ? 0 : analogGlitch.scanLineJitter - intensityIncrement;
                analogGlitch.verticalJump = analogGlitch.verticalJump - intensityIncrement < 0 ? 0 : analogGlitch.verticalJump - intensityIncrement;
                analogGlitch.horizontalShake = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;
                analogGlitch.colorDrift = analogGlitch.colorDrift  - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement;
                noiseAndGrain.intensityMultiplier = (analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement)/3f;
                tiltShift.blurArea = (analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement)/15;
                bloomAndFlares.bloomIntensity = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;
                ramp.opacity = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;


                mainMenuPresAnyToStart.SetAlpha(analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement);
                mainMenuTitle.SetAlpha(analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement);
            }
        } else {

        } //If game started, else you're in the menu
    } //Update()
}
