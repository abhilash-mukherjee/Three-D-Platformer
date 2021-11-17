using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyMovement : MonoBehaviour
{
        // Variables
        [SerializeField] private float moveSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpDistance;
        [SerializeField] private float jumpHeight;

        private Vector3 movement;
        private Vector3 velocity;

        public float maxGroundAngle = 120;
        public bool debug;
        public float slopeLimit = 45f;

        [SerializeField] private bool isGrounded;
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float gravity;

        // References
        private CharacterController controller;
        private Animator anim;

        private void Start() {
            controller = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            Move();

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jumping();
            }
        }

        private void Move()
        {
            isGrounded = false;
            float capsuleHeight = controller.height;
            Vector3 capsuleBottom = transform.TransformPoint(controller.center - (Vector3.up * capsuleHeight / 2f));
            float radius = transform.TransformVector(controller.radius, 0f, 0f).magnitude;

            Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, radius * 5f))
            {
                float normalAngle = Vector3.Angle(hit.normal, transform.up);
                if (normalAngle < slopeLimit)
                {
                    float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                    if (hit.distance < maxDist)
                        isGrounded = true;
                }
            }

            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            movement = new Vector3(0, 0, Input.GetAxis("Horizontal")) * moveSpeed;

            if(isGrounded)
            {
                if(movement != Vector3.zero && !Input.GetKey(KeyCode.RightShift))
                {
                    Walk();
                }
                else if(movement != Vector3.zero && Input.GetKey(KeyCode.RightShift))
                {
                    Run();
                }
                else if(movement == Vector3.zero)
                {
                    Idle();
                }
                else if (velocity.y < 0)
                 anim.SetTrigger("Falling");

            }

            //Turns player around
            if (movement != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement.normalized), 0.03f);

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }



            // else if (velocity.y < 0)
            // {
            //     Falling();
            // }

            GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;

            controller.Move(movement * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

        }

        private void Idle()
        {
            anim.SetFloat("Speed", 0, 0.5f, Time.deltaTime);
        }

        private void Walk()
        {
            moveSpeed = walkSpeed;
            anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
        }

        private void Run()
        {
            moveSpeed = runSpeed;
            anim.SetFloat("Speed", 1, 0.2f, Time.deltaTime);
        }

        private void Jump()
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -1 * gravity);
            anim.SetFloat("Speed", 2, 0.1f, Time.deltaTime);
        }

        private void Jumping()
        {
            anim.SetTrigger("Jumping");
        }

        // private void Falling()
        // {
        //     anim.SetTrigger("Falling");
        // }
}
