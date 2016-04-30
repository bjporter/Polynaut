using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;
using Kino;
using UnityEngine.UI;

public class PlayerLogic : MonoBehaviour {

    public static PlayerLogic instance;

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


    ////////////////////////////////////
    /// Player / HUD / Game Variables
    ////////////////////////////////////
    private float powerPercent;
    private float powerStartPercent = 75.582f;
    private float powerSecondsLeft = 30; 
    private float powerSecondsStart = 30;
    private bool powerStartCountDown = false;
    private int battariesLeft = 3;

    private bool endingTwoOfThreeStarted = false;
    private bool endingThreeOfThreeStarted = false;

    [SerializeField]
    private GameObject titlePanel;

    [SerializeField]
    private ShakyText mainMenuTitle;

    private bool showPolyNaut;
    private float mainMenuAlphaStart = 0;

    [SerializeField]
    private ShakyText mainMenuPresAnyToStart;

    [SerializeField]
    private GameObject mainMenuAsteroids;

    [SerializeField]
    private GameObject gameOverPanelEnding2;

    [SerializeField]
    private GameObject gameOverPanelEnding3;

    [SerializeField]
    private GameObject hudPanel;

    [SerializeField]
    private GameObject hudPowerTextGroup;

    [SerializeField]
    private GameObject asteroidField1;

    [SerializeField]
    private GameObject largeShip;

    private ShakyText hudPowerText;

    void Awake () {
        if(instance == null) {
            instance = this;
        }

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
        hudPowerText = hudPowerTextGroup.GetComponent<ShakyText>();

        //Initialization
        digitalGlitch.intensity = .2f;
        analogGlitch.scanLineJitter = .414f;
        analogGlitch.verticalJump = .02f;
        analogGlitch.horizontalShake = .011f;                                                          
        analogGlitch.colorDrift = .11f;

        powerPercent = powerStartPercent;
    }

    void Start() {
    }

    private float endGameRotationYStart;
    private float endGameRotationXStart;

    public void AddBatteryPower() {
        battariesLeft--;
        powerSecondsLeft += 30;
    }

    void Update () {
        //KEEP AT THE TOP
        if (gameStarted && Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if(endingThreeOfThreeStarted) {
            gameOverPanelEnding3.SetActive(true);
            fpsController.LockKeyboardMove();
        }

        if (endingTwoOfThreeStarted) {
            asteroidField1.transform.position = player.transform.position;

            /* //Rotate player to top of screen
            if (Time.deltaTime >= ONE_FRAME_60FPS) { // or 1/60th a frame
                Vector3 eulerAngles = player.transform.rotation.eulerAngles;

                Debug.Log("player val = " + player.transform.rotation.eulerAngles.y);
                Debug.Log("cam val = " + transform.rotation.eulerAngles.x);

                if (player.transform.rotation.eulerAngles.y > 86f) {
                    eulerAngles.y -= ((player.transform.rotation.eulerAngles.y - 86f) / 180f);
                }

                if (player.transform.rotation.eulerAngles.y < 86f) {
                    eulerAngles.y += ((86f - player.transform.rotation.eulerAngles.y) / 180f);
                }


                if (player.transform.rotation.eulerAngles.x > 287f) {
                    eulerAngles.x -= ((player.transform.rotation.eulerAngles.x - 86f) / 180f);
                }

                if (player.transform.rotation.eulerAngles.x < 287f) {
                    eulerAngles.x += ((86f - player.transform.rotation.eulerAngles.x) / 180f);
                }



                Debug.Log(eulerAngles.x + ", " + eulerAngles.y);
                player.transform.rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
                transform.rotation = Quaternion.Euler(eulerAngles.x, 180f, 180f);
            }*/

            return;
        }

        /////////////////////////////////////////////////////////////////
        //// Player has fallen of ledge into oblivion, show asteroids
        /////////////////////////////////////////////////////////////////
        if (player.transform.position.y <= -100f) {
            fpsController.LockKeyboardMove();
            //fpsController.LockMouseLook();
            endingTwoOfThreeStarted = true;
            asteroidField1.SetActive(true);
            asteroidField1.transform.position = player.transform.position;
            hudPanel.SetActive(false);
            gameOverPanelEnding2.SetActive(true);
            endGameRotationYStart = player.transform.rotation.eulerAngles.y;
            endGameRotationXStart = transform.rotation.eulerAngles.x;
}

        /*
            GAME STARTED
        */
        if (Input.anyKey && !gameObject.GetComponentInParent<FirstPersonController>().isActiveAndEnabled) {
            gameObject.GetComponentInParent<FirstPersonController>().enabled = true;
            gameStarted = true;
            //fpsController.LockMouseLook();
            fpsController.LockKeyboardMove();
        }

        if(powerStartCountDown) {
            powerSecondsLeft -= Time.deltaTime;
            powerPercent = (powerSecondsLeft / powerSecondsStart) * powerStartPercent;
            Debug.Log(Time.deltaTime);
            Debug.Log("Time: " + Time.deltaTime + ", " + powerSecondsLeft);
            Debug.Log("*" + "(" + powerSecondsStart + ") * " + powerStartPercent);

            if (powerPercent < 0.025f) {

                endingThreeOfThreeStarted = true;
                powerPercent = 0.0f;
            }

            hudPowerText.SetText(powerPercent.ToString("0.00") + "%");
        }

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

                    powerStartCountDown = true;
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
            if (Time.deltaTime >= ONE_FRAME_60FPS) { // or 1/60th a frame
                
                if(showPolyNaut && mainMenuAlphaStart <= .3) {
                    mainMenuAlphaStart += 0.0065f;
                    mainMenuTitle.SetAlpha(mainMenuAlphaStart);
                    mainMenuPresAnyToStart.SetAlpha(mainMenuAlphaStart);
                }

                if (largeShip.transform.position.y >= 300f) {
                    Vector3 t = largeShip.transform.position;
                    t.y -= 6f;
                    largeShip.transform.position = t;
                } else {
                    showPolyNaut = true;
                }
            }
        } //If game started, else you're in the menu
    } //Update()
}
