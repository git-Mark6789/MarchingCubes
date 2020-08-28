using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    public class Player : MonoBehaviour
    {
        public float moveSpeed;
        public float rotationSpeed;

        private Vector2 mousePos;
        private float vertical;


        private void LateUpdate()
        {
            // Dirty code for quick movement
            if (Input.GetKey(KeyCode.Space))
                vertical += 5f * Time.deltaTime;
            else if (Input.GetKey(KeyCode.LeftShift))
                vertical -= 5f * Time.deltaTime;
            else
                vertical = 0;

            vertical = Clamp(-1f, 1f, vertical);


            Vector2 mouseMoved = new Vector2(Input.GetAxis("Mouse X") * rotationSpeed,
                Input.GetAxis("Mouse Y") * rotationSpeed);

            mousePos += mouseMoved * Time.deltaTime;
            mousePos.y = Clamp(-90f, 90f, mousePos.y);

            transform.localRotation = Quaternion.AngleAxis(mousePos.x, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(-mousePos.y, Vector3.right);

            // Move in direction we are facing
            transform.Translate(Vector3.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime, Space.Self);
            transform.Translate(Vector3.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, Space.Self);
            transform.Translate(Vector3.up * moveSpeed * vertical * Time.deltaTime, Space.World);
        }

        private float Clamp(float min, float max, float angle)
        {
            if (angle < min)
                angle = min;

            if (angle > max)
                angle = max;

            return angle;
        }
    }
}
