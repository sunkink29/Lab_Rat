using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;


public class FirstPersonController : MonoBehaviour {

	[SerializeField] KeyCode RunKey = KeyCode.LeftControl;
	[SerializeField] KeyCode CrouchKey = KeyCode.LeftShift;
	[SerializeField] bool playSounds = false;
	[SerializeField] float walkSpeed = 5f;
	[SerializeField] float runSpeed = 10f;
	[SerializeField] [Range(0f, 1f)] float runstepLenghten = 0.5f;
	[SerializeField] float jumpHeight = 5f;
	[SerializeField] float crouchSpeed = 3f;
	[SerializeField] private CameraController cameraRotator = new CameraController();
	[SerializeField] private bool m_LockCursor = false;
	[SerializeField] private CurveControlledBob headBob = new CurveControlledBob();
	[SerializeField] private LerpControlledBob jumpBob = new LerpControlledBob();
	[SerializeField] private float stepInterval;
	[SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
	[SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
	[SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
	[SerializeField] GameObject pauseGameMenu;
	[SerializeField] bool isOnRope;
	[SerializeField] float maxInterationDistance = 1.5f;
	
	Camera playerCamera;
	GameObject neckPivot;
	bool isPaused = false;
	Animator playerCameraAnimater;
	Transform playerTransform;
	Rigidbody playerRigidbody;
	Collider groundObjectCollider;
	Vector3 movement;
	Vector3 originalCameraPosition;
	float targetSpeed = 5f;
	float stepCycle;
	float nextStep;
	bool jump;
	bool previouslyGrounded;
	bool isCrouching;
	bool isOnGround = true;
	bool isWalking;
	bool isRunning;
	AudioSource audioSource;
	GameObject InterationText;
	bool canInteract = false;

	void Start () 
	{
		playerTransform = GetComponent<Transform> ();
		playerRigidbody = GetComponent<Rigidbody> ();
		playerCameraAnimater = GetComponentInChildren<Animator> ();
		audioSource = GetComponent<AudioSource>();
		InterationText = GameObject.Find ("Interation Message");
		playerCamera = Camera.main;
		neckPivot = GameObject.FindWithTag ("Neck Pivot");
		originalCameraPosition = playerCamera.transform.localPosition;
		headBob.Setup(playerCamera, stepInterval);
		stepCycle = 0f;
		nextStep = stepCycle/2f;
		cameraRotator.Setup (neckPivot.transform, playerTransform);
		pauseGameMenu.SetActive (true);

	}

	void Awake()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || 
		    	Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
			Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !m_LockCursor;
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		isOnGround = true;
		groundObjectCollider = other;
	}

	void OnTriggerExit(Collider other)
	{
		if (other == groundObjectCollider) 
		{
			isOnGround = false;
		}
	}

	void OnDisable()
	{
//		Cursor.lockState = CursorLockMode.None;
//		Cursor.visible = true;
	}

	void Update()
	{
		if (m_LockCursor && Input.GetMouseButton(0) && !isPaused 
		    && (Application.platform == RuntimePlatform.WindowsEditor || 
		    Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor))
		{
			Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !m_LockCursor;
		}
//		RotateView();
		jump = Input.GetButtonDown("Jump");
		
		if (!previouslyGrounded && isOnGround)
		{
			StartCoroutine(jumpBob.DoBobCycle());
			PlayLandingSound();
//			m_MoveDir.y = 0f;
//			jumping = false;
		}
		
		previouslyGrounded = isOnGround;
		if (!isPaused)
		{
			//animate ();
			cameraRotator.rotate ();
			CheckForPlayerInterations();
		}
	}

	void FixedUpdate()
	{
		Vector3 h = Input.GetAxisRaw ("Horizontal") * playerTransform.right;
		Vector3 v = Input.GetAxisRaw ("Vertical") * playerCamera.transform.forward;
			
		if (jump && isOnGround)
		{
			playerRigidbody.velocity = new Vector3(0, jumpHeight, 0);
			PlayJumpSound();
			jump = false;
//			jumping = true;
		}

		if (!isPaused) 
		{
			CheckSpeedState ();
			Move (h, v);
			cameraRotator.rotate ();
			UpdateCameraPosition (targetSpeed);
			if (isOnGround)
				ProgressStepCycle (targetSpeed);
		}
	}

 	void ProgressStepCycle(float speed)
	{
		if (movement.sqrMagnitude > 0 && (movement.x != 0 || movement.y != 0))
		{
			stepCycle += (movement.magnitude + (speed*(isWalking ? 1f : runstepLenghten)))*
				Time.fixedDeltaTime;
		}
		
		if (!(stepCycle > nextStep))
		{
			return;
		}
		
		nextStep = stepCycle + stepInterval;
		
		PlayFootStepAudio();
	}

	void PlayFootStepAudio()
	{
		if (!isOnGround)
		{
			return;
		}
		// pick & play a random footstep sound from the array,
		// excluding sound at index 0
		int n = Random.Range(1, m_FootstepSounds.Length);
		audioSource.clip = m_FootstepSounds[n];
		audioSource.PlayOneShot(audioSource.clip);
		// move picked sound to index 0 so it's not picked next time
		m_FootstepSounds[n] = m_FootstepSounds[0];
		m_FootstepSounds[0] = audioSource.clip;
	}

	
	
	void Move (Vector3 h, Vector3 v)
	{
		movement = h + v;
		movement.y = 0;
		movement = movement.normalized * targetSpeed * Time.deltaTime;
		if (!isOnRope)
			playerRigidbody.MovePosition (playerRigidbody.position + movement);
		else
			playerRigidbody.AddForce (movement / Time.deltaTime);
	}

	void CheckSpeedState ()
	{
		if (movement.x != 0 && movement.z != 0) 
		{
			isWalking = true;
			targetSpeed = walkSpeed;
		}
		else
			isWalking = false;

		if (Input.GetKey (RunKey)) 
		{
			isRunning = true;
			isWalking = false;
			targetSpeed = runSpeed;
		}
		else
			isRunning = false;

		if (Input.GetKey (CrouchKey)) 
		{
			isCrouching = true;
			isWalking = true;
			targetSpeed = crouchSpeed;
		}
		else 
			isCrouching = false;
	}

	public void setIsPaused(bool paused) {
		isPaused = paused;
	}

	void UpdateCameraPosition(float speed) {
		Vector3 newCameraPosition;
		movement /= Time.fixedDeltaTime;
		if (movement.magnitude > 0 && isOnGround) {
			playerCamera.transform.localPosition =
				headBob.DoHeadBob(movement.magnitude +
				                    (speed*(isWalking ? 1f : runstepLenghten)));
			newCameraPosition = playerCamera.transform.localPosition;
			newCameraPosition.y = playerCamera.transform.localPosition.y - jumpBob.Offset();
		} else {
			newCameraPosition = playerCamera.transform.localPosition;
			newCameraPosition.y = originalCameraPosition.y - jumpBob.Offset();
		}
		playerCamera.transform.localPosition = newCameraPosition;
	}

	void animate() {
		
		playerCameraAnimater.SetBool ("Is On Ground", isOnGround);
		playerCameraAnimater.SetBool ("Is Crouching", isCrouching);
		playerCameraAnimater.SetBool ("Is Walking", isWalking);
		playerCameraAnimater.SetBool ("Is Running", isRunning);
	}

	void PlayJumpSound() {
		if (playSounds) {
			audioSource.clip = m_JumpSound;
			audioSource.Play ();
		}
	}

	void PlayLandingSound() {
		if (playSounds) {
			audioSource.clip = m_LandSound;
			audioSource.Play ();
			nextStep = stepCycle + .5f;
		}
	}

	void CheckForPlayerInterations() {
		Vector3 centerScreen = new Vector3 (playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2, 0);
		Ray InterationRay = playerCamera.ScreenPointToRay (centerScreen);
		RaycastHit hitInfo;
		Physics.Raycast (InterationRay, out hitInfo);
		Interactible hitObjectScript = hitInfo.transform.gameObject.GetComponent<Interactible> ();
		bool interact = Input.GetButtonDown ("Interact");
		if (hitInfo.distance <= maxInterationDistance && hitObjectScript != null) {
			canInteract = true;
		} else {
			canInteract = false;
		}
		if (interact && canInteract) {
			hitObjectScript.interact();
		}
		InterationText.SetActive (canInteract);
		Debug.DrawLine (InterationRay.origin, InterationRay.origin + (InterationRay.direction * maxInterationDistance));
		//Debug.Log (hitInfo.transform.gameObject);
		//Debug.DrawRay (InterationRay.origin, InterationRay.direction, Color.yellow);
	}
}