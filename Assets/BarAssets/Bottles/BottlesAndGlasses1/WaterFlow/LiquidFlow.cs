using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidFlow : MonoBehaviour
{
    [SerializeField] private GameObject bottle;
    [SerializeField] private GameObject glass;
    [SerializeField] private GameObject liquidFlow; // == ObiSolver
    [SerializeField] private GameObject liquidInBottle;
    [SerializeField] private GameObject liquidInGlass;
    [SerializeField] private AudioSource audiosource;

    private float tiltThreshold = -45f;
    private float tiltThreshold2 = 45f;
    private Material liquidInBottleMaterial;
    private Material liquidInGlassMaterial;

    private float bottleFillCurrent = 0.9f;
    private float bottleFillEnd = 0.2f;
    private float glassFillCurrent = 0f;
    private float glassFillEnd = 0.8f; 
    private float fillingSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        liquidFlow.SetActive(false);

        GetAllMaterials();
        SetDefaultFillValues();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the tilting angle based on the x- and y-coordinates
        float tiltAngle = CalculateTiltAngle(bottle.transform);

        if (tiltAngle < tiltThreshold || tiltAngle > tiltThreshold2)
        {
            // Setup Water Emitter to the right direction
            Vector3 horizontalTilt = new Vector3(transform.up.x, 0f, transform.up.z);
            // Making sure that there is no division or multiplication by 0
            if (horizontalTilt.sqrMagnitude > 0.001f)
            {
                // Rotate the water emitter so that it points in the horizontal tilt direction.
                liquidFlow.transform.rotation = Quaternion.LookRotation(horizontalTilt);
            }

            liquidFlow.SetActive(true);
            audiosource.Play();

            // Change level of "Fill" in WhiskeyBottle
            if (liquidInBottleMaterial.HasProperty("_Fill"))
            {
                ChangeBottleFill();                
            }

            if (liquidInGlassMaterial.HasProperty("_Fill"))
			{
                ChangeGlassFill();
			}
        }
        else
        {
            audiosource.Stop();
            liquidFlow.SetActive(false);
        }
    }

    void GetAllMaterials()
	{
        liquidInBottleMaterial = liquidInBottle.GetComponent<Renderer>().material;
        liquidInGlassMaterial = liquidInGlass.GetComponent<Renderer>().material;
    }

    void SetDefaultFillValues()
	{
        if (liquidInBottleMaterial.HasProperty("_Fill"))
        {
            liquidInBottleMaterial.SetFloat("_Fill", bottleFillCurrent);
        }

        if (liquidInGlassMaterial.HasProperty("_Fill"))
		{
            liquidInGlassMaterial.SetFloat("_Fill", glassFillCurrent);
		}
    }

    float CalculateTiltAngle(Transform bottleTransform)
    {
        // Get the "Up"-vector of the bottle
        Vector3 bottleUp = bottleTransform.up;
        
        // Calculate the angle between the global "Up"-vector and the "Up"-vector of the bottle
        float angle = Vector3.Angle(bottleUp, Vector3.up);

        return angle;
    }

    void ChangeBottleFill()
	{
        if (bottleFillCurrent > bottleFillEnd)
        {
            bottleFillCurrent -= fillingSpeed * Time.deltaTime;
            liquidInBottleMaterial.SetFloat("_Fill", bottleFillCurrent);
        }
    }

    void ChangeGlassFill()
	{
        if (glassFillCurrent < glassFillEnd)
        {
            glassFillCurrent += fillingSpeed * Time.deltaTime;
            liquidInGlassMaterial.SetFloat("_Fill", glassFillCurrent);
        }
    }
}
