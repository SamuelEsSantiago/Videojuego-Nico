using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VientoHazard : MonoBehaviour
{
    
    [SerializeField] private float minInterval = 0f;
    [SerializeField] private float maxInterval = 1f;
    [SerializeField] private float minDuration = 1f;
    [SerializeField] private float maxDuration = 10f;
    [SerializeField] private float windForce = 3000f;
    private float interval;
    private float duration;
    private bool active;
    private bool d,i;
    private bool flipDir;
    [SerializeField] private float damageAmount;
    [SerializeField] private float damageThreshold = defaultDamageThreshold;
    public const float defaultDamageThreshold = 0; 
    float currentDamage;
    [SerializeField] private State hazardState;
    private PlayerManager player;
    new private ParticleSystem particleSystem;
    public GameObject fondo;
    void Start()
    { 
        hazardState.onEffect = false;
        player = PlayerManager.instance;
        particleSystem = GetComponent<ParticleSystem>();
        interval = GetTimeValue(minInterval,maxInterval);
        d = false;
        i = true;
        duration = 0;
        currentDamage = 0;
        particleSystem.Stop();
        fondo.SetActive(false);
    }
    private void Update() {
        transform.position = player.GetPosition();
        if(flipDir){
        active = particleSystem.isPlaying;
        }
        if(duration <= 0 && !i){
            if(particleSystem.isPlaying){
                particleSystem.Stop();
            }
                fondo.SetActive(false);
            interval = GetTimeValue(minInterval,maxInterval);
            d = false;
            i = true;
        }else if(duration > 0 && d){
            duration -= Time.deltaTime;
        }

        if(interval <= 0 && !d){
            flipDir = RandomGenerator.MatchProbability(50f);
            if(flipDir) transform.rotation = Quaternion.Euler(Vector3.forward * 180);
            else transform.rotation = Quaternion.Euler(Vector3.forward * 0);
            if (particleSystem.isStopped){
                currentDamage = 0;
                particleSystem.Play();
                fondo.SetActive(true);
            }
            duration = GetTimeValue(minDuration,maxDuration);
            i = false;
            d = true;
        }else if(interval > 0 && i){
            interval -= Time.deltaTime;
        }

        
    }
    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player" && !player.isImmune )
        {
            hazardState = player.statesManager.AddState(hazardState);
            ParticleSystem.CollisionModule collisionModule = particleSystem.collision;
            if(player.statesManager.currentStates.Contains(hazardState)){
                collisionModule.colliderForce = windForce;
                if (currentDamage < damageThreshold)
                {
                    currentDamage += damageAmount;
                    player.TakeTirement(damageAmount);
                }
            }else{
                currentDamage = 0;
                collisionModule.colliderForce = 0f;
            }
        }
    }

    float GetTimeValue (float min, float max){
        float time;
        time = RandomGenerator.NewRandom(min,max);
        return time;
    }
}
