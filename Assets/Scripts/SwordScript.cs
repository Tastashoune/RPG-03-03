using System.Collections;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public void Start()
    {
        BoxCollider2D sword = GetComponent<BoxCollider2D>();
        sword.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision ok avec l'épée : "+other.name);

        //if (other.name == "Gift1" || other.name == "Gift2")
        // tous les gameobjects qui contiennent "Gift" dans leurs noms
        if (other.name.Contains("Gift"))
        {
            GameObject gift = other.gameObject; // GameObject.Find(other.name);
            /*
             * ne marche pas :
            SpriteRenderer sr = gift.GetComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("vide");
            Debug.Log(Resources.Load<Sprite>("vide"));
            */
            SpriteRenderer sr = gift.GetComponent<SpriteRenderer>();
            Color currentColor = sr.color; // Récupérer la couleur actuelle
            currentColor.a = 0f; // Modifier l'alpha (0 = transparent, 1 = opaque)
            sr.color = currentColor; // Appliquer la nouvelle couleur
            //gift.transform.position += new Vector3(0, 1);

            Animator changeAnim = gift.GetComponent<Animator>();
            changeAnim.SetBool("Explode", true);
            // décalage offset explosion
            currentColor.a = 1f; // Modifier l'alpha (0 = transparent, 1 = opaque)
            sr.color = currentColor; // Appliquer la nouvelle couleur
            //yield return new WaitForSeconds(1f); // Attendre 2 secondes
        }

        if (other.name.Contains("Enemy"))
        {
            Debug.Log("attaque sur "+other.name);

            // recul de l'ennemi
            GameObject enemy = other.gameObject; // Find(other.name);
            float directionEnemy = enemy.transform.localScale.x;
            Rigidbody2D rbe = enemy.GetComponent<Rigidbody2D>();
            Debug.Log("rigidbody enemy = "+rbe);
            Vector2 force = (directionEnemy > 0) ? Vector2.right * 2f : Vector2.left * 2f;
            rbe.AddForce(force, ForceMode2D.Impulse);

            // clignotement de l'ennemi
            SpriteRenderer spr = enemy.GetComponent<SpriteRenderer>();
            //spr.color = new Color(1f, 0f, 0f, .5f);
            StartCoroutine(FlashRed(spr));
            // takedamage de l'ennemi
            EnemyCombatSystem ecs = other.GetComponent<EnemyCombatSystem>();
            ecs.TakeDamage(10);
        }
    }
    IEnumerator FlashRed(SpriteRenderer sprite)
    {
        if(sprite!=null)
        {
            Color originalColor = sprite.color; // Sauvegarder la couleur d'origine
            sprite.color = new Color(1f, 0f, 0f, 0.5f); // Rouge semi-transparent
            yield return new WaitForSeconds(.2f); // Attendre 0.2 seconde
            sprite.color = originalColor; // Restaurer la couleur d'origine
            yield return new WaitForSeconds(.2f); // Attendre 0.2 seconde
            sprite.color = new Color(1f, 0f, 0f, 0.5f); // Rouge semi-transparent
            yield return new WaitForSeconds(.2f); // Attendre 0.2 seconde
            sprite.color = originalColor; // Restaurer la couleur d'origine
        }
    }
}
