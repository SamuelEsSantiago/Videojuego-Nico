using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrecisionMedufJuego : MonoBehaviour
{
    int random, randomreloj;
    public float x, y;
    public float angle =0;
    public float speed=(2*Mathf.PI)/5;
    float radius=5;
    [SerializeField] Rigidbody2D Body;
    public Vector2 position;
    bool reloj;
    public bool moviendose;

    [SerializeField] private Image longLine;
    [SerializeField] private Color solvedColor;


    [Header("Score Range")]
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    bool solved;

    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        reloj = true;
    }
     void Update()
    {
        
        Moverse();
    }
    void Moverse(){
        if (reloj)
        {
            random=Random.Range(1,3);
            transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, x);
            x+=random;
            if (x>randomreloj)
            {
                randomreloj=Random.Range(-500,-750);
                reloj = false;
            }
        }else
        {
            random=Random.Range(1,3);
            transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, x);
            x-=random;
            if (x<randomreloj)
            {
                randomreloj=Random.Range(500,750);
                reloj = true;
            }
        }
    }
    
    public void OnClickButton(Button button)
    {
        if (solved) return;
        if (transform.localEulerAngles.z <= minAngle || transform.localEulerAngles.z >= maxAngle )
        {
            ScoreController.score++;
            solved = true;
            enabled = false;
            button.interactable = false;

            longLine.color = solvedColor;
        }
        else
        {
            enabled = !enabled;
        }
    }   

}