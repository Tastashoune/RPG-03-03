using UnityEngine;

public class Gift1 : MonoBehaviour
{
    public GrassManager grassManager;

    /*
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision Gift1 : " + collision.gameObject);

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            bool isAttacking = playerController.isAttacking;
            if(isAttacking)
            {
                Debug.Log("attaque ! (Gift1.cs l.15)" + collision);
                GameObject gift1 = GameObject.Find("Gift1");
                Animator changeAnim = gift1.GetComponent<Animator>();
                changeAnim.SetBool("Explode", true);
            }
        }
    }
    */

    public void DestroyGift(AnimationEvent other)
    {
        //Debug.Log("Appel de DestroyGift avec le paramètre : " + other);

        if (other != null)
        {
            //Debug.Log("Paramètre string de l'AnimationEvent: " + other.stringParameter);
                       
            //Debug.Log("Objet cadeau trouvé : " + this.name);
            if (this.name == "Gift1" || this.name == "Gift2")
            {
                Animator changeAnim = GetComponent<Animator>();
                if (changeAnim != null)
                {
                    changeAnim.SetBool("Explode", false);
                    Transform coordCaisse = this.transform;
                    Debug.Log("Coordonnées de la caisse : "+ coordCaisse);
                    Destroy(gameObject);
                    //Instantiate()
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