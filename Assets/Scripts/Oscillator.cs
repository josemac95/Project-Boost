using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esto hace que solamente se puede añadir un script de este tipo a un objeto
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
	// Posición original
	Vector3 startingPosition;

	// Vector de movimiento
	[SerializeField] Vector3 movementVector;

	// Hace que sea un slider [0, 1]
	[Range(0, 1)]
	// Factor de movimiento
	// 0 posición original, 1 posición final
	[SerializeField] float movementFactor;

	void Start()
	{
		// Posicion inicial
		startingPosition = transform.position;
	}

	void Update()
	{
		// Desplazamiento
		Vector3 offset = movementFactor * movementVector;
		// Mueve el objeto (manual)
		transform.position = startingPosition + offset;
	}
}
