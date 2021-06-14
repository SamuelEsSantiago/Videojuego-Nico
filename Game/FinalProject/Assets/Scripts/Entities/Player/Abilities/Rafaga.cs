using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rafaga : Ability
{
    public GameObject Rafagaobj;
    public GameObject parti;
    public bool isActivated;
    private Spore spore;
    [SerializeField] private float empuje;
    private List<Enemy> enemies;

    public override void UseAbility()
    {   
        if(player.currentStamina < staminaCost)return;
        isActivated=true;
        base.UseAbility();
    }
    protected override void Start()
    {
        base.Start();
        time = cooldownTime;
        isActivated=false;
        spore = parti.GetComponent<Spore>();
        spore.ParticleCollisionHandler += spore_ParticleCollision;
    }
    protected override void Update()
    {
        if (!isUnlocked)
        {
            this.enabled = false;
        }
        if (isInCooldown)
        {
            time += Time.deltaTime;
            if (time >= cooldownTime)
            {
                isInCooldown = false;
                time = 0;
            }
        }else
        {
            isActivated = false;
        }
        if (isActivated)
        {
            parti.SetActive(true);
        }else
        {
            parti.SetActive(false);
            if (enemies != null)
            {
                foreach (var enemy in enemies)
                {
                    if (enemy != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().velocity = new Vector2();
                    }
                }
            }
        }
        if (Input.GetKeyDown(hotkey))
        {
            if (player.currentStamina > staminaCost)
            {
                UseAbility();
            }
        }
    }
    void spore_ParticleCollision(GameObject other){
        if(other.TryGetComponent<Enemy>(out Enemy enemy)){
            enemies.Add(enemy);
            if (player.facingDirection == "left")
            {
                enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.left * empuje);
            }else if (player.facingDirection == "right")
            {
                enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.right * empuje);
            }
        }   
    }
}
