using UnityEngine;

public class ParticleMeshEmitter : MonoBehaviour
{
	public Mesh emitterMesh;

	private GameObject emitterObject;

	private ParticleSystem emitterSystem;

	private Vector3[] vertices;

	private bool emit;

	private float emissionTimer;

	public void Initialize(GameObject effectTargetObj)
	{
		emitterObject = effectTargetObj;
		emitterMesh = effectTargetObj.GetComponent<MeshFilter>().mesh;
		emitterSystem = GetComponent<ParticleSystem>();
	}

	public void Play()
	{
		emit = true;
	}

	public void Stop()
	{
		emit = false;
	}

	public void ParticleUpdate(bool updateMesh)
	{
		if (!emit)
		{
			return;
		}
		if (emissionTimer >= 1f / emitterSystem.emissionRate)
		{
			if (updateMesh)
			{
				UpdateEmitterMesh();
			}
			EmitParticle();
			emissionTimer = Main.m_gameDeltaTime;
		}
		else
		{
			emissionTimer += Main.m_gameDeltaTime;
		}
	}

	private void UpdateEmitterMesh()
	{
		vertices = emitterMesh.vertices;
	}

	private void EmitParticle()
	{
		Vector3 position = emitterObject.transform.TransformPoint(vertices[Random.Range(0, vertices.Length)]);
		GetComponent<ParticleSystem>().Emit(position, new Vector3(0f, 0f, 0f), emitterSystem.startSize, emitterSystem.startLifetime, emitterSystem.startColor);
	}
}
