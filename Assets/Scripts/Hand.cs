﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    Transform hand;
    Transform spine;
    Transform target;
    Transform readyPos;
    public float distanceToTarget;
    public float distanceThreshold;
    public float rotationSpeed;
    public float liftHandSpeed;
    public float swatSpeed;
    public bool swat;
    public bool coolDown;
    public bool reachedTarget;
    Vector3 swatPos;
    float t;
    Vector3 startPos;
    Vector3 overShoot;

    Vector3 lastFramePos;
    // Start is called before the first frame update
    void Start()
    {
        hand = gameObject.GetComponent<Transform>();
        spine = GameObject.FindObjectOfType<Spine>().GetComponent<Transform>();
        target = GameObject.FindObjectOfType<Target>().GetComponent<Transform>();
        readyPos = GameObject.FindObjectOfType<readyPos>().GetComponent<Transform>();
        lastFramePos = hand.position;
    }

    // Update is called once per frame
    void Update()
    {
       
        distanceToTarget = Vector3.Distance(target.position, spine.position);
        if(distanceToTarget< distanceThreshold)
        {
            // The step size is equal to speed times frame time.
            var step = rotationSpeed * Time.unscaledDeltaTime;

            Quaternion newRotation = Quaternion.LookRotation(spine.position - target.position);
            // Rotate our transform a step closer to the target's.
            spine.rotation = Quaternion.RotateTowards(spine.rotation, newRotation, step);
            if(!coolDown)
            LiftArm();
        }
        if(swat)
        {
            //Vector3 targetDir = (swatPos - hand.position).normalized;

            // hand.position = hand.position + targetDir * swatSpeed * Time.deltaTime;
            hand.right = -(hand.position - lastFramePos);

           
                t += swatSpeed * Time.unscaledDeltaTime;

            if (!reachedTarget)
                hand.position = BezierCurve(startPos, swatPos, t * t);
            else
                hand.position = BezierCurve2(swatPos, overShoot, 2*t);
                if (Vector3.Distance(hand.position, swatPos) < 0.1f && !reachedTarget)
                {

                // hand.position = swatPos;
                   // startPos = swatPos;
                    //swatPos = overShoot;
                reachedTarget = true;
                t = 0;
               
                }
            
        
            StartCoroutine(SwatCoroutine());

        }

        lastFramePos = hand.position;
        
    }

    Vector3 BezierCurve(Vector3 a, Vector3 d , float t) // kan optimereres
    {
        
        Vector3 b = a + 1f * Vector3.up;
        Vector3 c = d + +1f * Vector3.up;

        Vector3 m = Vector3.Lerp(a, b, t);
        Vector3 n = Vector3.Lerp(b, c, t);
        Vector3 o = Vector3.Lerp(c, d, t);
        Vector3 p = Vector3.Lerp(m, n, t);
        Vector3 e = Vector3.Lerp(n, o, t);
        Vector3 pointOnCurve = Vector3.Lerp(p, e, t);

        return pointOnCurve;

    }
    Vector3 BezierCurve2(Vector3 a, Vector3 d, float t) // kan optimereres
    {

        Vector3 b = a + -0.2f * Vector3.up;
        Vector3 c = d + +-0.1f * Vector3.up;

        Vector3 m = Vector3.Lerp(a, b, t);
        Vector3 n = Vector3.Lerp(b, c, t);
        Vector3 o = Vector3.Lerp(c, d, t);
        Vector3 p = Vector3.Lerp(m, n, t);
        Vector3 e = Vector3.Lerp(n, o, t);
        Vector3 pointOnCurve = Vector3.Lerp(p, e, t);

        return pointOnCurve;

    }
    void LiftArm()
    {
       
        Vector3 readyDirNorm =(readyPos.position - hand.position).normalized;

        hand.position = hand.position + readyDirNorm * liftHandSpeed * Time.unscaledDeltaTime;

        if(Vector3.Distance(hand.position, readyPos.position) < 0.05f && !swat) //magic number indtil videre
        {
            swat = true;
            swatPos = target.position;
            startPos = hand.position;
            overShoot = target.position + (target.position -hand.position).normalized; 
            t = 0;
        }
    }
    IEnumerator SwatCoroutine()
    {
        
        float scale = Time.timeScale;
        yield return new WaitForSeconds(2f*scale); //magic number
        swat = false;
        reachedTarget = false;
        coolDown = true;
       
        yield return new WaitForSeconds(2f*scale);
        coolDown = false;
        
        
    }
 
}