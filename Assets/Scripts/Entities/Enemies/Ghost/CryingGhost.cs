using UnityEngine;
using System.Collections;
public class CryingGhost : Enemy
{
    [SerializeField] private float startTimeBtwShot;
    private float timeBtwShot;
    
    protected override void ChasePlayer()
    {
        animationManager.ChangeAnimation("start_move");
        if (MathUtils.GetAbsXDistance(player.GetPosition(), GetPosition()) > 0.5f)
        {
            enemyMovement.GoTo(MathUtils.GetXDirection(GetPosition(), player.GetPosition()), chasing: false, gravity: false);
        }

        Vector3 shotPos = projectileShooter.ShotPos.position;
        RaycastHit2D hit = Physics2D.Linecast(shotPos, shotPos - projectileShooter.ShotPos.up * fieldOfView.ViewDistance, 1 << LayerMask.NameToLayer("Ground"));
        
        if (timeBtwShot <= 0)
        {
            projectileShooter.ShootProjectile(hit.point);
            timeBtwShot = startTimeBtwShot;
        }
        else
        {
            timeBtwShot -= Time.deltaTime;
        }
    }
}