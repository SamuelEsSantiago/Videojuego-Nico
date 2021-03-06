using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBPartsHandler : MonoBehaviour
{
    #region Part Stuff
    [Header("Part Stuff")]
    [SerializeField] private List<GameObject> parts;
    private int totalParts;
    [SerializeReference] private List<GameObject> movedParts; 

    public bool IsAssembled { get => Assembled(totalParts); }
    //[SerializeReference] private List<GameObject> assembledParts;
    #endregion

    #region GameObject Position Reference
    [Header("GameObject position reference")]
    [SerializeField] private GameObject rightPositionReference;
    [SerializeField] private GameObject leftPositionReference;
    [SerializeReference] private GameObject currentPositionsReference;
    public GameObject CurrentReference { get => currentPositionsReference; }


    #endregion

    #region Speed and Time
    [Header("Speed and Time")]
    [SerializeField] private float speedMultiplier;
    private float speed;
    [SerializeField] private float timeBtwMove;
    private float curTimeBtwMove;

    [SerializeField] private float waitTimeWhenAssembled;
    private float curAssembledTime;

    #endregion

    #region LaserWarning
    [SerializeField] private LaserShooter laserShooter;
    public Transform ShotPos {get; set;}
    public Vector2 EndPos {get; set;}
    #endregion

    #region Events
    public Action<GameObject> ChangedReference;
    #endregion


    void Awake()
    {
        ChangedReference?.Invoke(currentPositionsReference);
        //OnChangedReference(currentPositionsReference);
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = Entity.averageSpeed * speedMultiplier;

        totalParts = parts.Count + movedParts.Count;
        if (currentPositionsReference == null)
        {
            currentPositionsReference = leftPositionReference;
        }
        //OnChangedReference(currentPositionsReference);
        ChangedReference?.Invoke(currentPositionsReference);

        currentPositionsReference.transform.parent.GetComponent<MBJumper>().isReference = true;


    }

    // Update is called once per frame
    void Update()
    {
        //if (!Assembled(totalParts))
        if (!Assembled(totalParts))
        {
            if (curTimeBtwMove > timeBtwMove)
            {
                AddMovedPart();
                curTimeBtwMove = 0;
            }
            else
            {
                //UpdateAssembledParts();
                curTimeBtwMove += Time.deltaTime;
            }
        }
        else
        {
            if (parts.Count == 0)
            {
                parts.AddRange(movedParts);
                //Center =  MathUtils.FindCenterOfTransforms(assembledParts.GetRange(0, parts.Count));
            }
            if (curAssembledTime > waitTimeWhenAssembled)
            {
                //assembled = false;
                //movedParts.Clear();
                if (currentPositionsReference.GetComponentInParent<MBJumper>().inStartPos)
                {
                    movedParts.Clear();
                    ChangePositionReference();
                    //OnChangedReference(currentPositionsReference);
                    ChangedReference?.Invoke(currentPositionsReference);
                    curAssembledTime = 0;
                }
            }
            else
            {
                curAssembledTime += Time.deltaTime;
            }
        }
    }

    

    void FixedUpdate()
    {
        foreach (var part in movedParts)
        {

            var reference = GetTargetReference(part);

            if (reference != null)
            {
                //float speedRatio =  Vector2.Distance(part.transform.position, reference.transform.position) / distanceSpeedRatio ;
                part.transform.position = Vector2.MoveTowards(part.transform.position, reference.transform.position, speed * Time.deltaTime);
                part.transform.rotation = reference.transform.rotation;
                
            }
        }
    }

    void AddMovedPart()
    {
        try
        {
            int randomPart = 0;
            randomPart = RandomGenerator.NewRandom(0, parts.Count-1);
            ShotPos = parts[randomPart].transform;
            EndPos = GetTargetReference(parts[randomPart]).transform.position;

            laserShooter.ShootLaserAndSetShotPos(ShotPos, EndPos);

            if (!movedParts.Contains(parts[randomPart]))
            {
                movedParts.Add(parts[randomPart]);
                parts.RemoveAt(randomPart);
            }
            
        }
        catch (ArgumentOutOfRangeException)
        {
            return;
        }

        
    }


    private GameObject GetTargetReference(GameObject part)
    {
        GameObject reference = ScenesManagers
                .GetComponentsInChildrenList<Transform>(currentPositionsReference)
                .Find(g => g.gameObject.ToString() == part.gameObject.ToString()).gameObject;

        if (reference != null)
        {
            return reference;
        }
        else
        {
            return new GameObject();
        }
    }

    private void ChangePositionReference()
    {
        currentPositionsReference = 
            currentPositionsReference == rightPositionReference? 
                leftPositionReference :
                rightPositionReference;
    }


    bool Assembled(int manyParts)
    {
        if (movedParts.Count < manyParts)
        {
            return false;
        }

        foreach (var part in movedParts.GetRange(0, manyParts))
        {
            if (part.transform.position != GetTargetReference(part).transform.position)
            {
                return false;
            }
        }
        return true;
    }
}
