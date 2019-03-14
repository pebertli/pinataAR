using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    public Collider AttackTriggerInstance;

    private const float _attackCooldownConstant = 0.75f;
    private const float _attackDelayConstant = 0.2f;

    private Animator _anim;
    private float _attackDelay = 0.2f;
    private float _attackCooldown = 0f;
    private bool _attacking = false;

    // Use this for initialization
    void Start()
    {
        _anim = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        //attack or collect candies
        if (GameController.Instance.Tool == GameController.PlayerTool.Bate)
        {
            //simulate time to get ready to another hit
            if (_attackCooldown > 0)
            {
                _attackCooldown -= Time.deltaTime;

            }
            else
            {
                AttackTriggerInstance.enabled = false;
            }
            //allowd to attack
            if (Input.GetMouseButtonDown(0) && _attackCooldown <= 0)
            {
                _attacking = true;
                _attackCooldown = _attackCooldownConstant;
                //choose an animation randomly
                int attackAnimation = Random.Range(1, 4);
                _anim.SetTrigger("attack" + attackAnimation);
            }

            if (_attacking)
            {
                // a delay to activate the hit boundbox
                if (_attackDelay > 0)
                    _attackDelay -= Time.deltaTime;
                else
                {
                    AttackTriggerInstance.enabled = true;
                    _attackDelay = _attackDelayConstant;
                    _attacking = false;
                }
            }
        }
        else if (GameController.Instance.Tool == GameController.PlayerTool.Hand)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //filter to candies
                if (Physics.Raycast(ray, out hit, 20.0f, 1 << 10 | 1 << 11 | 1 << 12))
                {
                    if (hit.transform.gameObject.CompareTag("Candy"))
                    {
                        CandyController c = hit.transform.gameObject.GetComponent<CandyController>();
                        //got a candy
                        GameController.Instance.AddScore(c.Points);
                        c.Destroy();
                    }
                    else if (hit.transform.gameObject.CompareTag("StarCandy"))
                        //got a star
                        hit.transform.gameObject.GetComponent<StarCandyController>().Damage(1);
                    else if (hit.transform.gameObject.CompareTag("BadCandy"))
                        //EventManager.TriggerEvent("Activate" + hit.transform.gameObject.name);
                        hit.transform.gameObject.GetComponent<BadCandyController>().Activate();

                }
            }

        }
    }
}
