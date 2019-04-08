using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacolController : AbstractObiectController
{

    public float mRotationalSpeed;

    public override void collisionAction(Collision2D col)
    {
        Debug.Log("Game Over my friend.");
    }

    protected override void StartMovement()
    {
        StartCoroutine(moveLinear(mWaitAfterTarget));   // Incepe miscarea

    }
    void Update()
    {
        // Rotate the object around its local Z axis
        transform.Rotate(new Vector3(0,0,1) * Time.deltaTime * mRotationalSpeed);
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
}
