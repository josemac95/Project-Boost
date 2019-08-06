using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
	void Start()
	{

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
			print("Thrusting");
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
