using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloqueRoto : MonoBehaviour
{
    public float VidaBloque = 10f;
    public float explosionForce = 2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BolaTanque"))
        {
            try
            {
                float ballMass = collision.gameObject.GetComponent<Rigidbody>().mass;
                float ballSpeed = collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                float fuerzaImpacto = (ballMass / 10 * ballSpeed / 10);
                
                Quaternion originalRotation = gameObject.transform.rotation;

                VidaBloque -= fuerzaImpacto;

                if (VidaBloque <= 0)
                {
                    // Dividir el rect�ngulo original en rect�ngulos m�s peque�os para llenar su espacio
                    Vector3 originalScale = gameObject.transform.localScale;
                    Vector3 originalPosition = gameObject.transform.position;

                    float newRectWidth = originalScale.x / 2f;
                    float newRectHeight = originalScale.y / 2f;
                    float newRectDepth = originalScale.z / 2f;

                    // Calcular el n�mero de rect�ngulos en cada direcci�n
                    int numRectsX = Mathf.CeilToInt(originalScale.x / newRectWidth);
                    int numRectsY = Mathf.CeilToInt(originalScale.y / newRectHeight);
                    int numRectsZ = Mathf.CeilToInt(originalScale.z / newRectDepth);

                    // Crear los nuevos rect�ngulos
                    for (int x = 0; x < numRectsX; x++)
                    {
                        for (int y = 0; y < numRectsY; y++)
                        {
                            for (int z = 0; z < numRectsZ; z++)
                            {
                                GameObject smallRect = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                //smallRect.AddComponent<Rigidbody>();

                                // Posicionar el nuevo rect�ngulo en el espacio del rect�ngulo original
                                float newX = originalPosition.x - (originalScale.x / 2f) + (newRectWidth / 2f) + (x * newRectWidth);
                                float newY = originalPosition.y - (originalScale.y / 2f) + (newRectHeight / 2f) + (y * newRectHeight);
                                float newZ = originalPosition.z - (originalScale.z / 2f) + (newRectDepth / 2f) + (z * newRectDepth);
                                smallRect.transform.position = new Vector3(newX, newY, newZ);

                                // Establecer la escala del nuevo rect�ngulo
                                if (x == numRectsX - 1)
                                {
                                    smallRect.transform.localScale = new Vector3(originalScale.x - (newRectWidth * (numRectsX - 1)), newRectHeight, newRectDepth);
                                }
                                else if (y == numRectsY - 1)
                                {
                                    smallRect.transform.localScale = new Vector3(newRectWidth, originalScale.y - (newRectHeight * (numRectsY - 1)), newRectDepth);
                                }
                                else if (z == numRectsZ - 1)
                                {
                                    smallRect.transform.localScale = new Vector3(newRectWidth, newRectHeight, originalScale.z - (newRectDepth * (numRectsZ - 1)));
                                }
                                else
                                {
                                    smallRect.transform.localScale = new Vector3(newRectWidth, newRectHeight, newRectDepth);
                                }

                                // Agregar esta l�nea para establecer la rotaci�n del nuevo bloque
                                smallRect.transform.rotation = originalRotation;

                                // A�adir un Rigidbody al nuevo rect�ngulo
                                Rigidbody smallRectRb = smallRect.AddComponent<Rigidbody>();

                                // Establecer la masa del nuevo rect�ngulo
                                smallRectRb.mass = gameObject.GetComponent<Rigidbody>().mass;

                                // Aplicar una fuerza de explosi�n al nuevo rect�ngulo
                                smallRectRb.AddExplosionForce(explosionForce, gameObject.transform.position, originalScale.magnitude, fuerzaImpacto, ForceMode.Impulse);
                                smallRect.AddComponent<DestroyAfterTime>();
                            }
                        }
                    }

                    // Destruir el rect�nguloa original
                    Destroy(gameObject);
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogError("Se produjo una excepci�n NullReferenceException al intentar obtener la masa y la velocidad de la bola de tanque. " + ex.Message);
            }
        }
    }
}