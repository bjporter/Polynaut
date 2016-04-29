using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;
using Kino;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {

    private GameObject player;
    private FirstPersonController fpsController;
    private AnalogGlitch analogGlitch;
    private DigitalGlitch digitalGlitch;
    private TiltShift tiltShift;
    private NoiseAndGrain noiseAndGrain;
    private BloomAndFlares bloomAndFlares;
    private bool gameStarted = false;
    private float FXAfterStartSeconds = 5f;
    private const float ONE_FRAME_60FPS = 0.0167f;

    [SerializeField]
    private Text title_start_text_layer1;

    [SerializeField]
    private Text title_start_text_layer2;

    [SerializeField]
    private Text title_start_text_layer3;

    void Awake () {
        fpsController = gameObject.GetComponentInParent<FirstPersonController>();
        analogGlitch = gameObject.GetComponent<AnalogGlitch>();
        digitalGlitch = gameObject.GetComponent<DigitalGlitch>();
        tiltShift = gameObject.GetComponent<TiltShift>();
        noiseAndGrain = gameObject.GetComponent<NoiseAndGrain>();
        bloomAndFlares = gameObject.GetComponent<BloomAndFlares>();

        //Initialization
        digitalGlitch.intensity = .2f;
        analogGlitch.scanLineJitter = .4f;
        analogGlitch.verticalJump = .1f;
        analogGlitch.horizontalShake = .05f;
        analogGlitch.colorDrift = .3f;

        //tiltShift.enabled = true;
        //bloomAndFlares.enabled = true;
    }

    void Update () {
        //Debug.Log("game object info: " + gameObject.GetComponentInParent<FirstPersonController>().isActiveAndEnabled);

        /*
            GAME STARTED
        */
        if (Input.anyKey && !gameObject.GetComponentInParent<FirstPersonController>().isActiveAndEnabled) {
            gameObject.GetComponentInParent<FirstPersonController>().enabled = true;
            gameStarted = true;
        }

        Debug.Log("Delta time: " + Time.deltaTime);

        if(digitalGlitch.intensity <= 0) {
            if (tiltShift.isActiveAndEnabled) {
                tiltShift.enabled = false;
                bloomAndFlares.enabled = false;
            }
        }

        if (gameStarted) {
            if (Time.deltaTime >= ONE_FRAME_60FPS) { // or 1/60th a frame
                float intensityIncrement = (Time.deltaTime / FXAfterStartSeconds); // over 3 seconds get %, and multiply by max intensity of 95
                digitalGlitch.intensity = digitalGlitch.intensity - intensityIncrement < 0 ? 0 : digitalGlitch.intensity - intensityIncrement;
                analogGlitch.scanLineJitter = analogGlitch.scanLineJitter - intensityIncrement < 0 ? 0 : analogGlitch.scanLineJitter - intensityIncrement;
                analogGlitch.verticalJump = analogGlitch.verticalJump - intensityIncrement < 0 ? 0 : analogGlitch.verticalJump - intensityIncrement;
                analogGlitch.horizontalShake = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;
                analogGlitch.colorDrift = analogGlitch.colorDrift  - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement;
                noiseAndGrain.intensityMultiplier = (analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement)/7f;
                tiltShift.blurArea = (analogGlitch.colorDrift - intensityIncrement < 0 ? 0 : analogGlitch.colorDrift - intensityIncrement)/15;
                bloomAndFlares.bloomIntensity = analogGlitch.horizontalShake - intensityIncrement < 0 ? 0 : analogGlitch.horizontalShake - intensityIncrement;

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
                float randomStart = Random.Range(-.5f, .5f);
                Vector3 t = title_start_text_layer1.transform.position;
                t.x += randomStart;
                title_start_text_layer1.transform.position = t;
                Debug.Log("**RANDOM START** " + randomStart);
                randomStart = Random.Range(-.5f, .5f);
                t = title_start_text_layer2.transform.position;
                t.x += randomStart;
                title_start_text_layer2.transform.position = t;

                randomStart = Random.Range(-.5f, .5f);
                t = title_start_text_layer3.transform.position;
                t.x += randomStart;
                title_start_text_layer3.transform.position = t;
            }
        }
    }
}
