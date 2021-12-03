using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controlador : MonoBehaviour
{

    public void CambiarScena(string nombre)
    {
        print("cambiando a la scena" + nombre);
        SceneManager.LoadScene(nombre);
    }

    public void salir()
    {
        print("Salir del juego");
        Application.Quit();

    }
}
