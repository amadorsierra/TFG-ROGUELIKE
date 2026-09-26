using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        am.SetFloat("Horizontal", pm.moveDirection.x);
        am.SetFloat("Vertical", pm.moveDirection.y);
        am.SetFloat("Speed", pm.moveDirection.magnitude);

    }
}
