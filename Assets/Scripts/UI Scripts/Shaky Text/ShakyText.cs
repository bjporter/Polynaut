using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShakyText : MonoBehaviour {
    ///////////////////////
    /// Public inspector variables
    ///////////////////////
    [SerializeField]
    private string textValue;

    ///////////////////////
    /// Variables
    ///////////////////////
    private float[] textOriginalPositionX;
    private Text[] textList;
    int textCount = 0;

    ///////////////////////
    /// Constants
    ///////////////////////
    private const float ONE_FRAME_60FPS = 0.0167f;

    void Awake () {
        textList = new Text[3];
        textOriginalPositionX = new float[3];

        foreach (Transform t in transform) {
            textList[textCount] = t.GetComponent<Text>();
            textCount++;
        }

        //textList[0].color = Color.green;
        //Debug.Log("color Alpha: " + textList[0].color.a);
        //Debug.Log("transform position x " + textOriginalPositionX[0]);
    }
	
    void Start() {
        for(int i = 0; i < textList.Length; i++) {
            textOriginalPositionX[i] = textList[i].GetComponent<Transform>().position.x;
            textList[i].text = textValue;
        }

    }

    public void SetAlpha(float newAlpha) {
        for (int i = 0; i < textList.Length; i++) {
            Color c = textList[i].color;
            c.a = newAlpha;

            textList[i].color = c;
        }
    }

    void Update () {
        //Debug.Log("< time: " + Time.deltaTime);
        if (Time.deltaTime >= ONE_FRAME_60FPS) { // or 1/60th a frame
            float textRange = 2.5f;
            float outOfBoundsX = 5f;

            for(int i = 0; i < textList.Length; i++) {
                float randomStart = Random.Range(-textRange, textRange);
                Vector3 t = textList[i].transform.position;
                t.x += randomStart;

                if (Mathf.Abs(textList[i].transform.position.x - textOriginalPositionX[i]) > outOfBoundsX) {
                    t.x = textOriginalPositionX[i];
                }

                textList[i].transform.position = t;
            }
        }
    }
}
