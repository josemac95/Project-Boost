using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
	// Componente RigidBody del cohete
	Rigidbody rigidBody;

	// Intensidad de la fuerza vertical (local)
	[SerializeField]
	float f = 10f;

	void Start()
	{
		// Versión gameObject.* hace lo mismo, pero es diferente
		rigidBody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		ProcessInput();
	}

	// Procesa la entrada del usuario
	private void ProcessInput()
	{
		// Movimiento
		if (Input.GetKey(KeyCode.Space))
		{
			// Vector up como fuerza relativa
			rigidBody.AddRelativeForce(f * Vector3.up);
		}
		// Rotación
		if (Input.GetKey(KeyCode.A))
		{
			print("Rotating left");
		}
		else if (Input.GetKey(KeyCode.D))
		{
			print("Rotating right");
		}
	}
}
