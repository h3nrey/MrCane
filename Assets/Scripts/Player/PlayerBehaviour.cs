using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PlayerBehaviour : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float moveSpeedOnAir;
    [SerializeField] private float moveSpeedOnGround;
    [SerializeField] private float accelRate;

    [Header("Jump")]
    [SerializeField] private float jumpPower;

    [SerializeField] private float maxJumpPower;
    [SerializeField] private float jumpCutMultiplier;
    [SerializeField] private float coyouteTimer;
    [SerializeField] private float timeOutsideGround;

    [Header("Gravity")]
    [SerializeField] private float baseGravityScale;

    [SerializeField] private float fallGravityScale;

    [Header("Ground Point")]
    [SerializeField] private Transform groundPoint;

    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private int groundLayer;

    [Header("Cane")]
    [SerializeField] private GameObject cane;

    [SerializeField] private float caneJumpForce;
    [SerializeField] private bool holdingCane;

    private Vector2 moveInput;
    private Vector2 vel;

    // flags

    private bool isGrounded;
    private bool isJumping;
    private bool canJump;

    // Components
    private Rigidbody2D rb;

    // Events
    private delegate void TouchGround();

    private event TouchGround touchGround;

    // Start is called before the first frame update
    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = baseGravityScale;

        touchGround += DoCaneRicochet;
    }

    private void Update() {
        moveInput.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) {
            Jump();
        }

        if (Input.GetButtonUp("Jump")) {
            ApplyJumpCut();
        }

        if (Input.GetButton("Cane")) {
            ActiveCane();
        }

        if (Input.GetButtonUp("Cane")) {
            HideCane();
        }
    }

    private void FixedUpdate() {
        vel = rb.velocity;

        //CheckIfIsOnAir();
        Move();
        CheckIfIsFalling();
        CheckIfIsGrounded();
        CheckIfCanJump();
    }

    #region Movement

    private void Move() {
        rb.velocity = new Vector2(moveSpeed * moveInput.x * Time.fixedDeltaTime, vel.y);
    }

    private void CheckIfIsOnAir() {
        if (vel.y > 0 && !isGrounded && vel.x == 0) {
            moveSpeed = moveSpeedOnAir;
        }
        else if (isGrounded) {
            moveSpeed = moveSpeedOnGround;
        }
    }

    #endregion Movement

    #region Jump

    private void Jump() {
        if (canJump) {
            isJumping = true;
            rb.velocity += new Vector2(vel.x, jumpPower * Time.fixedDeltaTime);
        }
    }

    private void CheckIfCanJump() {
        if ((isGrounded || timeOutsideGround > Time.time) && isJumping == false && !holdingCane) {
            canJump = true;
        }
        else {
            canJump = false;
        }
    }

    private void ApplyJumpCut() {
        if (vel.y > 0.1f && isJumping) {
            rb.velocity = new Vector2(vel.x, vel.y * jumpCutMultiplier);
        }
    }

    #endregion Jump

    private void CheckIfIsFalling() {
        if (vel.y < -0.1f) {
            rb.gravityScale = fallGravityScale;
        }
        else {
            rb.gravityScale = baseGravityScale;
        }
    }

    private void CheckIfIsGrounded() {
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);
    }

    #region Cane

    private void ActiveCane() {
        if (isGrounded) return;
        holdingCane = true;
        cane.SetActive(true);
    }

    private void HideCane() {
        holdingCane = false;
        cane.SetActive(false);
    }

    private void DoCaneRicochet() {
        if (holdingCane) {
            //rb.velocity = new Vector2(vel.x, (vel.y + caneJumpForce) * Time.fixedDeltaTime);
            Vector3 caneScale = cane.transform.localScale;
            cane.transform.localScale = new Vector3(caneScale.x, caneScale.y / 2, caneScale.z);
            Coroutines.DoAfter(() =>
                rb.velocity = new Vector2(vel.x, (vel.y + caneJumpForce) * Time.fixedDeltaTime),
                0.05f,
                this
            );
            Coroutines.DoAfter(() => cane.transform.localScale = caneScale, 0.2f, this);
        }
    }

    #endregion Cane

    #region Collision

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject otherObj = other.gameObject;

        if (otherObj.layer == groundLayer) {
            isJumping = false;
            touchGround();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.layer == groundLayer) {
            timeOutsideGround = coyouteTimer + Time.time;
        }
    }

    #endregion Collision

    #region Gizmos

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }

    #endregion Gizmos
}