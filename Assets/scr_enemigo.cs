using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_enemigo : MonoBehaviour
{

    Rigidbody2D rb;
    float limiteCaminataIzq;
    float limiteCaminataDer;

    public float velCaminata = 5f;
    int direccion = 1;

    public enum tipoComportamientoEnemigo { pasivo, persecucion, ataque}

    public tipoComportamientoEnemigo comportamiento = tipoComportamientoEnemigo.pasivo;

    float entradaZonaPersecucion = 7f;
    float salidaZonaPersecucion = 8f;
    float distanciaAtaque = 2; 

    float distanciaConPersonaje;
    public Transform Personaje;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        limiteCaminataIzq = transform.position.x - GetComponent<CircleCollider2D>().radius;
        limiteCaminataDer = transform.position.x + GetComponent<CircleCollider2D>().radius;

        Destroy (GetComponent<CircleCollider2D>());

        anim = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        distanciaConPersonaje = Mathf.Abs(Personaje.position.x - transform.position.x);
        switch (comportamiento){
            //Pasivo
            case tipoComportamientoEnemigo.pasivo:
                    //Desplazarse (caminar)
                    rb.velocity = new Vector2(velCaminata * 0.5f *direccion, rb.velocity.y);

                    //Girando segun los limites de su zona de caminata
                    if (transform.position.x < limiteCaminataIzq){
                        direccion = 1;
                    }
                    if (transform.position.x > limiteCaminataDer){
                        direccion = -1;
                    }

                    //velocidad del animator
                    anim.speed = 1f;

                    //entrar en la zona de persecucion
                    if (distanciaConPersonaje < entradaZonaPersecucion) comportamiento = tipoComportamientoEnemigo.persecucion;
                break;

            //Persecución    
            case tipoComportamientoEnemigo.persecucion:
                    //Desplazarse (Corriendo)
                    rb.velocity = new Vector2(velCaminata * 1f * direccion, rb.velocity.y);

                    //Girando segun los limites de su zona de caminata
                    if (Personaje.position.x > transform.position.x) direccion = 1;
                    if (Personaje.position.x < transform.position.x) direccion = -1;
                    
                    //velocidad del animator
                    anim.speed = 1.5f;

                    //volver a la zona pasiva
                    if (distanciaConPersonaje > salidaZonaPersecucion) comportamiento = tipoComportamientoEnemigo.pasivo;

                    if (distanciaConPersonaje < distanciaAtaque ) comportamiento = tipoComportamientoEnemigo.ataque; 

                break;

            //Atacar
            case tipoComportamientoEnemigo.ataque:
                    anim.SetTrigger("atacar");

                    if (Personaje.position.x > transform.position.x) direccion = 1;
                    if (Personaje.position.x < transform.position.x) direccion = -1;

                    //Girando segun la posición del mariachi
                    if (Personaje.position.x > transform.position.x){
                        direccion = 1;
                    }
                    if (Personaje.position.x < transform.position.x){
                        direccion = -1;
                    }

                    //velocidad del animator
                    anim.speed = 1f;

                    //volver a la zona de persecución
                    if (distanciaConPersonaje > distanciaAtaque ) {
                        comportamiento = tipoComportamientoEnemigo.persecucion; 
                        anim.ResetTrigger("atacar");
                    }

                break;
        }
        transform.localScale = new Vector3( 0.4f * direccion,0.4f,0.4f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && comportamiento == tipoComportamientoEnemigo.ataque)
        {
            Personaje.GetComponent<ScripFlexeando>().RecibirNavajazo();
        }
    }
}
