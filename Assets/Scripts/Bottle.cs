using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Transform origin;
    public GameObject liquid;

    private bool isFlowing;
    private Liquid currentLiquid;

    private void Update() {
        CheckFlow();
    }

    private void CheckFlow()
    {
        float zAngle = transform.localEulerAngles.z;
        bool flowCheck = zAngle > 80 && zAngle < 280;

        if (isFlowing == flowCheck)
            return;

        isFlowing = flowCheck;
        if (isFlowing)
            StartFlow();
        else
            EndFlow();
    }

    private void StartFlow()
    {
        currentLiquid = CreateLiquid();
        currentLiquid.Begin();
    }

    private void EndFlow()
    {
        currentLiquid.End();
        currentLiquid = null;
    }

    private float FlowAngle()
    {
        return transform.up.y * Mathf.Rad2Deg;
    }

    private Liquid CreateLiquid()
    {
        GameObject gameObject = Instantiate(liquid, origin.position, Quaternion.identity, transform);
        return gameObject.GetComponent<Liquid>();
    }
}
