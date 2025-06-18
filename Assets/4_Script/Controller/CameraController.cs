using Combat.InputActions;
using UnityEngine;

namespace Combat.Controller
{
	[RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {

		[Header("Camera Settings")]
		[SerializeField] private float cameraBoomLength;
		[SerializeField] private float moveSpeed;
		[SerializeField] private float rotSpeed;
		[SerializeField] private float zoomSpeed;

		[Header("Limits")]
		[SerializeField] private float minOrthoSize;
		[SerializeField] private float maxOrthoSize;

		private new Camera camera;

		private void Awake()
		{

		}

		private void Start()
		{
			camera = GetComponent<Camera>();
		}

		private Vector2 mouseDelta = Vector2.zero;
		private Vector2 forwardVector = Vector2.zero;
		private Vector2 rightVector = Vector2.zero;

		private Vector3 lookatPosition = Vector3.zero;
		private Vector3 targetLookatPosition = Vector3.zero;
		private Vector2 sphericalAngles = new Vector2(-45f, 45f);

		private float targetSize = 20f;
		private float currentSize = 20f;
		private float scrollDelta = 0f;

		private float deltaTime = .01f;

		private void Update()
		{
			CalcVariables();
			SetCameraTransform();
		}

		private void CalcVariables()
		{
			float rotY = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
			rightVector.Set(-Mathf.Sin(rotY), -Mathf.Cos(rotY));
			forwardVector.Set(rightVector.y, -rightVector.x);

			lookatPosition = Vector3.Lerp(lookatPosition, targetLookatPosition, 10 * deltaTime);

			currentSize = Mathf.Lerp(currentSize, targetSize, deltaTime);
		}

		public void SetInputs(bool isLeftPressed, bool isRightPressed, bool initPressed,
			Vector2 mouseDelta, float scrollDelta)
		{
			mouseDelta.y = -mouseDelta.y;
			this.mouseDelta = mouseDelta;

			this.scrollDelta = scrollDelta;

			if (isRightPressed)
			{
				Vector2 tmpV = (forwardVector * mouseDelta.x + -rightVector * mouseDelta.y);
				targetLookatPosition += new Vector3(tmpV.x, 0, tmpV.y) * moveSpeed * deltaTime;
			}
			if (isLeftPressed)
			{
				sphericalAngles += mouseDelta * rotSpeed * deltaTime;
				if (sphericalAngles.x >= 360) sphericalAngles.x -= 360;
				sphericalAngles.y = Mathf.Clamp(sphericalAngles.y, 30f, 89.9f);
			}
			if (initPressed)
			{
				targetLookatPosition = Vector3.zero;
				sphericalAngles = new Vector2(-45f, 45f);
			}
		}

		private void SetCameraTransform()
		{
			float phi = sphericalAngles.x * Mathf.Deg2Rad;
			float theta = sphericalAngles.y * Mathf.Deg2Rad;

			float x = cameraBoomLength * Mathf.Cos(theta) * Mathf.Sin(phi);
			float y = cameraBoomLength * Mathf.Sin(theta);
			float z = cameraBoomLength * Mathf.Cos(theta) * Mathf.Cos(phi);

			Vector3 cameraPosition = lookatPosition + new Vector3(x, y, z);
			transform.position = cameraPosition;

			transform.LookAt(lookatPosition);

			targetSize += -scrollDelta * zoomSpeed * deltaTime;
			targetSize = Mathf.Clamp(targetSize, minOrthoSize, maxOrthoSize);
			camera.orthographicSize = targetSize;
		}

	}
}