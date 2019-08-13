using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
	// Componente RigidBody del cohete
	Rigidbody rigidBody;

	// Componente de la fuente de audio
	AudioSource audioSource;

	// Intensidad de la fuerza vertical (coordenadas locales)
	// El valor asignado será el por defecto, pero prevalece el
	// que haya en el inspector
	[SerializeField] float mainThrust = 1000f;

	// Intensidad de la fuerza de rotación en el eje forward (local)
	// El valor asignado será el por defecto, pero prevalece el
	// que haya en el inspector
	[SerializeField] float rcsThrust = 250f;

	// Estados del cohete
	enum State { Alive, Dying, Trascending }

	// Estado actual del cohete
	State state = State.Alive;

	// Tiempo de carga
	float loadTime = 1f;

	void Start()
	{
		// Versión gameObject.* hace lo mismo, pero es diferente
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		Audio();
		// Si esta vivo permite el control
		if (state == State.Alive)
		{
			Thrust();
			Rotate();
		}
	}

	// Si choca con un objeto
	void OnCollisionEnter(Collision collision)
	{
		// Si no esta vivo, nada
		if (state != State.Alive) return;
		// Si esta vivo
		switch (collision.gameObject.tag)
		{
			case "Friendly":
				// No pasa nada
				break;
			case "Finish":
				state = State.Trascending;
				// Carga la siguiente escena en 1 s
				Invoke("LoadNextLevel", loadTime);
				break;
			default:
				// Mueres
				state = State.Dying;
				// Carga la siguiente escena en 1 s
				Invoke("LoadFirstLevel", loadTime);
				break;
		}
	}

	// Carga la siguiente escena (utilizado con invoke)
	private void LoadNextLevel()
	{
		SceneManager.LoadScene(1);
	}

	// Carga la primera escena (utilizado con invoke)
	private void LoadFirstLevel()
	{
		SceneManager.LoadScene(0);
	}

	private void Audio()
	{
		// Control de sonido
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// Reproduce el sonido del cohete
			audioSource.Play();
		}
		else if (Input.GetKeyUp(KeyCode.Space))
		{
			// Para el sonido del cohete
			audioSource.Stop();
		}
		// Si esta muriendo
		if (state == State.Dying)
		{
			// Para el sonido del cohete
			audioSource.Stop();
		}
	}

	// Propulsión del cohete
	private void Thrust()
	{
		// Movimiento
		if (Input.GetKey(KeyCode.Space))
		{
			// Fuerza relativa (coordenadas locales)
			// a · t como velocidad
			// Cantidad de propulsión para este frame
			// Debe superar a la gravedad para despegar
			rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
		}
	}

	// Rota el cohete
	private void Rotate()
	{
		// Control manual de la rotación
		rigidBody.freezeRotation = true;

		// Cantidad de rotación para este frame
		// r · t es la velocidad de rotación
		float rotationThisFrame = rcsThrust * Time.deltaTime;

		// Rotación izquierda
		if (Input.GetKey(KeyCode.A))
		{
			// Coordenadas locales
			transform.Rotate(Vector3.forward * rotationThisFrame);
		}
		// Rotación derecha
		else if (Input.GetKey(KeyCode.D))
		{
			// Coordenadas locales
			transform.Rotate(-Vector3.forward * rotationThisFrame);
		}

		// Restablece el control de físicas automático
		rigidBody.freezeRotation = false;
	}
}
