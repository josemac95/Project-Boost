using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esto hace que solamente se puede añadir un script de este tipo a un objeto
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
	// Posición original
	Vector3 startingPosition;

	// Tau (2 · Pi)
	const float tau = 2f * Mathf.PI;
	// Fase desplazada -Pi / 2
	const float phase = -Mathf.PI / 2f;

	// Periodo del movimiento
	[SerializeField] float period = 10f;

	// Vector de movimiento
	[SerializeField] Vector3 movementVector = new Vector3(0, -10f, 0);

	void Start()
	{
		// Posicion inicial
		startingPosition = transform.position;
	}

	void Update()
	{
		// Protección contra NaN (not a number)
		// Versión float para la división por 0
		if (period <= Mathf.Epsilon) return;
		// Número de ciclos (crece continuamente desde 0)
		// Es t · f (tiempo por frecuencia)
		float cycles = Time.time / period;
		// Valor de la onda sinusoidal
		float rawSinWave = Mathf.Sin(cycles * tau + phase);
		// Factor de movimiento automático
		// De [-1, 1] a [0, 1]
		float movementFactor = (1f + rawSinWave) / 2f;
		// Desplazamiento
		Vector3 offset = movementFactor * movementVector;
		// Mueve el objeto
		transform.position = startingPosition + offset;
	}
}
