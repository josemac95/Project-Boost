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

	// Estado del cohete
	bool isTransitioning = false;

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
		// Si no está en una transición permite el control
		if (!isTransitioning)
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
			StopThrust();
		}
	}

	// Aplica la propulsión
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

	// Para la propulsión
	private void StopThrust()
	{
		// Para el sonido del cohete
		audioSource.Stop();
		// Partículas del cohete
		mainEngineParticles.Stop();
	}

	// Rota el cohete
	private void RespondToRotateInput()
	{
		// Rotación izquierda
		if (Input.GetKey(KeyCode.A))
		{
			// r · t es la velocidad de rotación
			RotateManually(rcsThrust * Time.deltaTime);
		}
		// Rotación derecha
		else if (Input.GetKey(KeyCode.D))
		{
			RotateManually(-rcsThrust * Time.deltaTime);
		}
	}

	// Rota manualmente
	// rotationThisFrame es la cantidad de rotación para este frame
	private void RotateManually(float rotationThisFrame)
	{
		// Control manual de la rotación
		rigidBody.freezeRotation = true;
		// Coordenadas locales
		transform.Rotate(Vector3.forward * rotationThisFrame);
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
		// Si está en una transición o no hay colisiones, nada
		if (isTransitioning || collisionsDisabled) return;
		// Si está vivo y se puede controlar
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
		isTransitioning = true;
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
		isTransitioning = true;
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
