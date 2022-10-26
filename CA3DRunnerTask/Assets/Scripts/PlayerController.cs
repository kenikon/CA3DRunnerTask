using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dreamteck.Splines;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    float Horizontal;
    [SerializeField] float HorizontalMultiplier = 1.5f;
    [SerializeField] float JumpForce = 5f;
    Rigidbody rb;
    Animator anim;
    bool isGround = true;
    bool isHitObstacle = false;
    SplineFollower follower;
    Collider[] colliders;
    [SerializeField] Transform detectTransform;
    [SerializeField] float DetectionRange = 1;
    [SerializeField] LayerMask layer;
    [SerializeField] Transform holdTransform;
    [SerializeField] int ItemCount = 0;
    [SerializeField] float ItemDistanceBetween = 0.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        follower = GetComponentInParent<SplineFollower>();
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0.6f, 0.0f, 0.0f, 0.5f);
        Gizmos.DrawSphere(detectTransform.position,DetectionRange);
    }
    // Update is called once per frame
    void Update()
    {
        colliders = Physics.OverlapSphere(detectTransform.position, DetectionRange, layer);

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Collectable")) {
                Debug.Log(hit.name);
                hit.tag = "Collected";
                hit.transform.parent = holdTransform;

                var seq = DOTween.Sequence();
                seq.Append(hit.transform.DOLocalJump(new Vector3(0, ItemCount*ItemDistanceBetween), 2, 1, 0.3f))
                .Join(hit.transform.DOScale(1.25f, 0.1f))
                .Insert(0.1f,hit.transform.DOScale(0.3f,0.2f));
                seq.AppendCallback(() => {
                    hit.transform.localRotation = Quaternion.Euler(0,0,0);
                });

                ItemCount++;
            }
        }

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
