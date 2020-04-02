using UnityEngine;

public class Character : MonoBehaviour
{
    public Animator animator;

    public int state {
        get; set;
    }

    private void Update()
    {
        animator.SetInteger("State", state);
    }
}
