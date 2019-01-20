using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControleDanoInimigo : MonoBehaviour {

    private _GameController gameController;
    private playerScript pScript;
    private float kx;
    private bool getHit;  // indica se tomou um hit
    private SpriteRenderer sRender;
    private Animator animator;

    public bool olhandoEsquerda;
    public bool playerEsquerda;
    private bool died; // indica se esta morto

    [Header("Configurção resistência/fraqueza")]
    public float[] ajusteDano; // sistema de ajuste de dano com resitências e fraquesas

    [Header("Configurção de knockBack")]
    public GameObject knockForcePrefab; // força de repulsão
    public Transform knockPosition;  // ponto de origem da força
    public float knockX; // valor padrão da posição x
    public Color[] characterColor; // controle de cor do personagem

    [Header("Configurção de vida")]
    public int vidaInimigo;
    public int vidaAtual;
    public GameObject barrasVida; // Objeto contendo todas as barras
    public Transform hpBar; // Objeto indicador da quantidade de vida
    private float percVida; // Controla o percentual de vida
    public GameObject danoTxtPrefab; // objeto que irá exibir o dano tomado

    [Header("Configuração de chão")]
    public Transform groundCheck;
    public LayerMask whatIsGround;

    [Header("Configuração de Loot")]
    public GameObject lootes;
    


	void Start () {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        pScript = FindObjectOfType(typeof(playerScript)) as playerScript;
        sRender = GetComponent<SpriteRenderer>();
        sRender.color = characterColor[0];
        barrasVida.SetActive(false);
        vidaAtual = vidaInimigo;
        animator = GetComponent<Animator>();

        if (olhandoEsquerda)
        {
            float x = transform.localScale.x;
            float xBarraVida = barrasVida.transform.localScale.x;
            x *= -1;
            xBarraVida *= -1;
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
            barrasVida.transform.localScale = new Vector3(xBarraVida, barrasVida.transform.localScale.y,
                                                       barrasVida.transform.localScale.z);
        }
    }
	

	void Update () {

        float xPlayer = pScript.transform.position.x;

        if (xPlayer < transform.position.x)
        {
            playerEsquerda = true;

        } else if(xPlayer > transform.position.x)
        {
            playerEsquerda = false;
        }

        if(olhandoEsquerda && playerEsquerda)
        {
            kx = knockX;
        } else if (!olhandoEsquerda && playerEsquerda)
        {
            kx = knockX * -1;
        } else if(olhandoEsquerda && !playerEsquerda)
        {
            kx = knockX * -1;
        } else if (!olhandoEsquerda && !playerEsquerda)
        {
            kx = knockX;
        }

        knockPosition.localPosition = new Vector3(kx, knockPosition.localPosition.y, 0);
        animator.SetBool("grounded", true);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (died)
        {
            return;
        }

        switch (col.gameObject.tag)
        {
            case "arma":
                if (getHit == false)
                {
                    getHit = true;
                    barrasVida.SetActive(true);
                    ArmaInfo infoArma = col.gameObject.GetComponent<ArmaInfo>();
                    float danoArma = Random.Range(infoArma.danoMin, infoArma.danoMax);
                    int tipoDano = infoArma.tipoDano;
                    animator.SetTrigger("hit");

                    float danoTomado = danoArma + (danoArma * (ajusteDano[tipoDano] / 100));

                    vidaAtual -= Mathf.RoundToInt(danoTomado);

                    percVida = (float) vidaAtual / (float) vidaInimigo; // calcula o percentual de vida

                    if(percVida < 0)
                    {
                        percVida = 0;
                    }

                    hpBar.localScale = new Vector3(percVida, 1, 1);

                    if (vidaAtual <= 0)
                    {
                        died = true;
                        animator.SetInteger("idAnimaton", 3);
                        StartCoroutine("loot");
                    }

                    print("Tomei " + danoTomado + " de dano do tipo " + gameController.tiposDano[tipoDano]);

                    GameObject danoTemp = Instantiate(danoTxtPrefab, transform.position, transform.localRotation);
                    danoTemp.GetComponentInChildren<TextMeshPro>().text = Mathf.RoundToInt(danoTomado).ToString();
                    danoTemp.GetComponentInChildren<MeshRenderer>().sortingLayerName = "HID";

                    GameObject fxTemp = Instantiate(gameController.fxDano[tipoDano], transform.position, transform.localRotation);
                    Destroy(fxTemp, 1);

                    int forcaX = 50;
                    if(!playerEsquerda)
                    {
                        forcaX *= -1;
                    }

                    danoTemp.GetComponent<Rigidbody2D>().AddForce(new Vector2(forcaX, 230));
                    Destroy(danoTemp, 1.0f);

                    GameObject knockTemp = Instantiate(knockForcePrefab, knockPosition.position, knockPosition.localRotation);
                    Destroy(knockTemp, 0.02f);

                    StartCoroutine("invuneravel");
                }

                break;
        }
    }

    void flip()
    {
        olhandoEsquerda = !olhandoEsquerda;
        float x = transform.localScale.x;
        float xBarraVida = barrasVida.transform.localScale.x;
        x *= -1;
        xBarraVida *= -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        barrasVida.transform.localScale = new Vector3(xBarraVida, barrasVida.transform.localScale.y, 
                                                        barrasVida.transform.localScale.z);

    }

    IEnumerator loot()
    {
        yield return new WaitForSeconds(1);
        GameObject fxMorte = Instantiate(gameController.fxMorte, groundCheck.position, transform.localRotation);
        yield return new WaitForSeconds(0.5f);
        sRender.enabled = false;

        int qtdMoedas = Random.Range(1, 5);
        for(int l = 0; l < qtdMoedas; l++)
        {
            GameObject lootTemp = Instantiate(lootes, transform.position, transform.localRotation);
            lootTemp.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-25, 25), 75));
            yield return new WaitForSeconds(0.1f);
        }
       

        yield return new WaitForSeconds(0.7f);
        Destroy(fxMorte);
        Destroy(this.gameObject);
    }

    IEnumerator invuneravel()
    {
        sRender.color = characterColor[1];
        yield return new WaitForSeconds(0.2f);
        sRender.color = characterColor[0];
        yield return new WaitForSeconds(0.2f);
        sRender.color = characterColor[1];
        yield return new WaitForSeconds(0.2f);
        sRender.color = characterColor[0];
        yield return new WaitForSeconds(0.2f);
        sRender.color = characterColor[1];
        yield return new WaitForSeconds(0.2f);
        sRender.color = characterColor[0];
        yield return new WaitForSeconds(0.2f);
        getHit = false;
        barrasVida.SetActive(false);
    }
}
