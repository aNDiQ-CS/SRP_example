using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class LitMaterialParamsOverride : MonoBehaviour {
	
	static int baseColorId = Shader.PropertyToID("_BaseColor");
	static int metalnessId = Shader.PropertyToID("_Metalness");
	static int roughnessId = Shader.PropertyToID("_Roughness");
	static MaterialPropertyBlock block;
	
	[SerializeField]
	private Color baseColor = Color.white;

	[SerializeField]
	[Range(0, 1)]
	private float roughness = 0.5f;

	[SerializeField]
	[Range(0, 1)]
	private float metalness = 0.0f;

	void Awake () {
		OnValidate();
	}

	void OnValidate () {
		if (block == null) {
			block = new MaterialPropertyBlock();
		}
		block.SetColor(baseColorId, baseColor);
		block.SetFloat(roughnessId, roughness);
		block.SetFloat(metalnessId, metalness);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}