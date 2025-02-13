using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Altered code from the Lerp.cs script. Changes the primitive shapes to cubes and adds rotation to the cubes as part of the animation.
// Also changes the way the color of the cubes changes over the course of the animation
public class DrawHeart : MonoBehaviour
{
    GameObject[] cubes;
    static int numCube = 250; 
    float time = 0f;
    Vector3[] initPos;
    Quaternion[] initRotate;
    Vector3[] startPosition, endPosition;
    Quaternion[] startRotate, endRotate;
    float lerpFraction; // Lerp point between 0~1
    float theta;

    float radius;
    float radStart = 3f;
    float cubeSize;
    void Start()
    {
        // Assign proper types and sizes to the variables.
        cubes = new GameObject[numCube];
        initPos = new Vector3[numCube];
        initRotate = new Quaternion[numCube];

        startPosition = new Vector3[numCube]; 
        endPosition = new Vector3[numCube]; 

        startRotate = new Quaternion[numCube];
        endRotate = new Quaternion[numCube];

        // Define target positions. Start = random, End = heart 
        for (int i =0; i < numCube; i++){
            // Random start positions
            float r = 15f;
            startPosition[i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));        
            // Heart shape end position
            theta = i* 2 * Mathf.PI / numCube;
            endPosition[i] = new Vector3( 
                        5f * Mathf.Sqrt(2f) * Mathf.Pow(Mathf.Sin(theta), 3f),
                        5f * (-Mathf.Pow(Mathf.Cos(theta), 3f) - Mathf.Pow(Mathf.Cos(theta), 2) + (2f * Mathf.Cos(theta))) + 3f,
                        10f + Mathf.Sin(time));
            
            // Random start rotations
            startRotate[i] = Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
            // Rotation of the squares resets to "basic" at the end when they come together to make the heart
            endRotate[i] = Quaternion.Euler(0f, 0f, 0f);
        }

        for (int i =0; i < numCube; i++){
            float r = 10f; // radius of the circle
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube); 

            // Position
            initPos[i] = startPosition[i];
            cubes[i].transform.position = initPos[i];

            // Rotation
            initRotate[i] = startRotate[i];
            cubes[i].transform.rotation = initRotate[i];

            // Color
            // Get the renderer of the cubes and assign colors.
            Renderer cubeRenderer = cubes[i].GetComponent<Renderer>();
            // HSV color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numCube; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            cubeRenderer.material.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Measure Time 
        time += Time.deltaTime; // Time.deltaTime = The interval in seconds from the last frame to the current one
        // what to update over time?
        for (int i =0; i < numCube; i++){
            // Lerp : Linearly interpolates between two points.
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Vector3.Lerp.html
            // Vector3.Lerp(startPosition, endPosition, lerpFraction)
            
            // lerpFraction variable defines the point between startPosition and endPosition (0~1)
            // let it oscillate over time using sin function
            lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

            // Lerp logic. Update position       
            float t = i* 2 * Mathf.PI / numCube;
            cubes[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction);

            // Lerp logic to update rotation
            cubes[i].transform.rotation = Quaternion.Slerp(startRotate[i], endRotate[i], lerpFraction);

            // Gives the cubes a new start point to move to every time they return to the heart shape
            if(cubes[i].transform.position == endPosition[i])
            {   
                float r = Random.Range(5f, 15f);
                startPosition[i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));
            }

            // Color Update over time
            Renderer cubeRenderer = cubes[i].GetComponent<Renderer>();
            float hue = (float)i / numCube; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(Mathf.Abs(hue * Mathf.Sin(time)), Mathf.Sin(time), 0.5f+ Mathf.Sin(time));
            cubeRenderer.material.color = color;
        }
    }
}
