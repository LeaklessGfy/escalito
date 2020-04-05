using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Transform origin;
    private bool isFlowing;

    private void Update()
    {
        float zAngle = transform.localEulerAngles.z;
        bool shouldFlow = zAngle > 80 && zAngle < 280;

        if (isFlowing == shouldFlow)
        {
            return;
        }

        isFlowing = shouldFlow;
        if (isFlowing)
        {
            origin.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            origin.GetComponent<ParticleSystem>().Stop();
        }
    }
}
