using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minimap;

public class PlayerManager : Entity
{
    #region Main Parameters
    public float maxStamina = 100;
    public float currentStaminaLimit {get => realCurrentStaminaLimit; 
        set{
            if(value < 10){
                realCurrentStaminaLimit = 10;
            }else{
                realCurrentStaminaLimit = value;
            }
        }
    }
    private float realCurrentStaminaLimit = 100;
    public float maxOxygen = 100;
    public float walkingSpeed;
    public float currentSpeed;
    public float speedMods = 1;
    public const float defaultwalkingSpeed = 7f;
    public const float defaultGravity = 2.5f;
    public float currentGravity;
    public const float defaultMass = 10;
    public float currentMass;
    public const float defaultDmgMod = 1;
    public float dmgMod {get;set;}
    //private GameObject[] players;
    #endregion

    #region Constant change Parameters
    public float currentStamina {get;set;}
    public float currentOxygen {get;set;}
    private float jumpTimeCounter;
    #endregion

    #region States
    //public bool isRunning;
    public bool isSwiming;
    public bool isStruggling;
    public bool isImmune;
    public bool isAiming;
    public bool isDashingH;
    public bool isDashingV;
    public bool isDoubleJumping;
    public bool isDodging;
    public bool DeathActive;
    public bool isDeath;
    public bool isInvisible;
    #endregion

    #region Layers, rigids, etc
    public GameObject camara;
    public GameObject Darkness;
    #endregion

    [SerializeField]private bool loosingStamina;
    [SerializeField]private bool loosingOxygen;
    private float regenCooldown;
    public PlayerInputs inputs;
    public State inmunityState;
    public AbilityManager abilityManager;
    [SerializeField] GameObject gameOverPrefab;
    public static PlayerManager instance;
    new void Awake()
    {
        base.Awake();
        if(instance==null)instance=this;
        else if(instance!=this)Destroy(gameObject);
        DontDestroyOnLoad(this);
    }

    new void Start()
    {
        RestoreValuesForDead();
        base.Start();
        regenCooldown = 5;
        GunProjectile.instance.ObjectShot -= gun_ObjectShot;
        GunProjectile.instance.ObjectShot += gun_ObjectShot;
    }

    new void Update()
    {
        if (!isCaptured)
        {
            SetAnimationStates();
        }
        rigidbody2d.mass = currentMass;
        isStruggling = false;
        isWalking = inputs.movementX != 0 && isGrounded && !isRunning;
        isFalling = rigidbody2d.velocity.y < 0f;
        if (!isCaptured)
        {
            if (!isFlying && !isDashingH && !isDashingV)
            {
                rigidbody2d.gravityScale = currentGravity;
                Move();
                Jump();
            }
            else if(!isDashingH && !isDashingV)
            {
                rigidbody2d.gravityScale = 0;
                Flying();
            }
            
        }
        else
        {
            rigidbody2d.Sleep();
        }
        
        if (inputs.movementX>0)
        {
            transform.eulerAngles = new Vector3(0,0,0);
        }
        else if (inputs.movementX<0)
        {
            transform.eulerAngles = new Vector3(0,180,0);
        }

        // animator.SetBool("Turn Left", moveInput<0 ); // Checks if the player turned left to start the turning animation
        if (loosingStamina)
        {
            if (regenCooldown > 0)
            {
                regenCooldown -= Time.deltaTime;
            }
            else
            {
                regenCooldown = 7;
                loosingStamina = false;
            }
        }
        else
        {
            StartCoroutine(Regeneration(1f, 0.05f));
        }
        if (loosingOxygen)
        {
            if (regenCooldown > 0)
            {
                regenCooldown -= Time.deltaTime;
            }
            else
            {
                regenCooldown = 7;
                loosingOxygen = false;
            }
        }
        else
        {
            StartCoroutine(Regeneration(1f, 0.05f));
        }
        if (isInWater)
        {
            StartCoroutine(Drowning(1f, 0.05f));
        }
        if(DeathActive){
            if (currentStamina <= 0 || currentOxygen <= 0)
            {
                WhenHeDied();
            }
        }
        base.Update();
    } 
    public void Jump()
    {
        if(!inputs.enabled)return;
        if ((isGrounded && inputs.jump) || (isInWater && inputs.jump))
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, rigidbody2d.gravityScale + jumpForce);
            isJumping = true;
            jumpTimeCounter = jumpTime;
        }   
        if (inputs.jump && isJumping)
        {
            if (jumpTimeCounter>0)
            {
                animationManager.RestartAnimation();
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, rigidbody2d.gravityScale + jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        else
        {
            isJumping = false;
        }
    }

    void Move()
    {
        walkingSpeed = defaultwalkingSpeed * speedMods;
        if(!isRunning) currentSpeed = walkingSpeed;
        if(!inputs.enabled){
            rigidbody2d.velocity = new Vector2(0f, rigidbody2d.velocity.y);
            return;
            }
        if (isInIce)
        {
            rigidbody2d.AddForce(new Vector2(inputs.movementX * currentSpeed, rigidbody2d.velocity.y));
        }
        else
        {
            rigidbody2d.velocity = new Vector2(inputs.movementX * currentSpeed, rigidbody2d.velocity.y);    
        }
    }

    void Flying()
    {
        if(!inputs.enabled)return;
        rigidbody2d.velocity = new Vector2(inputs.movementX, inputs.movementY)*currentSpeed;
    }

    // Call these methods to decrease or increase stamina periodically
    #region Periodic stamina change
    /// <summary>
    /// Decreases a certain amount of stamina through given time
    /// </summary>
    /// <param name="timeTired"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    public IEnumerator Tirement(int timeTired, float damage)
    {
        yield return new WaitForSeconds(timeTired);
        TakeTirement(damage);
        yield return null;
        if (isInWater)
        {
            yield return new WaitForSeconds(timeTired);
            TakeTirement(damage);
            yield return null;
        }else{
            yield return null;
        }
    }
    public IEnumerator Drowning(float timeDrowned, float drown)
    {
        yield return new WaitForSeconds (timeDrowned);
        if (!loosingOxygen)
        {
            currentOxygen -= drown;
        }
        yield return new WaitForSeconds(timeDrowned);
    }
        
    /// <summary>
    /// Increases a certain amount of stamina through given time
    /// </summary>
    /// <param name="timeTired"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    IEnumerator Regeneration(float timeRegen, float regen)
    {
        yield return new WaitForSeconds (timeRegen);
        if (!loosingStamina)
        {
            RegenStamina(regen);
        }
        yield return new WaitForSeconds(timeRegen);
        if (!isInWater)
        {
             yield return new WaitForSeconds (timeRegen);
            RegenOxygen(regen);
        }
    }
    #endregion





    // Call these methods to increase or decrease stamina directly
    #region Direct stamina changes
    public void TakeTirement(float damage)
    {
        if(isDeath) return;
        if(currentStamina > currentStaminaLimit) currentStamina = currentStaminaLimit;
        currentStamina -= damage * dmgMod;
        loosingStamina = true;
    }
    public void RegenStamina(float regen)
    {
        if(isDeath) return;
        if (currentStamina<currentStaminaLimit)
        {
        currentStamina += regen;
        }
        if (currentStamina>currentStaminaLimit)
        {
            currentStamina = currentStaminaLimit;
        }
    }

    public void RegenStaminaDontLimit(float regen)
    {
        if(isDeath) return;
        if (currentStamina<maxStamina )
        {
            currentStamina += regen;
        }
        if (currentStamina>maxStamina)
        {
            currentStamina = maxStamina;
        }
        if (currentStamina > currentStaminaLimit)
        {
            currentStaminaLimit = currentStaminaLimit + (currentStamina - currentStaminaLimit);   
        }
    }
    void RegenOxygen(float regen)
    {
        if(isDeath) return;
        if (currentOxygen<100)
        {
        currentOxygen += regen;
        }
        if (currentOxygen>100)
        {
            currentOxygen = 100;
        }
    }
    #endregion


    public void SetImmune()
    {
        statesManager.AddStateDontRepeat(inmunityState);
    }
    public void SetImmune(float time)
    {
        var inmun = Instantiate(inmunityState);
        inmun.duration = time;
        statesManager.AddStateDontRepeat(inmun);

    }

    void WhenHeDied()
    {
        if(isDeath) return;
        Debug.Log("ImdeadTnx4EvEr");
        currentGravity = defaultGravity;
        rigidbody2d.gravityScale = currentGravity;
        animationManager.ChangeAnimation("Nico_pass_out");
        SetEnabledPlayer(false);
        isDeath = true;
        Instantiate(gameOverPrefab);
    }

    public void SetEnabledPlayer(bool value)
    {
        abilityManager.SetActive(value);
        if (inputs != null) {
            inputs.enabled = value;
            if(!value){
                inputs.movementX = 0;
                inputs.movementY = 0;
            }
        }
        else
        {
            Debug.Log("trying to set " + value + " inputs. Can not do that, they're false for some reason. Called by " + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
        }
    }


    #region Animation
    // Not proud of this but what can I do
    void SetAnimationStates()
    {
        if (isDeath || animationManager.currentState == "Nico_pass_out") return;
        if (isAiming)
        {
            animationManager.ChangeAnimation("Nico_pre_pointing");
        }
        else if (animationManager.previousState != "Nico_pre_pointing")
        {
            if (isGrounded && !isFlying)
            {
                if (isWalking)
                {
                    animationManager.ChangeAnimation("Nico_walk");
                }
                else if (isRunning)
                {
                    animationManager.ChangeAnimation("Nico_run");
                }
                else 
                {
                    animationManager.ChangeAnimation("Nico_idle");
                }
            }
            else
            {
                if (isJumping && !isFlying)
                {
                    animationManager.ChangeAnimation("Nico_jump");
                }
                else if (isFlying)
                {
                    animationManager.ChangeAnimation("Nico_fly");
                }
                else if (isFalling)
                {
                    animationManager.ChangeAnimation("Nico_fall");
                }
            }
        }
    }

    public void ResetAnimations(){
        isWalking = false;
        isRunning = false;
        isJumping = false;
        isFlying = false;
        isFalling = false;
        isAiming = false;
        SetAnimationStates();
    }
    void gun_ObjectShot()
    {
        Debug.Log("object shot");
        animationManager.ChangeAnimation("Nico_throw");
        animationManager.SetNextAnimation("Nico_idle");
    }
    #endregion
    public void RestoreValuesForDead(){
        //statesManager.StopAll();
        EquipmentManager.instance.UnequipAll();
        abilityManager.LoadData();
        gameObject.GetComponent<SpriteRenderer>().color= new Color(1,1,1,1); 
        inmunityState.onEffect = false;
        statesManager.StopAll();
        walkingSpeed = defaultwalkingSpeed;
        currentSpeed = walkingSpeed;
        currentStaminaLimit = maxStamina;
        currentGravity = defaultGravity;
        currentMass = defaultMass;
        currentStamina = maxStamina;
        currentOxygen = maxOxygen;
        dmgMod = defaultDmgMod;
        speedMods = 1f;
        isInvisible = false;
        isInWater = false;
        isInDark = false;
        isInSnow = false;  
        isInIce = false;
        isDeath = false;
        DeathActive = true;
        animationManager.ChangeAnimation("Nico_idle");
        SetEnabledPlayer(true);
        ResetAnimations();
        Inventory.instance.LoadSaveData();
        Cofre.instance.LoadSaveData();
    }
}