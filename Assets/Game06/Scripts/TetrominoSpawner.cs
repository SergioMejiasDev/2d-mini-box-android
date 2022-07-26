using UnityEngine;

/// <summary>
/// Clase encargada de generar los tetrominós durante la partida.
/// </summary>
public class TetrominoSpawner : MonoBehaviour
{
    /// <summary>
    /// Todos los posibles tetrominós que pueden ser generados.
    /// </summary>
    [SerializeField] GameObject[] tetrominos = null;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager6.manager6.PauseGame();
        }
    }

    /// <summary>
    /// Función que genera una nueva pieza durante la partida.
    /// </summary>
    public void Spawn()
    {
        int randomValue = Random.Range(0, tetrominos.Length);

        Instantiate(tetrominos[randomValue], transform.position, Quaternion.identity);
    }
}