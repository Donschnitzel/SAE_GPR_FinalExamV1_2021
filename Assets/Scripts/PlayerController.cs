using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationPower = 3f;
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float sprintSpeed = 3f;
    [SerializeField] private Transform followTransform;
    [SerializeField] private Animator animator;
    [SerializeField]  private Rigidbody _rigidbody;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float sprintInput;
    private IPlayerAction[] playerActions;

    private Transform _transform;
    private static readonly int HORIZONTAL_ANIMATOR_ID = Animator.StringToHash("Horizontal");
    private static readonly int VERTICAL_ANIMATOR_ID = Animator.StringToHash("Vertical");

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerActions = GetComponentsInChildren<IPlayerAction>();
    }
        

    private void FixedUpdate()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        lookInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        sprintInput = Input.GetAxis("Sprint");

        UpdateFollowTargetRotation();

        float speed = 0;
        if (CanMove())
        {
            speed = Mathf.Lerp(walkSpeed, sprintSpeed, sprintInput);
            Vector3 movement = _transform.forward * (moveInput.y * speed) + _transform.right * (moveInput.x * speed);
            _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
        }
        else
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        }

        animator.SetFloat(HORIZONTAL_ANIMATOR_ID, moveInput.x * speed);
        animator.SetFloat(VERTICAL_ANIMATOR_ID, moveInput.y * speed);

        //only rotate the player when moving, allows user to look at the player when idle
        if (moveInput.magnitude > 0.01f)
        {
            //Set the player rotation based on the look transform
            transform.rotation = Quaternion.Euler(0, followTransform.eulerAngles.y, 0);
            //reset the y rotation of the look transform
            followTransform.localEulerAngles = new Vector3(followTransform.localEulerAngles.x, 0, 0);
        }
    }

    private void UpdateFollowTargetRotation()
    {
        //Update follow target rotation
        Quaternion rotation = followTransform.rotation;
        rotation *= Quaternion.AngleAxis(lookInput.x * rotationPower, Vector3.up);
        rotation *= Quaternion.AngleAxis(lookInput.y * rotationPower, Vector3.right);
        followTransform.rotation = rotation;

        var angles = followTransform.localEulerAngles;
        angles.z = 0;
        var angle = angles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }
        followTransform.localEulerAngles = angles;
    }

    private bool CanMove()
    {
        if (playerActions == null)
            return true;

        foreach (var action in playerActions) // TODO: think about using foreach, bad for memory management
        {
            if (action.IsInAction())
            {
                return false;
            }
        }

        return true;
    }
}
