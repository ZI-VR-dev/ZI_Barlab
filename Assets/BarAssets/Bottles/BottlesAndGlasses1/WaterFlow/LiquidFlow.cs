using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    

    private bool isTilting = false;
    private float tiltThreshold = -70f;
    private float tiltThreshold2 = 70f;
    private Material liquidInBottleMaterial;
    private Material liquidInGlassMaterial;
    private Material foamMaterial;

    private float bottleFillCurrent = 0.997f;
    private float bottleFillEnd = 0.4f;
    private float glassFillCurrent = 0f;
    private float glassFillEnd;
    private float fillingSpeed = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        glassFillEnd = liquidInGlass.GetComponent<Renderer>().material.GetFloat("_Fill");
        //liquidInGlass = glassParentObject.transform.Find("Liquid").gameObject;
        liquidFlow_ObiSolver.SetActive(false);
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
            if (isTilting == false) 
            {
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

            // Change level of "Fill" in Bottles
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
            if (isTilting == false)
            {
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

    // Change the level of shader in the bottle
    void ChangeBottleFill()
	{
        if (bottleFillCurrent > bottleFillEnd)
        {
            bottleFillCurrent -= (fillingSpeed/10) * Time.deltaTime;
            liquidInBottleMaterial.SetFloat("_Fill", bottleFillCurrent);
        }
    }

	// Change the level of shader in the glass
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
