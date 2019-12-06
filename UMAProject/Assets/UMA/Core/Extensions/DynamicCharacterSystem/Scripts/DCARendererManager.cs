﻿using System.Collections.Generic;
using UnityEngine;

namespace UMA.CharacterSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DynamicCharacterAvatar))]
    public class DCARendererManager : MonoBehaviour
    {
        [System.Serializable]
        public class RendererElement
        {
            public List<UMARendererAsset> rendererAssets = new List<UMARendererAsset>();
            public List<SlotDataAsset> slotAssets = new List<SlotDataAsset>();
            public List<string> wardrobeSlots = new List<string>();
        }
        public List<RendererElement> RendererElements = new List<RendererElement>();

        private DynamicCharacterAvatar avatar;
        private UMAData.UMARecipe umaRecipe = new UMAData.UMARecipe();
        List<SlotDataAsset> wardrobeSlotAssets = new List<SlotDataAsset>();
        private UMAContextBase context;
        private List<SlotData> slotsToAdd = new List<SlotData>();

#pragma warning disable 0414
        //for use with the editor to save the settings for whether to show the help box or not.
        //pragma disables the warning for not being used in this class.
        [SerializeField]
        private bool showHelp = true; 
#pragma warning restore 0414

        // Use this for initialization
        void Start()
        {
            avatar = GetComponent<DynamicCharacterAvatar>();
            avatar.CharacterBegun.AddListener(CharacterBegun);
            context = UMAContextBase.FindInstance();
        }

        void CharacterBegun(UMAData umaData)
        {
            //If mesh is not dirty then we haven't changed slots.
            if (!umaData.isMeshDirty)
                return;

            SlotData[] slots = umaData.umaRecipe.slotDataList;
            slotsToAdd.Clear();

            foreach(RendererElement element in RendererElements)
            {
                if (element.rendererAssets == null || element.rendererAssets.Count <= 0)
                    continue;

                wardrobeSlotAssets.Clear();

                //First, lets collect a list of the slotDataAssets that are present in the wardrobe recipes of the wardrobe slots we've specified
                foreach (string wardrobeSlot in element.wardrobeSlots)
                {
                    UMATextRecipe recipe = avatar.GetWardrobeItem(wardrobeSlot);
                    if (recipe != null)
                    {
                        recipe.Load(umaRecipe, context);

                        if (umaRecipe.slotDataList != null)
                        {
                            for (int i = 0; i < umaRecipe.slotDataList.Length; i++)
                            {
                                SlotData slotData = umaRecipe.slotDataList[i];
                                if(slotData != null && slotData.asset != null)
                                    wardrobeSlotAssets.Add(slotData.asset);
                            }
                        }
                    }
                }

                //Next, check each slot for if they are in the list of specified slots or exist in one of the wardrobe recipes of the wardrobe slot we specified.
                foreach (SlotData slot in slots)
                {
                    if (element.slotAssets.Contains(slot.asset) || wardrobeSlotAssets.Contains(slot.asset))
                    {
                        //We check for at least one rendererAsset at the top level for loop.
                        //Set our existing slot to the first renderer in our renderer list.
                        slot.rendererAsset = element.rendererAssets[0];

                        //If we have more renderers then make a copy of the SlotData and set that copy's rendererAsset to this item's renderer.
                        //Add the newly created slots to a running list to combine back with the entire slot list at the end.
                        for (int i = 1; i < element.rendererAssets.Count; i++)
                        {
                            SlotData addSlot = slot.Copy();
                            addSlot.rendererAsset = element.rendererAssets[i];
                            slotsToAdd.Add(addSlot);
                        }                        
                    }
                }
            }

            //If we have added Slots, then add the first slots to the list and set the recipe's slots to the new combined list.
            if(slotsToAdd.Count > 0)
            {
                slotsToAdd.AddRange(slots);
                umaData.umaRecipe.SetSlots(slotsToAdd.ToArray());
                slotsToAdd.Clear();
            }

            wardrobeSlotAssets.Clear();
        }
    }
}
