using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour {

    private _GameController gameController;

    private Animator playerAnimator;
    private Rigidbody2D playerRb;
    private float horizontal;
    private float vertical;
    private Vector3 dir = Vector3.right;

    public Transform groundCheck; // Variável responsavel para detectar colisão com a superficie
    public float speed; // Velocidade de movimento do personagem
    public float jumpForce; // Força para gerar o pulo do personagem
    public bool Grounded; // Verifica se o personagem está pisando em alguma superficie
    public int idAnimation; // Inicia o id da animação
    public bool lookLeft; // Indica se o personagem está virado para a esquerda
    public bool attacking; // Verifica se o personagem está atacando
    public Collider2D standing; // Colisor em pé
    public Collider2D crounching; // coliso abaixado
    public LayerMask whatIsGround; //Incica o que é superficie para o teste do grounded
    public Transform hand;
    public LayerMask interaction;
    public GameObject objetoInteracao;
    public GameObject[] armas;

    public int vidaMax;
    public int vidaAtual;
    

	// Use this for initialization
	void Start () {
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;

        vidaAtual = vidaMax;

        foreach(GameObject o in armas)
        {
            o.SetActive(false);
        }
	}

    // Usado para detectar colisões com os objetos
    void FixedUpdate()
    {
        Grounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f, whatIsGround);

        playerRb.velocity = new Vector2(horizontal * speed, playerRb.velocity.y);

        Interagir();

    }
    // Update is called once per frame
    void Update () {
         horizontal = Input.GetAxisRaw("Horizontal");
         vertical = Input.GetAxisRaw("Vertical");

        if(horizontal > 0 && lookLeft && attacking == false)
        {
            Flip();
        } else if(horizontal < 0 && !lookLeft && attacking == false)
        {
            Flip();
        }

        if (vertical < 0)
        {
            idAnimation = 2;
            if (Grounded)
            {
                horizontal = 0;
            }          
        }
        else if (horizontal != 0 && vertical >= 0)
        {
            idAnimation = 1;
        } else if(vertical >= 0)
        {
            idAnimation = 0;
        }

        if(Input.GetButtonDown("Fire1") && vertical >= 0 && attacking == false && objetoInteracao == null)
        {
            playerAnimator.SetTrigger("atack");
        }

        if (Input.GetButtonDown("Fire1") && vertical >= 0 && attacking == false && objetoInteracao != null)
        {
            objetoInteracao.SendMessage("interacao", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("Jump") && Grounded && attacking == false)
        {
            playerRb.AddForce(new Vector2(0, jumpForce));
          
        }

        if(attacking && Grounded)
        {
            horizontal = 0;
        }

        if(vertical < 0 && Grounded)
        {
            crounching.enabled = true;
            standing.enabled = false;
        } else if(vertical >=0 && Grounded)
        {
            crounching.enabled = false;
            standing.enabled = true;
        } else if(vertical != 0 && Grounded == false)
        {
            crounching.enabled = false;
            standing.enabled = true;
        }

        playerAnimator.SetBool("grounded", Grounded);
        playerAnimator.SetInteger("idAnimaton", idAnimation);
        playerAnimator.SetFloat("speedY", playerRb.velocity.y);

	}

    void Flip()
    {
        lookLeft = !lookLeft;
        float x = transform.localScale.x;
        x *= -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        dir.x = x;
    }

    public void Atack(int atk)
    {
        switch(atk)
        {
            case 0:
                attacking = false;
                armas[2].SetActive(false);
                break;
            case 1:
                attacking = true;
                break;
        }
    }

    void Interagir()
    {
        Debug.DrawRay(hand.position, dir * 0.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(hand.position, dir, 0.1f, interaction);
        if(hit)
        {
            objetoInteracao = hit.collider.gameObject;
        } else
        {
            objetoInteracao = null;
        }

    }

    void cobtroleArma(int id)
    {
        foreach (GameObject o in armas)
        {
            o.SetActive(false);
        }

        armas[id].SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case "coletavel":
                col.gameObject.SendMessage("coletar", SendMessageOptions.DontRequireReceiver);
                break;
        }
    }

}
