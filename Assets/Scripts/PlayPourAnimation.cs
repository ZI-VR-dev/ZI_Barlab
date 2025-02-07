using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayPourAnimation : MonoBehaviour
{
    private PlayableDirector PD;
    public MeshRenderer MR;
    public MeshRenderer MR_02;

    // Start is called before the first frame update
    void Start()
    {
        PD = GetComponent<PlayableDirector>();
        MR.enabled = false;
        MR_02.enabled = false;  
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(enableMeshes());

            PD.Play();

            StartCoroutine(disableMeshes());
        }
    }

    private IEnumerator disableMeshes()
    {
        yield return new WaitForSeconds(4.35f);
        MR.enabled = false;
        MR_02.enabled = false;
    }

    private IEnumerator enableMeshes()
    {
        yield return new WaitForSeconds(1.475f);
        MR.enabled = true;
        MR_02.enabled = true;
    }
}
