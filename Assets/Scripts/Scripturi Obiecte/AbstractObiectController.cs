using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractObiectController : MonoBehaviour {

    public float mDuratieTravers;
    public float mDistance;
    public float mWaitAfterTarget;
    public Vector2 mDirection;
    public Vector2 mTargetPoint;

    public Vector2 mPozitieStart;        // Pozitia initiala a obstacolului

    
    protected virtual void Start()
    {
        mDirection = mDirection.normalized;
        mPozitieStart = transform.position;
        ComputeTargetPoint(mPozitieStart,mDirection,mDistance);
        StartMovement();

    }

    protected virtual void StartMovement()
    {
        // do nothing...
    }


    protected IEnumerator moveLinear(float waitDuration)
    {
        // Loops each cycles
        while (Application.isPlaying)
        {
            // First step, travel from A to B
            float counter = 0f;
            while (counter < mDuratieTravers)
            {
                transform.position = Vector3.Lerp(mPozitieStart, mTargetPoint, counter / mDuratieTravers);
                counter += Time.deltaTime;
                yield return null;
            }

            // Make sure you're exactly at B, in case the counter 
            // wasn't precisely equal to travelDuration at the end
            transform.position = mTargetPoint;

            // Second step, wait
            yield return new WaitForSeconds(waitDuration);

            // Third step, travel back from B to A
            counter = 0f;
            while (counter < mDuratieTravers)
            {
                transform.position = Vector3.Lerp(mTargetPoint, mPozitieStart, counter / mDuratieTravers);
                counter += Time.deltaTime;
                yield return null;
            }

            transform.position = mPozitieStart;

            // Finally, wait
            yield return new WaitForSeconds(waitDuration);
        }

    }

    public void SetAndComputeProperties(Vector2 newDirection,Vector2 newStartPosition,float duration,float newDistance)
    {
        this.mDirection = newDirection;
        this.mPozitieStart = newStartPosition;
        this.mDistance = newDistance;
        this.mDuratieTravers = duration;

        this.mTargetPoint = ComputeTargetPoint(mPozitieStart, mDirection, mDistance);
    }
    public static Vector2 ComputeTargetPoint(Vector2 from,Vector2 direction, float distance)
    {
        return  from + direction * distance;
    }

    abstract public void collisionAction(Collision2D col);         // Trebuie supraincarcata in scripturile speciala pentru miscari
}
