using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidFlow : MonoBehaviour
{
    [SerializeField] private GameObject bottle;
    [SerializeField] private GameObject glass;
    [SerializeField] private GameObject liquidFlow;
    [SerializeField] private GameObject topOfBottle;
    [SerializeField] private GameObject liquidInBottle;
    [SerializeField] private GameObject liquidInGlass;

    private Quaternion fixedRotation = Quaternion.identity; // Perfect alignment with the world axes
    private float tiltThreshold = 45f;
    private float tiltThreshold2 = 135f;
    private Material liquidInBottleMaterial;
    private Material liquidInGlassMaterial;

    private float bottleFillCurrent = 0.54f;
    private float bottleFillEnd = 0.5f;
    private float glassFillCurrent;
    private float glassFillEnd; 
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

        if (tiltAngle < tiltThreshold || tiltAngle > tiltThreshold2)
        {
            liquidFlow.transform.rotation = fixedRotation;
            liquidFlow.transform.position = topOfBottle.transform.position;
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
            // This switch is currently necessary, as the shader not setting properly to 0f.
            // Some glass see a fill up at 0.4, while others at 0f.
			switch (liquidInGlass.tag)
			{
                case "WhiskeyInGlass":
                    glassFillCurrent = 0.38f;
                    glassFillEnd = 0.5f;
                    break;
                case "CognacInGlass":
                    glassFillCurrent = 0.46f;
                    glassFillEnd = 0.55f;
                    break;
                case "ColaInGlass":
                    glassFillCurrent = 0.22f;
                    glassFillEnd = 0.65f;
                    break;
                case "ShotInGlass":
                    glassFillCurrent = 0.405f;
                    glassFillEnd = 0.6f;
                    break;
                case "WineInGlass":
                    glassFillCurrent = 0.475f;
                    glassFillEnd = 0.8f;
                    break;
                case "BeerInGlass":
                    glassFillCurrent = 0.45f;
                    glassFillEnd = 0.53f;
                    break;
                default:
                    glassFillCurrent = 0f;
                    glassFillEnd = 0.9f;
                    break;
			}
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
