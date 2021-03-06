using System;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectile : MonoBehaviour
{
    public static GunProjectile instance;
    private void Awake() {
        if(instance!=null){
            return;
        }
        instance = this;
    }
    //public Item prueba;
    public GameObject projectilePrefab;
    public Transform shotPoint;
    public float offset;
    public GameObject linterna;
    [SerializeField] private MouseDirPointer mouseDirPointer;
    private PlayerManager player;
    private Vector2 mousePosition; //No necesario la posicion exacta del mouse
    void Start()
    {
        player = PlayerManager.instance;
    }
    private void Update() {
        mousePosition = mouseDirPointer.PointerDir;

        //Vector2 dif = mousePosition - (Vector2)transform.position;
        Vector2 dif = mouseDirPointer.MouseDirection;
        float rotZ = Mathf.Atan2(dif.y,dif.x) * Mathf.Rad2Deg;
        //float rotZ = MathUtils.GetAngleBetween(player.transform.position, mousePosition);
        transform.rotation = Quaternion.Euler(0f,0f,rotZ + offset);
    }
    public void StartAiming(){
        PlayerManager.instance.isAiming = true;
    }
    public void StopAiming(){
        PlayerManager.instance.isAiming = false;
    }
    

    public Action ObjectShot;
    public void ShotObject(Item item){
        GameObject projectile = Instantiate(projectilePrefab,shotPoint.position,transform.rotation);
        projectile.GetComponent<ObjProjectile>().SetItem(item);

        ObjectShot?.Invoke();
    }
    /*
    public void ShotObject(Item item, float duration){
        GameObject projectile = Instantiate(projectilePrefab,shotPoint.position,transform.rotation);
        var objProj =  projectile.GetComponent<ObjProjectile>();
        objProj.SetItem(item);
        objProj.knockback.duration = duration;
        ObjectShot?.Invoke();
    }
    */
}
