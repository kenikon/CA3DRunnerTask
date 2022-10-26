using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dreamteck.Splines;

public class PlayerController : MonoBehaviour
{
    float Horizontal;
    [Header("Movement")]
    [SerializeField] float HorizontalMultiplier = 1.5f;
    [SerializeField] float JumpForce = 5f;
    Rigidbody rb;
    Animator anim;
    bool isGround = true;
    bool isHitObstacle = false;
    SplineFollower follower;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        follower = GetComponentInParent<SplineFollower>();
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
        if (other.transform.CompareTag("Road")) {
            isGround = true;
        }

        if (other.transform.CompareTag("Ground")) {
            anim.SetTrigger("FallOnGround");
            follower.followSpeed = 0;
            Invoke("Restart", 3.5f);
        }

        if (other.transform.CompareTag("Obstacle") && !isHitObstacle) {
            isHitObstacle = true;
            anim.SetTrigger("FallOnRoad");
            follower.followSpeed = 0;
            Invoke("Restart", 3.5f);
        }
    }

    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
