using UnityEngine;

public class fly : MonoBehaviour
{

    [SerializeField]  float movementSpeed = 7f;
    Rigidbody rb;
    private void Start()
    {
        rb=GetComponent<Rigidbody>();
    }
    void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal")*1.5f, Input.GetAxis("Depth")*1.5f, Input.GetAxis("Vertical"));
       // movementSpeed = Mathf.Max(movementSpeed += Input.GetAxis("Mouse ScrollWheel"), 0.0f);
        Vector3 targetV=((transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * Input.GetAxis("Depth")) * movementSpeed);
        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"),0);
        Cursor.lockState = CursorLockMode.Locked;
        if (movement == Vector3.zero)
            targetV = Vector3.zero;
            rb.velocity = Vector3.Lerp(rb.velocity, targetV, 0.99f * Time.deltaTime);
    }
}
