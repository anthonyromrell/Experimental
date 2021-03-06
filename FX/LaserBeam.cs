﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LaserBeam : MonoBehaviour
{
    public GameObject beamStart;
    public GameObject beamEnd;
    public GameObject beam;
    private LineRenderer line;
    public Transform endPoint;
    [FormerlySerializedAs("EnableEvent")] public UnityEvent enableEvent;
    [FormerlySerializedAs("DisableEvent")] public UnityEvent disableEvent;
    [FormerlySerializedAs("TriggerEnterEvent")] public UnityEvent triggerEnterEvent;
    [FormerlySerializedAs("DirectionBool")] public BoolData directionBool;
    
    [Header("Adjustable Variables")]
    public float beamEndOffset = 1f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
	public float textureLengthScale = 3; //Length of the beam texture
    private float holdTime = 1.0f;
    private WaitForFixedUpdateObj wfsObj;

   
    private void OnEnable()
    {
        line = beam.GetComponent<LineRenderer>();
        Invoke(nameof(Disable), holdTime);
        enableEvent.Invoke();
        
        wfsObj = ScriptableObject.CreateInstance<WaitForFixedUpdateObj>();
        
        if (directionBool.value)
        {
           //transform.rotation = Quaternion.Euler(0,0,0);
        }
        else
        {
          // transform.rotation = Quaternion.Euler(0,180,0);
        }
        StartCoroutine(ShootBeamInDir()) ;
        enableEvent.Invoke();
    }

    private IEnumerator ShootBeamInDir()
    {

        while (true)
        {
            var start = transform.position;
            var dir = endPoint.position;
            line.SetPosition(0, start);
            beamStart.transform.position = start;
            var end = Physics.Raycast(start, dir, out var hit) ? hit.point : dir;
            beamEnd.transform.position = end;
            line.SetPosition(1, end);
//
//            beamStart.transform.LookAt(beamEnd.transform.position);
//            beamEnd.transform.LookAt(beamStart.transform.position);
//
            var distance = Vector3.Distance(start, end);
            var sharedMaterial = line.sharedMaterial;
            sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
            sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
            yield return wfsObj;
        }
    }
    
    private void Disable()
    {
        disableEvent.Invoke();
        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
       StopAllCoroutines();
    }
}