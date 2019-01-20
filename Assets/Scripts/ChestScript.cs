using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour {

    private _GameController gameController;
    private SpriteRenderer spriteRenderer;

    public Sprite[] imagemObjeto;
    public bool open;
    public GameObject[] lootes;
    private bool gerouLoot;

	void Start () {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	public void interacao()
    {
        if (open == false)
        {
            open = true;
            spriteRenderer.sprite = imagemObjeto[1];
            if(gameController == null)
            {
                gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
            }
            if (!gerouLoot)
            {
                StartCoroutine("gerarLoot");
            }

        } else if(open)
        {
            open = false;
            spriteRenderer.sprite = imagemObjeto[0];
        }
    }

    IEnumerator gerarLoot()
    {
        gerouLoot = true;
        int qtdMoedas = Random.Range(3, 10);
        for (int l = 0; l < qtdMoedas; l++)
        {
            int rand = 0;
            int idLoot = 0;
            rand = Random.Range(0, 100);

            if(rand >= 75)
            {
                idLoot = 1;
            }

            GameObject lootTemp = Instantiate(lootes[idLoot], transform.position, transform.localRotation);
            lootTemp.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-25, 25), 75));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
