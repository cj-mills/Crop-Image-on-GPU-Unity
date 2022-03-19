using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [Tooltip("The screen to which the test image is attached")]
    public GameObject screen;

    [Tooltip("Toggle whether to crop the test image")]
    public bool cropImage;

    // A copy of the original test image
    private RenderTexture image;


    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the image texture attached to the screen
        Texture screenTexture = screen.GetComponent<MeshRenderer>().material.mainTexture;

        // Create a new RenderTexture with the same dimensions as the test image
        image = new RenderTexture(screenTexture.width, screenTexture.height, 24, RenderTextureFormat.ARGB32);
        // Copy the screenTexture to the image RenderTexture
        Graphics.Blit(screenTexture, image);

        // Get a reference to the Main Camera object
        GameObject mainCamera = GameObject.Find("Main Camera");
        // Adjust the camera so that the whole image is visible
        mainCamera.GetComponent<Camera>().orthographicSize = image.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Allocate a temporary RenderTexture with the original image dimensions
        RenderTexture rTex = RenderTexture.GetTemporary(image.width, image.height, 24, image.format);
        // Copy the original image
        Graphics.Blit(image, rTex);

        if (cropImage)
        {
            // Stores the size of the new square image
            int size;
            // Stores the coordinates in the original image to start copying from
            int[] coords;
            // Temporarily tores the new square image
            RenderTexture tempTex;

            if (image.width > image.height)
            {
                // Set the dimensions for the new square image
                size = image.height;
                // Set the coordinates in the original image to start copying from
                coords = new int[] { (int)((image.width - image.height) / 2f), 0 };
                // Allocate a temporary RenderTexture
                tempTex = RenderTexture.GetTemporary(size, size, 24, image.format);
            }
            else
            {
                // Set the dimensions for the new square image
                size = image.width;
                // Set the coordinates in the original image to start copying from
                coords = new int[] { 0, (int)((image.height - image.width) / 2f) };
                // Allocate a temporary RenderTexture
                tempTex = RenderTexture.GetTemporary(size, size, 24, image.format);
            }

            // Copy the pixel data from the original image to the new square image
            Graphics.CopyTexture(image, 0, 0, coords[0], coords[1], size, size, tempTex, 0, 0, 0, 0);

            // Free the resources allocated for the Temporary RenderTexture
            RenderTexture.ReleaseTemporary(rTex);
            // Allocate a temporary RenderTexture with the new dimensions
            rTex = RenderTexture.GetTemporary(size, size, 24, image.format);
            // Copy the square image
            Graphics.Blit(tempTex, rTex);

            // Free the resources allocated for the Temporary RenderTexture
            RenderTexture.ReleaseTemporary(tempTex);
        }

        // Apply the new RenderTexture
        screen.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", rTex);
        // Adjust the screen dimensions to fit the new RenderTexture
        screen.transform.localScale = new Vector3(rTex.width, rTex.height, screen.transform.localScale.z);

        // Free the resources allocated for the Temporary RenderTexture
        RenderTexture.ReleaseTemporary(rTex);

    }
}
