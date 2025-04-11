using UnityEngine;

public class EnemyChecks : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private EnemyVelocity enemyVelocity;
    private TimeManager timeManager;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public LayerMask wallLayer;
    public bool isFacingRight;
    private bool lastTimeTraveledRight;
    private bool lastTimeTraveledLeft;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyVelocity = GetComponent<EnemyVelocity>();

        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(timeManager.tpPause)
        {
            return;
        }

        IsGrounded();
        IsTouchingWallRight();
        IsTouchingWallLeft();
    }

    #region Ground Checks
    public bool IsGrounded()
    {
        Vector3 origin = transform.position; // Single origin at object position
        Vector3 halfExtents = new Vector3(0.4f, 0.5f, 0); // Box size (adjust as needed)
        Vector3 direction = Vector3.down;
        float raycastDistance = 0.55f;

        // Perform the BoxCast
        bool hitGround = Physics.BoxCast(origin, halfExtents, direction, 
                                        out RaycastHit hit, Quaternion.identity, 
                                        raycastDistance, groundLayer);
        // Debug Draw the BoxCast
        DebugDrawBoxCast(origin, halfExtents, direction, raycastDistance, Color.red);        

        if(hitGround)
        {
            
        }

        return hitGround;
    }
    #endregion



    #region Wall Checks
    public bool IsTouchingWallRight()
    {
        Vector3 origin = transform.position;
        Vector3 halfExtents = new Vector3(0.18f, 0.5f, 0);
        Vector3 direction = Vector3.right;
        float raycastDistance = 0.36f;

        DebugDrawBoxCast(origin, halfExtents, direction, raycastDistance, Color.red);

        bool hitWall = Physics.BoxCast(origin, halfExtents, direction, 
                                    out RaycastHit hit, Quaternion.identity, 
                                    raycastDistance, wallLayer);

        return hitWall;
    }

        public bool IsTouchingWallLeft()
    {
        Vector3 origin = transform.position;
        Vector3 halfExtents = new Vector3(0.18f, 0.5f, 0);
        Vector3 direction = Vector3.left;
        float raycastDistance = 0.36f;

        DebugDrawBoxCast(origin, halfExtents, direction, raycastDistance, Color.red);

        bool hitWall = Physics.BoxCast(origin, halfExtents, direction, 
                                    out RaycastHit hit, Quaternion.identity, 
                                    raycastDistance, wallLayer);

        return hitWall;
    }

    public bool AttachedToWallRight()
    {
        if(IsTouchingWallRight() && !IsGrounded() && lastTimeTraveledRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AttachedToWallLeft()
    {
        if(IsTouchingWallLeft() && !IsGrounded() && lastTimeTraveledLeft)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion



    #region Debug Draw Casts
    void DebugDrawBoxCast(Vector3 position, Vector3 halfExtents, Vector3 direction, float distance, Color color)
    {
        Vector3 endPosition = position + direction.normalized * distance;

        // Draw initial and final boxes
        DebugDrawBox(position, halfExtents, color);
        DebugDrawBox(endPosition, halfExtents, color);

        // Draw the cast line
        //Debug.DrawLine(position, endPosition, color);
    }

    void DebugDrawBox(Vector3 center, Vector3 halfExtents, Color color)
    {
        Vector3[] corners = new Vector3[8];

        for (int i = 0; i < 8; i++)
        {
            corners[i] = center + new Vector3(
                (i & 1) == 0 ? -halfExtents.x : halfExtents.x,
                (i & 2) == 0 ? -halfExtents.y : halfExtents.y,
                (i & 4) == 0 ? -halfExtents.z : halfExtents.z
            );
        }

        // Connect edges of the box
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[3], color);
        Debug.DrawLine(corners[3], corners[2], color);
        Debug.DrawLine(corners[2], corners[0], color);

        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[7], color);
        Debug.DrawLine(corners[7], corners[6], color);
        Debug.DrawLine(corners[6], corners[4], color);

        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }
    #endregion



    #region Miscellaneous

    #endregion
}
