using UnityEngine;

public class Gift : MonoBehaviour
{
    public GrassManager grassManager;
    public void DestroyGift(AnimationEvent other)
    {
        Debug.Log("Appel de DestroyGift avec le paramètre : " + other);

        if (other != null)
        {
            Debug.Log("Paramètre string de l'AnimationEvent: " + other.stringParameter);
                       
            Debug.Log("Objet cadeau trouvé : " + this.name);
            //if (this.name == "Gift2")
            {
                Animator changeAnim = GetComponent<Animator>();
                if (changeAnim != null)
                {
                    changeAnim.SetBool("Explode", false);
                    Transform coordCaisse = this.transform;
                    Debug.Log("Coordonnées de la caisse : " + coordCaisse);
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
            Debug.LogError("Paramètre AnimationEvent est null.");
        }
    }
}
