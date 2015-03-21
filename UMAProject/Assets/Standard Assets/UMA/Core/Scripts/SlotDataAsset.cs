using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace UMA
{
	[System.Serializable]
	public partial class SlotDataAsset : ScriptableObject, ISerializationCallbackReceiver
	{
		public string slotName;
		[System.NonSerialized]
		public int nameHash;

		[UMAAssetFieldVisible]
		public UMAMaterial material;

#if !UMA2_LEAN_AND_CLEAN 
		public string[] textureNameList;
		public SkinnedMeshRenderer meshRenderer;
		public Material materialSample;
#endif

		public float overlayScale = 1.0f;
		public Transform[] animatedBones = new Transform[0];

		public DnaConverterBehaviour slotDNA;
		public UMAMeshData meshData;
		public int subMeshIndex;
		/// <summary>
		/// Use this to identify slots that serves the same purpose
		/// Eg. ChestArmor, Helmet, etc.
		/// </summary>
		public string slotGroup;
		/// <summary>
		/// Use this to identify what kind of overlays fit this slotData
		/// Eg. BaseMeshSkin, BaseMeshOverlays, GenericPlateArmor01
		/// </summary>
		public string[] tags;
		public UMADataEvent CharacterBegun;
		public UMADataSlotMaterialRectEvent SlotAtlassed;
		public UMADataEvent DNAApplied;
		public UMADataEvent CharacterCompleted;
        
       
		public SlotDataAsset()
		{
            
		}

		public int GetTextureChannelCount(UMAGeneratorBase generator)
		{
			return material.channels.Length;
		}
        
		public override string ToString()
		{
			return "SlotData: " + slotName;
		}

#if UNITY_EDITOR
		public void UpdateMeshData(SkinnedMeshRenderer meshRenderer)
		{
			meshData = new UMAMeshData();
			meshData.RetrieveDataFromUnityMesh(meshRenderer);
			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
		public void OnAfterDeserialize()
		{
			nameHash = UMASkeleton.StringToHash(slotName);
		}
		public void OnBeforeSerialize() { }
	}
}
