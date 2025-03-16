using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidFlow : MonoBehaviour
{
    [SerializeField] private GameObject bottle;
    [SerializeField] private GameObject glass;
    [SerializeField] private GameObject liquidFlow; // == ObiSolver
    //[SerializeField] private GameObject topOfBottle;
    [SerializeField] private GameObject liquidInBottle;
    [SerializeField] private GameObject liquidInGlass;

    private Quaternion fixedRotation = Quaternion.identity; // Perfect alignment with the world axes
    private float tiltThreshold = 45f;
    private float tiltThreshold2 = 135f;
    private Material liquidInBottleMaterial;
    private Material liquidInGlassMaterial;

    private float bottleFillCurrent = 0.9f;
    private float bottleFillEnd = 0.2f;
    private float glassFillCurrent = 0f;
    private float glassFillEnd = 0.95f; 
    private float fillingSpeed = 0.05f;

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
        AdjustLiquidFlowAngle(bottle.transform);

        if (tiltAngle < tiltThreshold || tiltAngle > tiltThreshold2)
        {
            liquidFlow.transform.rotation = fixedRotation;
            //liquidFlow.transform.position = topOfBottle.transform.position;
            liquidFlow.SetActive(true);

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

    void AdjustLiquidFlowAngle(Transform bottleTransform)
	{
        float yBottleRotation = bottleTransform.eulerAngles.y;
        liquidFlow.transform.eulerAngles = new Vector3(liquidFlow.transform.eulerAngles.x, yBottleRotation, liquidFlow.transform.eulerAngles.z);
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
