using UnityEngine;
using System;
using Pathfinding;
using System.Collections;
using UnityEngine.Tilemaps;

public class MoleCactus : Enemy
{
    [Header("Self Additions")]
    [Header("Surface")]
    [SerializeField] private float secondFovDistance;
    [SerializeField] private byte surfaceTagIndex;
    private bool surfaceActive;

    [SerializeField] private Vector2 groundCheckerToSurface;
    [SerializeField] private Vector2 groundCheckerToGround;

    bool justStarted = true;
    bool inGround;


    #region PathFinding variables
    [Header("Pathfinfing")]
    [SerializeField] private Transform target;
    [SerializeField] private float activateDistance;
    [SerializeField] private float pathUpdateSeconds;
    public bool canMove = true;
    [SerializeField] private float nextWaypointDistance;
    [SerializeField] private float jumpNodeHeightRequirement;
    private Path path;
    private int currentWaypoint = 0;
    private Seeker seeker;
    private Vector2 lastGroundPos;
    
    #endregion

    [Header("Animation")]
    [SerializeField] private float distanceToAnimate;
    [SerializeField] private float delayAfterAnim;
    bool canAnimate = true;

    void ActivateAnimation()
    {
        canAnimate = true;
    }

    new void Start()
    {
        base.Start();
        seeker = GetComponent<Seeker>();
        target = player.transform;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        lastGroundPos = GetPosition();

        //groundChecker.transform.localPosition = groundCheckerToSurface;

    }

    new void Update()
    {
        if (canAnimate)
        {
            if (Vector2.Distance(GetPosition(), player.GetPosition()) <= distanceToAnimate)
            {
                OnSurfaceChange(true);
            }
            else
            {
                OnSurfaceChange(false);
            }
        }
        // Enables or disables the Surface graph to be transvarsable, so it can follow a path there
        if (Vector2.Distance(fieldOfView.FovOrigin.position, player.GetPosition()) <= secondFovDistance)
        {
            //if (!surfaceActive)
            {
                seeker.traversableTags = MathUtils.EditBitInBitmask(seeker.traversableTags, surfaceTagIndex, true);
                surfaceActive = true;
                lastGroundPos = GetPosition();
                
                //OnSurfaceChange(exitGround: true);
            }
        }
        else
        {
            //if (surfaceActive)
            {
                seeker.traversableTags = MathUtils.EditBitInBitmask(seeker.traversableTags, surfaceTagIndex, false);
                surfaceActive = false;

                //OnSurfaceChange(exitGround: false);
            }
        }
        
        
        base.Update();
    }

    protected override void MainRoutine()
    {
        //animationManager.ChangeAnimation("idle_in_ground");
        enemyMovement.GoTo(lastGroundPos, chasing: false, gravity: false);
    }

    protected override void ChasePlayer()
    {
        if (TargetInDistance() && canMove)
        {
            var currentAnim = animationManager.currentState;
            FollowPath();
        }
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        try
        {
            if (canMove)
            {
                
                groundChecker.transform.position = Vector3.MoveTowards(groundChecker.transform.position, path.vectorPath[currentWaypoint], enemyMovement.ChaseSpeed * Time.deltaTime );
                
            }
        }
        catch (System.Exception)
        {
            
        }
    }

    #region Pathfinding Stuff
    void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(GetPosition(), target.position, OnPathComplete);
        }
        
    }

    void FollowPath()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - (Vector2) GetPosition()).normalized;
        Vector2 force = direction * (surfaceActive? enemyMovement.ChaseSpeed : enemyMovement.DefaultSpeed) * Time.deltaTime;

        // Movement
        enemyMovement.GoTo((Vector2) path.vectorPath[currentWaypoint], chasing: surfaceActive, gravity: false);
        //enemyMovement.Translate(direction, chasing: surfaceActive);
        //rigidbody2d.AddForce(force, ForceMode2D.Force);
        // Next Waypoint
        

        float distance = Vector2.Distance(GetPosition(), path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(GetPosition(), target.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    #endregion

    void OnSurfaceChange(bool exitGround)
    {
        if (exitGround)
        {
            inGround = false;
            if (animationManager.currentState == "exit_ground")
            {
                animationManager.ChangeAnimation("idle_outside_ground");
            }
            else
            {
                animationManager.ChangeAnimation("exit_ground");
            }
        }
        else// if (MathUtils.BoundsIsEncapsulated(ground.GetBounds(), groundChecker.Collider2D.bounds))
        {
            inGround = true;
            try
            {
                transform.position = path.vectorPath[currentWaypoint];
            }
            catch (System.Exception)
            {

            }
            if (animationManager.currentState == "enter_ground")
            {
                animationManager.ChangeAnimation("idle_in_ground");
            }
            else
            {
                animationManager.ChangeAnimation("enter_ground");
            }
        }
        canAnimate = false;
        Invoke("ActivateAnimation", delayAfterAnim);

        collisionHandler.gameObject.SetActive(exitGround);
        //Invoke("ActivateMove", 1.5f);
    }

    void ActivateMove()
    {
        groundChecker.transform.position = path.vectorPath[currentWaypoint];
        canMove = true;
        animationManager.nextStateEnabled = false;
    }
}