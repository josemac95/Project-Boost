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

	// Tiempo de carga
	float loadTime = 1f;

	// Estados del cohete
	enum State { Alive, Dying, Trascending }
	// Estado actual del cohete
	State state = State.Alive;

	// Colisiones desactivadas
	bool collisionsDisabled = false;

	// Clip de audio para el motor del cohete
	[SerializeField] AudioClip mainEngineSound;
	// Clip de audio para la victoria
	[SerializeField] AudioClip victorySound;
	// Clip de audio para la muerte
	[SerializeField] AudioClip deathSound;

	// Para el sistema de partículas, hay que instanciar los prefabs de partículas
	// y desde el inspector se asocian las instancias en la escena

	// Partículas para el motor del cohete
	[SerializeField] ParticleSystem mainEngineParticles;
	// Partículas para la victoria
	[SerializeField] ParticleSystem victoryParticles;
	// Partículas para la muerte
	[SerializeField] ParticleSystem deathParticles;

	void Start()
	{
		// Versión gameObject.* hace lo mismo, pero es diferente
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		// Si está vivo permite el control
		if (state == State.Alive)
		{
			RespondToThrustInput();
			RespondToRotateInput();
		}
		// Si es una build debug (una casilla antes de hacer la build)
		if (Debug.isDebugBuild)
		{
			RespondToDebugKeys();
		}
	}

	// Propulsión del cohete
	private void RespondToThrustInput()
	{
		// Movimiento
		if (Input.GetKey(KeyCode.Space))
		{
			ApplyThrust();
		}
		else
		{
			// Para el sonido del cohete
			audioSource.Stop();
			// Partículas del cohete
			mainEngineParticles.Stop();
		}
	}

	private void ApplyThrust()
	{
		// Fuerza relativa (coordenadas locales)
		// a · t como velocidad
		// Cantidad de propulsión para este frame
		// Debe superar a la gravedad para despegar
		rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
		// Si no se está reproduciendo un audio
		// => No poner el loop en AudioSource
		// => No hay solapamiento de audio
		if (!audioSource.isPlaying)
		{
			// Reproduce el sonido del cohete
			audioSource.PlayOneShot(mainEngineSound);
		}
		// Tras el sonido, las partículas
		mainEngineParticles.Play();
	}

	// Rota el cohete
	private void RespondToRotateInput()
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

	// Responde a las teclas debug para facilitar el testing
	private void RespondToDebugKeys()
	{
		// L para cargar el siguiente nivel
		if (Input.GetKeyDown(KeyCode.L))
		{
			LoadNextLevel();
		}
		// C para activar/desactivar las colisiones
		else if (Input.GetKeyDown(KeyCode.C))
		{
			collisionsDisabled = !collisionsDisabled;
		}
	}

	// Si choca con un objeto
	void OnCollisionEnter(Collision collision)
	{
		// Si no está vivo, nada
		if (state != State.Alive || collisionsDisabled) return;
		// Si está vivo
		switch (collision.gameObject.tag)
		{
			case "Friendly":
				// No pasa nada
				break;
			case "Finish":
				StartVictorySecuence();
				break;
			default:
				StartDeathSecuence();
				break;
		}
	}

	// Secuencia para la victoria
	private void StartVictorySecuence()
	{
		state = State.Trascending;
		// Sonido de victoria
		audioSource.Stop();
		audioSource.PlayOneShot(victorySound);
		// Partículas de victoria
		mainEngineParticles.Stop();
		victoryParticles.Play();
		// Carga la siguiente escena en 1 s
		Invoke("LoadNextLevel", loadTime);
	}

	// Secuencia para la muerte

	private void StartDeathSecuence()
	{
		// Mueres
		state = State.Dying;
		// Sonido de muerte
		audioSource.Stop();
		audioSource.PlayOneShot(deathSound);
		// Partículas de muerte
		mainEngineParticles.Stop();
		deathParticles.Play();
		// Carga la siguiente escena en 1 s
		Invoke("LoadFirstLevel", loadTime);
	}

	// Carga la siguiente escena (utilizado con invoke)
	private void LoadNextLevel()
	{
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
		SceneManager.LoadScene(nextSceneIndex);
	}

	// Carga la primera escena (utilizado con invoke)
	private void LoadFirstLevel()
	{
		SceneManager.LoadScene(0);
	}
}
