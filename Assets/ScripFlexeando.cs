using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScripFlexeando : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    public float fuerzaSalto;
    public bool enPiso;
    public Transform refPie;
    public float velX  = 20f;

    public Transform contArma;
    public bool tieneArma;

    public Transform mira;
    public Transform refManoArma;

    public Transform refOjos;
    public Transform cabeza;


    //Recibir mordida una a una
    float momUltMordida = 0;
    float tiempoGracia = 1;

    //Vidas
    int vidasTotal = 2;
    int vidasActual;
    public TMPro.TextMeshProUGUI TextoVida;

    //ContadorMuertos
    int enemigosMuertos;

    public UnityEngine.UI.Image mascaraDanio;
    public UnityEngine.UI.Image Vidas;
    public UnityEngine.UI.Image Vida;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        vidasActual = vidasTotal;
        enemigosMuertos = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Movernos horizontalmente
        float movX;
        movX = Input.GetAxis("Horizontal");
        anim.SetFloat("absMovX", Mathf.Abs(movX));
        rb.velocity = new Vector2(velX * movX, rb.velocity.y);

        //Detectar si estamos pisando
        enPiso = Physics2D.OverlapCircle(refPie.position, 2f, 1<<8);//Cuando el pie está cerca del piso
        anim.SetBool("enPiso", enPiso);

        //saltar
        if (Input.GetButtonDown("Jump") && enPiso)
        {
            rb.AddForce(new Vector2(0, fuerzaSalto), ForceMode2D.Impulse );
        }

        //Girarse
        if (tieneArma){
            if (mira.transform.position.x < transform.position.x) {
                transform.localScale = new Vector3(-1,1,1)/2;
            }
            if (mira.transform.position.x > transform.position.x) {
                transform.localScale = new Vector3(1,1,1)/2;
            }
        }else{
            if (movX < 0) {
             transform.localScale = new Vector3(-1,1,1)/2;
            }
            if (movX > 0) {
                transform.localScale = new Vector3(1,1,1)/2;
            }
        }
        
        //Detectar el mouse y colocar ahi la mira
        if (tieneArma) {
            mira.position = Camera.main.ScreenToWorldPoint(
                new Vector3(
                 Input.mousePosition.x,
                 Input.mousePosition.y,
                 -Camera.main.transform.position.z
                )
            );
            refManoArma.position = mira.position;

            if (Input.GetButtonDown("Fire1"))
            {
                disparar();
            }
        }

        if (enemigosMuertos == 3)
        {
            SceneManager.LoadScene("GANAR");
        }
        

    }

    private void LateUpdate()
    {
        if (tieneArma) {
            //Que mire hacia donde apunta el arma
            cabeza.up = refOjos.position - mira.position;
        }
    }
    void disparar() {
        RaycastHit2D hit = Physics2D.Raycast(contArma.position, (mira.position - contArma.position).normalized, 1000f, ~(1 << 10));
        if (hit.collider != null){//le dio a algo
            if (hit.collider.gameObject.CompareTag("enemiga")) {
                Destroy(hit.collider.gameObject);
                enemigosMuertos++;
            }
        } 
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("arma")){
             tieneArma = true;
             Destroy(collision.gameObject);
             contArma.gameObject.SetActive(true);
        }
    }

    public void RecibirNavajazo() {
        if (Time.time > momUltMordida + tiempoGracia){
            momUltMordida = Time.time;
            vidasActual = vidasActual - 1;
            if (vidasActual == 1 ){
                Vidas.color = new Color(1,1,1, 0);
            } 
            if (vidasActual <= 0 ){
                Vida.color = new Color(1,1,1, 0);
                SceneManager.LoadScene("PERDER");
            }
        }
    }

    private void FixedUpdate(){
        float valorAlfa = 1f / vidasTotal * (vidasTotal - vidasActual); 
        mascaraDanio.color = new Color(1,1,1, valorAlfa);
    }
}
