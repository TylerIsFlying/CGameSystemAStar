using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleMoveScript : MonoBehaviour
{

    [SerializeField]
    private float speed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float movement = Time.deltaTime * speed;

        Vector3 move = new Vector3(transform.position.x + (horizontal * movement), transform.position.y, transform.position.z + (vertical * movement));
        transform.position = move;

    }
}
