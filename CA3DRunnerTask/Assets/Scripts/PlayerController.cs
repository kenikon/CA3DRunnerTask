using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dreamteck.Splines;
using DG.Tweening;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    float Horizontal;
    [SerializeField] float HorizontalMultiplier = 1.5f;
    [SerializeField] float JumpForce = 5f;
    Rigidbody rb;
    Animator anim;
    bool isOnRoad = true;
    bool isHitObstacle = false;
    bool isGroundActive = true;
    SplineFollower follower;
    Collider[] colliders;
    [SerializeField] Transform detectTransform;
    [SerializeField] float DetectionRange = 1;
    [SerializeField] LayerMask layer;
    [SerializeField] Transform holdTransform;
    [SerializeField] int ItemCount = 0;
    [SerializeField] float ItemDistanceBetween = 0.5f;
    float NextDropTime;
    [SerializeField] float DropRate = 1f;
    [SerializeField] float DropSecond = 1f;
    [SerializeField] List<GameObject> CollectedItems;
    [SerializeField] Transform DropAreaTransform;
    int DropCount = 0;
    [SerializeField] float DropDistanceBetween = 1f;
    CinemachineVirtualCamera vCam;
    CinemachineTransposer TransPoser;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        follower = GetComponentInParent<SplineFollower>();
        vCam = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<CinemachineVirtualCamera>();
        TransPoser = vCam.GetCinemachineComponent<CinemachineTransposer>();
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
                hit.isTrigger = true;

                var seq = DOTween.Sequence();
                seq.Append(hit.transform.DOLocalJump(new Vector3(0, ItemCount*ItemDistanceBetween), 2, 1, 0.3f))
                .Join(hit.transform.DOScale(1.25f, 0.1f))
                .Insert(0.1f,hit.transform.DOScale(0.3f,0.2f));
                seq.AppendCallback(() => {
                    hit.transform.localRotation = Quaternion.Euler(0,0,0);
                });

                CollectedItems.Add(hit.gameObject);
                ItemCount++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isOnRoad) {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            isOnRoad = false;
        }
    }

    private void FixedUpdate() {
        Horizontal = Input.GetAxis("Horizontal");
        transform.localPosition += new Vector3(Horizontal * 5, 0, 0) * Time.fixedDeltaTime * HorizontalMultiplier;
    }

    private void OnTriggerStay(Collider other) {
        // TransPoser.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
        // transform.Rotate(0, 180, 0);
        if (other.transform.CompareTag("DropArea")) {

            isGroundActive = false;

        if (Time.time >= NextDropTime) {
            // if (CollectedItems.Count <= 0) return;
            if (CollectedItems.Count > 0) {
                GameObject go = CollectedItems[CollectedItems.Count - 1];
                go.transform.GetComponent<Collider>().isTrigger = false;
                go.transform.parent = DropAreaTransform;
                var Seq = DOTween.Sequence();
                Seq.Append(go.transform.DOJump(DropAreaTransform.position + new Vector3(0, DropCount * DropDistanceBetween), 2, 1, 0.3f)
                    .Join(go.transform.DOScale(1.5f, 0.1f))
                    .Insert(0.1f, go.transform.DOScale(1f, 0.2f))
                    .AppendCallback(() => {
                        go.transform.rotation = Quaternion.Euler(0,0,0);
                    }));

                DropCount++;
                CollectedItems.Remove(go);
                ItemCount--;

                NextDropTime = Time.time + DropSecond / DropRate;
                if (ItemCount == 0) {
                    // TransPoser.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
                    // vCam.transform.parent = null;
                    transform.Rotate(0, 180, 0);
                }
            }
        }

        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Road")) {
            isOnRoad = true;
        }

        if (other.transform.CompareTag("Ground") && isGroundActive) {
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

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("FinishLine")) {
            anim.SetTrigger("Dance");
            follower.followSpeed = 0;
        }
    }

    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
