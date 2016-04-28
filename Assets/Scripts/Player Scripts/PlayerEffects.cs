using UnityEngine;
using System.Collections;
using Kino;

/*
Notes:
    - directional light intensity 1.76 default
*/
public class PlayerEffects : MonoBehaviour {

    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private GameObject directionalLight;
    
    private Material skyboxMaterial;
    private Light light;
    private float skyBoxRotationTopX = 276f;
    private Bokeh bokeh;
    private AnalogGlitch analogGlitch;
    private DigitalGlitch digitalGlitch;
    private Renderer glitchedRenderer;
    private class Bounds {
        public float xMax = 110f;
        public float xMin = -43f;
        public float zMax = 51f;
        public float zMin = -85f;
    }
    private bool outOfBounds;
    
    private Bounds glitchBounds = new Bounds();

    void Awake() {
        analogGlitch = GetComponent<AnalogGlitch>();
        digitalGlitch = GetComponent<DigitalGlitch>();
        bokeh = GetComponent<Bokeh>();
        skyboxMaterial = GetComponent<Skybox>().material;
        skyboxMaterial.SetFloat("_Blend", 0);

        light = directionalLight.GetComponent<Light>();
    }

    void Update() {

        if(transform.localEulerAngles.x < 330f && transform.localEulerAngles.x > 280f) {
            //Debug.Log(transform.localEulerAngles.x);
            float closenessToTopOfSkybox = Mathf.Abs(skyBoxRotationTopX - transform.localEulerAngles.x);
            //Debug.Log("rotation = " + closenessToTopOfSkybox);
            float focalLength = (closenessToTopOfSkybox * 2f);
            //Debug.Log("Focal length pre x 2 = " + focalLength);

            if (focalLength <= 0) {
                focalLength = 0.001f;
            }

            focalLength = focalLength / 1100;
            
            //Debug.Log("Focal length post conditional " + focalLength);

            //const float MAX_FOCAL = 95;
            //Debug.Log("Focal length final * 95 = " + focalLength);

            bokeh.focalLength = focalLength;
        }

        if (!controller.isGrounded) { //player jumps
            //Debug.Log("Intensity of dig glitch: " + digitalGlitch.intensity);

            //Debug.Log("Player Position: (x,y,z): (" + gameObject.transform.position.x + "," +
                //gameObject.transform.position.y + "," +
                //gameObject.transform.position.z + ")");
        }

        float glitchVal = 0;

        if (gameObject.transform.position.x < glitchBounds.xMin) {
             glitchVal = (glitchBounds.xMin - gameObject.transform.position.x) / 100;
            outOfBounds = true;
        }
        else if (gameObject.transform.position.x > glitchBounds.xMax) {
             glitchVal = (gameObject.transform.position.x - glitchBounds.xMax) / 100;
            outOfBounds = true;
        }
        else if (gameObject.transform.position.z < glitchBounds.zMin) {
             glitchVal = (glitchBounds.zMin - gameObject.transform.position.z) / 100;
            outOfBounds = true;
        }
        else if (gameObject.transform.position.z > glitchBounds.zMax) {
             glitchVal = (gameObject.transform.position.z - glitchBounds.zMax) / 100;
            outOfBounds = true;
        }

        if (outOfBounds) {
            if (glitchVal > 1) {
                glitchVal = 1;
            }

            analogGlitch.verticalJump = glitchVal > .15f ? .15f : glitchVal;
            analogGlitch.horizontalShake = glitchVal > .4f ? .4f : glitchVal;
            analogGlitch.scanLineJitter = glitchVal > .25f ? .25f : glitchVal;
            analogGlitch.colorDrift = glitchVal*2 > 1f ? 1f : glitchVal;
            digitalGlitch.intensity = glitchVal/4f > .5f ? .5f : glitchVal;

            float directionalCloseness = glitchVal;
            
            float directionalLightIntensity  = directionalCloseness <= 0 ? 1.76f : Mathf.Abs(1.76f - (directionalCloseness * 5));            
            //Debug.Log(glitchVal + ", " + directionalLightIntensity);
            light.intensity = directionalLightIntensity;

            float skyBoxBlendVal = glitchVal * 3 > 1f ? 1f : glitchVal * 3;
            skyboxMaterial.SetFloat("_Blend", skyBoxBlendVal);
        }
    }
}




