using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeVP
{
    public class ArcadeVehicleController : MonoBehaviour
    {
        public enum surfaceType { drivable, dirt, outOfBounds };
        public enum groundCheck { rayCast, sphereCaste };
        public enum MovementMode { Velocity, AngularVelocity };
        public MovementMode movementMode;
        public groundCheck GroundCheck;
        public LayerMask drivableSurface, dirtTrackSurface, outOfBoundsSurface;

        public float MaxSpeed, accelaration, turn, gravity = 7f, downforce = 5f;
        [Tooltip("if true : can turn vehicle in air")]
        public bool AirControl = false;
        [Tooltip("if true : vehicle will drift instead of brake while holding space")]
        public bool kartLike = false;
        [Tooltip("turn more while drifting (while holding space) only if kart Like is true")]
        public float driftMultiplier = 1.5f;
        public float dirtTrackMultiplier = 0.75f;
        public float outOfBoundsMultiplier = 0.35f;
        public float damagedMultiplier = 0.85f;
        public float naturalDecelerationPercentage = 0.15f;
        float currentDamagedMultiplier = 1f;

        public Rigidbody rb, carBody;

        [HideInInspector]
        public RaycastHit hit;
        public AnimationCurve frictionCurve;
        public AnimationCurve turnCurve;
        public PhysicsMaterial frictionMaterial;
        [Header("Visuals")]
        public Transform BodyMesh;
        public Transform[] FrontWheels = new Transform[2];
        public Transform[] RearWheels = new Transform[2];
        [HideInInspector]
        public Vector3 carVelocity;

        [Range(0, 10)]
        public float BodyTilt;
        [Range(0, 45)]
        public float driftAngle = 30f;


        [Header("Audio settings")]
        public AudioSource engineSound;
        [Range(0, 1)]
        public float minPitch;
        [Range(1, 3)]
        public float MaxPitch;
        public AudioSource SkidSound;

        [HideInInspector]
        public float skidWidth;
        private surfaceType currentSurfaceType;
        private float currentGroundTypeMultiplier = 1f;
        private float radius, steeringInput, accelerationInput;
        private bool brakeInput;
        private Vector3 origin;
        private bool canMove;


        void OnEnable()
        {
            InputManager.onMoveInput += ProvideSteeringInput;
            InputManager.onAccelerationInput += ProvideAccelerationInput;
            InputManager.onBrakeAction += ProvideBrakeInput;
        }

        void OnDisable()
        {
            InputManager.onMoveInput -= ProvideSteeringInput;
            InputManager.onAccelerationInput -= ProvideAccelerationInput;
            InputManager.onBrakeAction -= ProvideBrakeInput;
        }

        private void Start()
        {
            canMove = false;
            currentDamagedMultiplier = 1f;
            radius = rb.GetComponent<SphereCollider>().radius;
            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 100;
            }
        }

        private void Update()
        {
            Visuals();
            AudioManager();
        }

        public void AudioManager()
        {
            engineSound.pitch = Mathf.Lerp(minPitch, MaxPitch, Mathf.Abs(carVelocity.z) / MaxSpeed);
            if (Mathf.Abs(carVelocity.x) > 10 && grounded())
            {
                SkidSound.mute = false;
            }
            else
            {
                SkidSound.mute = true;
            }
        }


        void FixedUpdate()
        {
            if (!canMove)
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, accelaration * Time.deltaTime);
                rb.angularVelocity = Vector3.Slerp(rb.angularVelocity, Vector3.zero, accelaration * Time.deltaTime);
                carVelocity = carBody.transform.InverseTransformDirection(rb.linearVelocity);
                return;
            }

            carVelocity = carBody.transform.InverseTransformDirection(carBody.linearVelocity);

            if (Mathf.Abs(carVelocity.x) > 0)
            {
                //changes friction according to sideways speed of car
                frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));
            }


            if (grounded())
            {

                currentGroundTypeMultiplier = GetGroundTypeMultiplier();


                //turnlogic
                float sign = Mathf.Sign(carVelocity.z);
                float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);
                if (kartLike && brakeInput || currentSurfaceType == surfaceType.dirt) { TurnMultiplyer *= driftMultiplier; } //turn more if drifting


                if (accelerationInput > 0.1f || carVelocity.z > 1)
                {
                    carBody.AddTorque(Vector3.up * steeringInput * sign * turn * 100 * TurnMultiplyer);
                }
                else if (accelerationInput < -0.1f || carVelocity.z < -1)
                {
                    carBody.AddTorque(Vector3.up * steeringInput * sign * turn * 100 * TurnMultiplyer);
                }


                // mormal brakelogic
                if (!kartLike)
                {
                    if (brakeInput)
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    }
                    else
                    {
                        rb.constraints = RigidbodyConstraints.None;
                    }
                }

                //accelaration logic

                if (movementMode == MovementMode.AngularVelocity)
                {
                    if (Mathf.Abs(accelerationInput) > 0.1f && !brakeInput && !kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * accelerationInput * MaxSpeed * currentDamagedMultiplier * currentGroundTypeMultiplier / radius, accelaration * Time.deltaTime);
                    }
                    else if (Mathf.Abs(accelerationInput) > 0.1f && kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * accelerationInput * MaxSpeed * currentDamagedMultiplier * currentGroundTypeMultiplier / radius, accelaration * Time.deltaTime);
                    } else if (!brakeInput)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, naturalDecelerationPercentage * Time.deltaTime);
                    }
                }
                else if (movementMode == MovementMode.Velocity)
                {
                    if (Mathf.Abs(accelerationInput) > 0.1f && !brakeInput && !kartLike)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, carBody.transform.forward * accelerationInput * MaxSpeed * currentDamagedMultiplier * currentGroundTypeMultiplier, accelaration / 10 * Time.deltaTime);
                    }
                    else if (Mathf.Abs(accelerationInput) > 0.1f && kartLike)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, carBody.transform.forward * accelerationInput * MaxSpeed * currentDamagedMultiplier * currentGroundTypeMultiplier, accelaration / 10 * Time.deltaTime);
                    } else if (!brakeInput)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity * naturalDecelerationPercentage, Time.deltaTime);
                    }
                }

                // down force
                rb.AddForce(-transform.up * downforce * rb.mass);

                //body tilt
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
            }
            else
            {
                if (AirControl)
                {
                    //turnlogic
                    float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);

                    carBody.AddTorque(Vector3.up * steeringInput * turn * 100 * TurnMultiplyer);
                }

                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity + Vector3.down * gravity, Time.deltaTime * gravity);
            }

        }
        public void Visuals()
        {
            //Don't update visual to emulate wheel rotation on replay of recorded data
            if (!canMove)
                return;


            //tires
            foreach (Transform FW in FrontWheels)
            {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                                   30 * steeringInput, FW.localRotation.eulerAngles.z), 0.7f * Time.deltaTime / Time.fixedDeltaTime);
                FW.GetChild(0).localRotation = rb.transform.localRotation;
            }
            RearWheels[0].localRotation = rb.transform.localRotation;
            RearWheels[1].localRotation = rb.transform.localRotation;

            //Body
            if (carVelocity.z > 1)
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / MaxSpeed),
                                   BodyMesh.localRotation.eulerAngles.y, BodyTilt * steeringInput), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }
            else
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }


            if (kartLike || currentSurfaceType == surfaceType.dirt)
            {
                if (brakeInput || currentSurfaceType == surfaceType.dirt)
                {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, driftAngle * steeringInput * Mathf.Sign(carVelocity.z), 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                }
                else
                {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, 0, 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                }

            }

        }

        public bool grounded() //checks for if vehicle is grounded or not
        {
            origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

            if (GroundCheck == groundCheck.rayCast)
            {
                if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
                {
                    return true;
                }
                else if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, dirtTrackSurface))
                {
                    return true;
                }
                else if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, outOfBoundsSurface))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (GroundCheck == groundCheck.sphereCaste)
            {
                if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface))
                {
                    return true;

                }
                else if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, dirtTrackSurface))
                {
                    return true;

                }
                else if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, outOfBoundsSurface))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else { return false; }
        }

        public float GetGroundTypeMultiplier()
        {
            var direction = -transform.up;
            origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
            var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;
            if (GroundCheck == groundCheck.rayCast)
            {
                if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
                {
                    currentSurfaceType = surfaceType.drivable;
                    return 1f;
                }
                else if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, dirtTrackSurface))
                {
                    currentSurfaceType = surfaceType.dirt;
                    return dirtTrackMultiplier;
                }
                else if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, outOfBoundsSurface))
                {
                    currentSurfaceType = surfaceType.outOfBounds;
                    return outOfBoundsMultiplier;
                }
                else
                {
                    Debug.Log("Ground type not detected");
                    return 1f;
                }
            }

            else if (GroundCheck == groundCheck.sphereCaste)
            {
                if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface))
                {
                    currentSurfaceType = surfaceType.drivable;
                    return 1f;

                }
                else if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, dirtTrackSurface))
                {
                    currentSurfaceType = surfaceType.dirt;
                    return dirtTrackMultiplier;

                }
                else if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, outOfBoundsSurface))
                {
                    currentSurfaceType = surfaceType.outOfBounds;
                    return outOfBoundsMultiplier;

                }
                else
                {
                    return 1f;
                }
            }
            else
            {
                return 1f;
            }
        }

        private void OnDrawGizmos()
        {
            //debug gizmos
            radius = rb.GetComponent<SphereCollider>().radius;
            float width = 0.02f;
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2 * width, 4 * radius));
                if (GetComponent<BoxCollider>())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
                }

            }

        }

        private void ProvideSteeringInput(float input)
        {
            steeringInput = input;
        }

        private void ProvideAccelerationInput(float input)
        {
            accelerationInput = input;
        }

        private void ProvideBrakeInput(bool input)
        {
            brakeInput = input;
        }

        public void SetCanMove(bool value)
        {
            canMove = value;
        }

        public void ReactivateComponent(bool canMove)
        {
            currentDamagedMultiplier = 1f;
            this.canMove = canMove;
            //StartCoroutine(ReactivateComponentCoroutine(canMove));
        }

        // IEnumerator ReactivateComponentCoroutine(bool canMove)
        // {
        //     carVelocity = Vector3.zero;
        //     rb.linearVelocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        //     yield return new WaitForSeconds(1f);
        //     this.canMove = canMove;
        // }

        public void ForceWheelRotation(float distanceDiff, float steering)
        {
            float rbRadius = rb.GetComponent<SphereCollider>().radius;
            float rotationAmount = distanceDiff / rbRadius * (360 / (2 * Mathf.PI));
            foreach (Transform FW in FrontWheels)
            {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                                    30 * steering, FW.localRotation.eulerAngles.z), 0.7f * Time.deltaTime / Time.fixedDeltaTime);
                FW.GetChild(0).Rotate(rotationAmount, 0, 0);
            }
            foreach (Transform RW in RearWheels)
            {
                RW.Rotate(rotationAmount, 0, 0);
            }
        }

        public float GetCurrentSpeed()
        {
            return carVelocity.magnitude;
        }

        public void Impacted()
        {
            currentDamagedMultiplier = damagedMultiplier;
        }

    }
}
