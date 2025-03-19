using UnityEngine;

public class Gift : MonoBehaviour
{
    public GrassManager grassManager;
    public void DestroyGift(AnimationEvent other)
    {
        Debug.Log("Appel de DestroyGift avec le param�tre : " + other);

        if (other != null)
        {
            Debug.Log("Param�tre string de l'AnimationEvent: " + other.stringParameter);
                       
            Debug.Log("Objet cadeau trouv� : " + this.name);
            //if (this.name == "Gift2")
            {
                Animator changeAnim = GetComponent<Animator>();
                if (changeAnim != null)
                {
                    changeAnim.SetBool("Explode", false);
                    Transform coordCaisse = this.transform;
                    Debug.Log("Coordonn�es de la caisse : " + coordCaisse);
                    Destroy(gameObject);
                    grassManager.PopGrass(coordCaisse);
                }
                else
                {
                    Debug.LogError("Composant Animator introuvable sur " + name);
                }
            }           
        }
        else
        {
            Debug.LogError("Param�tre AnimationEvent est null.");
        }
    }
}
