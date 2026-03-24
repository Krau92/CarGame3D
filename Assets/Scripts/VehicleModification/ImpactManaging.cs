using System;
using ArcadeVP;
using UnityEngine;
using System.Threading.Tasks;

public class ImpactManaging : MonoBehaviour
{
    public ArcadeVehicleController vehicleController;
    public AudioClip impactClip;
    public AudioSource audioSource;
    public MeshFilter meshFilter;
    public Mesh mesh;
    public bool reset = false;

    [Header("Deformation Settings")]
    public float strength = 0.05f;
    public float maxDistance = 1f;

    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;
    private volatile bool hasResult;
    private Vector3[] resultVertices;

    [SerializeField] private float impactThreshold = 10f;

    void OnEnable()
    {
        if(CircuitManager.Instance != null)
        {
            CircuitManager.Instance.OnRestartCircuit += ResetDeformation;
        }
    }

    void OnDisable()
    {
        if(CircuitManager.Instance != null)
        {
            CircuitManager.Instance.OnRestartCircuit -= ResetDeformation;
        }
    }

    void Awake()
    {
        mesh = Instantiate(meshFilter.sharedMesh);
        meshFilter.sharedMesh = mesh;

        originalVertices = mesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        Array.Copy(originalVertices, modifiedVertices, originalVertices.Length);
    }

    void Update()
    {
        if (reset)
        {
            Array.Copy(originalVertices, modifiedVertices, originalVertices.Length);
            ApplyToMesh(modifiedVertices);
            reset = false;
        }

        if (hasResult)
        {
            hasResult = false;
            modifiedVertices = resultVertices;
            ApplyToMesh(modifiedVertices);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (vehicleController.carVelocity.magnitude > impactThreshold)
        {
            if (audioSource != null && impactClip != null)
            {
                audioSource.PlayOneShot(impactClip);
            }
            vehicleController.Impacted();
            var contactPoint = collision.GetContact(0);
            DeformMeshAsync(contactPoint.point, -contactPoint.normal, strength, maxDistance);
        }
    }

    public void DeformMeshAsync(Vector3 impactPointWS, Vector3 impactNormalWS, float impactStrength, float impactMaxDistance)
    {

        Vector3 impactPointLS = transform.InverseTransformPoint(impactPointWS);
        Vector3 impactNormalLS = transform.InverseTransformDirection(impactNormalWS).normalized;

        Vector3[] baseVerts = new Vector3[modifiedVertices.Length];
        Array.Copy(modifiedVertices, baseVerts, modifiedVertices.Length);

        Task.Run(() =>
        {
            for (int i = 0; i < baseVerts.Length; i++)
            {
                float dist = Vector3.Distance(baseVerts[i], impactPointLS);
                
                //If it's too far from impact, skip it
                if (dist > impactMaxDistance) continue;

                float t = Mathf.Clamp01(dist / impactMaxDistance);
                float w = Mathf.SmoothStep(1f, 0f, t);
                
                // Modify vertex to simulate impact deformation
                baseVerts[i] -= impactNormalLS * (impactStrength * w);
            }

            resultVertices = baseVerts;
            hasResult = true;
        });
    }

    void ApplyToMesh(Vector3[] vertices)
    {
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void ResetDeformation()
    {
        reset = true;
    }

}