using System.Collections;
using UnityEngine;
using UnityEngine.N3DS;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public float moveForce = 10f;
    public float maxSpeed = 8f;

    [Header("Physics Damping")]
    public float deceleration = 5f;

    [Header("Camera References")]
    public Transform cameraTransform;

    [Header("Character References")]
    public Animator characterAnimator;

    [Header("Collectible Mass")]
    public float massPerCollectible = 0.5f;
    public float massIncreaseDuration = 0.5f;

    [Header("Katamari Growth")]
    public float scalePerCollectible = 0.05f;
    public float scaleGrowthDuration = 0.4f;
    public float maxScale = 5f;

    [Header("Katamari Force Scaling")]
    public float baseScale = 1f;              // Escala inicial del player
    public float minForceMultiplier = 0.4f;   // Fuerza mínima cuando es muy grande
    public float maxForceMultiplier = 1.2f;   // Fuerza máxima cuando es pequeño



    private Rigidbody rb;

    // Animator hashes
    private int animMoving;
    private int animMoveSpeed;
    private int animRight;
    private int animLeft;
    private int animLeftAround;
    private int animRightAround;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        animMoving = Animator.StringToHash("Moving");
        animMoveSpeed = Animator.StringToHash("MoveSpeed");
        animRight = Animator.StringToHash("Right");
        animLeft = Animator.StringToHash("Left");
        animLeftAround = Animator.StringToHash("LeftAround");
        animRightAround = Animator.StringToHash("RightAround");
    }

    void FixedUpdate()
    {
        float moveX = GamePad.CirclePad.x;
        float moveZ = GamePad.CirclePad.y;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 movement = (camForward * moveZ) + (camRight * moveX);

#if UNITY_EDITOR
        float editorMoveX = Input.GetAxis("Horizontal");
        float editorMoveZ = Input.GetAxis("Vertical");

        Vector3 movementEditor = (camForward * editorMoveZ) + (camRight * editorMoveX);

        if (rb.velocity.magnitude < maxSpeed)
            rb.AddForce(movementEditor * moveForce, ForceMode.Force);

        moveX = editorMoveX;
#endif

        if (rb.velocity.magnitude < maxSpeed)
            rb.AddForce(movement * moveForce, ForceMode.Force);

        // Deceleración
        if (moveX == 0 && moveZ == 0)
        {
            Vector3 vel = rb.velocity;
            vel.y = 0;

            Vector3 decel = vel.normalized * deceleration * Time.fixedDeltaTime;
            if (decel.magnitude > vel.magnitude)
                decel = vel;

            rb.velocity -= new Vector3(decel.x, 0, decel.z);
        }

        UpdateAnimator(moveX);
        UpdateAroundAnimator();
    }

    void UpdateAnimator(float moveX)
    {
        bool moving = IsMoving();

        characterAnimator.SetBool(animMoving, moving);
        characterAnimator.SetFloat(animMoveSpeed, GetMovementSpeed());

        bool right = false;
        bool left = false;

        if (moving)
        {
            if (moveX > 0.1f) right = true;
            else if (moveX < -0.1f) left = true;
        }

        characterAnimator.SetBool(animRight, right);
        characterAnimator.SetBool(animLeft, left);
    }

    // 🔥 Animaciones Around (L / R o Q / E)
    void UpdateAroundAnimator()
    {
        bool moving = IsMoving();

        bool leftAround = false;
        bool rightAround = false;

#if UNITY_EDITOR

            if (Input.GetKey(KeyCode.Q))
                leftAround = true;
            else if (Input.GetKey(KeyCode.E))
                rightAround = true;

#else

            if (GamePad.GetButton(GamePad.Button.L))
                leftAround = true;
            else if (GamePad.GetButton(GamePad.Button.R))
                rightAround = true;
        
#endif

        characterAnimator.SetBool(animLeftAround, leftAround);
        characterAnimator.SetBool(animRightAround, rightAround);
    }

    public bool IsMoving(float threshold = 0.1f)
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude > threshold;
    }

    public float GetMovementSpeed()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        float realSpeed = horizontalVelocity.magnitude;
        return Mathf.Clamp((realSpeed / maxSpeed) * 3f, 0f, 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Collectible"))
            return;

        GameObject collectible = collision.gameObject;

        collectible.transform.SetParent(transform);

        Collider col = collectible.GetComponent<Collider>();
        if (col != null)
            Destroy(col);

        Rigidbody rbCollectible = collectible.GetComponent<Rigidbody>();
        if (rbCollectible != null)
            Destroy(rbCollectible);

        // 🔥 Crecimiento Katamari
        StartCoroutine(GrowPlayer(scalePerCollectible));

        // 🔥 Aumentar masa (si ya lo tienes)
        StartCoroutine(IncreasePlayerMass(massPerCollectible));
    }

    IEnumerator GrowPlayer(float addedScale)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale + Vector3.one * addedScale;

        // Limitar tamaño máximo
        if (targetScale.x > maxScale)
            targetScale = Vector3.one * maxScale;

        float elapsed = 0f;

        while (elapsed < scaleGrowthDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / scaleGrowthDuration);
            yield return null;
        }

        transform.localScale = targetScale;
       
    }


    IEnumerator IncreasePlayerMass(float addedMass)
    {
        float startMass = rb.mass;
        float targetMass = startMass + addedMass;

        float elapsed = 0f;

        while (elapsed < massIncreaseDuration)
        {
            elapsed += Time.deltaTime;
            rb.mass = Mathf.Lerp(startMass, targetMass, elapsed / massIncreaseDuration);
            yield return null;
        }

        rb.mass = targetMass;
    }

    float GetForceMultiplierByScale()
    {
        float currentScale = transform.localScale.x;

        // Normalizamos el tamaño respecto a la escala base
        float scaleRatio = currentScale / baseScale;

        // Cuanto más grande, menos fuerza
        // Usamos inversa para efecto Katamari
        float multiplier = 1f / scaleRatio;

        // Clamp para evitar extremos
        return Mathf.Clamp(multiplier, minForceMultiplier, maxForceMultiplier);
    }

}
