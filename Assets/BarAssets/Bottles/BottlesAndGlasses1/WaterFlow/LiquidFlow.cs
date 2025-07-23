using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidFlow : MonoBehaviour
{
    [SerializeField] private GameObject bottleParentObject;
    [SerializeField] private GameObject glassParentObject;
    [SerializeField] private GameObject liquidFlow_ObiSolver; // == ObiSolver
    [SerializeField] private GameObject liquidInBottle;
    [SerializeField] private GameObject liquidInGlass;
    [SerializeField] private GameObject foam;
    [SerializeField] private AudioSource audiosource;
    [SerializeField] private bool isTilting = false;
    [SerializeField] private bool isNotTilting = false;

    private float tiltThreshold = -45f;
    private float tiltThreshold2 = 45f;
    private Material liquidInBottleMaterial;
    private Material liquidInGlassMaterial;
    private Material foamMaterial;

    private float bottleFillCurrent = 0.9f;
    private float bottleFillEnd = 0.2f;
    private float glassFillCurrent = 0f;
    private float glassFillEnd = 0.85f; 
    private float fillingSpeed = 0.1f;
    //private bool isTilting = false;
    //private bool isNotTilting = false;

    // Start is called before the first frame update
    void Start()
    {
		liquidFlow_ObiSolver.SetActive(false);
        //audiosource.Play();
        GetAllMaterials();
        SetDefaultFillValues();
        SetColorOfLiquidInGlass();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the tilting angle based on the x- and y-coordinates
        float tiltAngle = CalculateTiltAngle(bottleParentObject.transform);

        if (tiltAngle < tiltThreshold || tiltAngle > tiltThreshold2)
        {
            // Trigger audiosource.Play() only once, when bottle is tilted for the first time
            isNotTilting = false;

            if (isTilting == false) 
            {
                Debug.Log("Bottle is tilting");
                isTilting = true;
                audiosource.Play(); 
            }

            // Setup Water Emitter to the right direction
            Vector3 horizontalTilt = new Vector3(transform.up.x, 0f, transform.up.z);
            // Making sure that there is no division or multiplication by 0
            if (horizontalTilt.sqrMagnitude > 0.001f)
            {
				// Rotate the water emitter so that it points in the horizontal tilt direction.
				liquidFlow_ObiSolver.transform.rotation = Quaternion.LookRotation(horizontalTilt);
            }

			liquidFlow_ObiSolver.SetActive(true);
            //audiosource.Play();

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
            // Trigger audiosource.Stop() only once, when bottle is not tilted anymore for the first time
            isTilting = false;
            if (isNotTilting == false) 
            {
                Debug.Log("Bottle is not tilting");
                isNotTilting = true;
                audiosource.Stop();
            }

            liquidFlow_ObiSolver.SetActive(false);
        }
    }

    void GetAllMaterials()
	{
        liquidInBottleMaterial = liquidInBottle.GetComponent<Renderer>().material;
        liquidInGlassMaterial = liquidInGlass.GetComponent<Renderer>().material;
        if (foam != null)
		{
            foamMaterial = foam.GetComponent<Renderer>().material;
        }
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

        if (foam != null && foamMaterial.HasProperty("_Fill"))
        {
            foamMaterial.SetFloat("_Fill", glassFillCurrent);
        }
    }

    void SetColorOfLiquidInGlass()
	{
        liquidInGlassMaterial.SetColor("_SideColor", liquidInBottleMaterial.GetColor("_SideColor"));
        liquidInGlassMaterial.SetColor("_TopColor", liquidInBottleMaterial.GetColor("_TopColor"));
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
            if (foam == null)
			{
                liquidInGlassMaterial.SetFloat("_Fill", glassFillCurrent);
            }
            else if (foam != null)
			{
                if (glassFillCurrent <= 0.8)
                {
                    liquidInGlassMaterial.SetFloat("_Fill", glassFillCurrent);
                }
                else
                {
                    liquidInGlassMaterial.SetFloat("_Fill", 0.8f);
                    foamMaterial.SetFloat("_Fill", glassFillCurrent);
                }
            }
        }
    }
}
