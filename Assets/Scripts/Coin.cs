using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public int valor;
    private _GameController gameController;


    void Start()
    {
        gameController = gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void coletar()
    {
        gameController.gold += valor;
        Destroy(this.gameObject);
    }

}
