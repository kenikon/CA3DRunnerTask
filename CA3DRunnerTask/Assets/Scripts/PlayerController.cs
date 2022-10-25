using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float Horizontal;
    [Header("Movement")]
    [SerializeField] float HorizontalMultiplier = 1.5f;
    [SerializeField] float JumpForce = 5f;
    Rigidbody rb;
    Animator anim;
    bool isGround = true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround) {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            isGround = false;
        }
    }

    private void FixedUpdate() {
        Horizontal = Input.GetAxis("Horizontal");
        transform.localPosition += new Vector3(Horizontal * 5, 0, 0) * Time.fixedDeltaTime * HorizontalMultiplier;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Ground")) {
            isGround = true;
        }
    }
}
