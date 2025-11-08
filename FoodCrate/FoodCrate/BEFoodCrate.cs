using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace FoodCrate;

public class BEFoodCrate : BlockEntityCrate
{
  protected override void InitInventory(Block block, ICoreAPI api)
  {
    base.InitInventory(block, api);
    Inventory.SlotModified += FoodCrate_SlotModified;
  }

  public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
  {
    base.GetBlockInfo(forPlayer, dsc);
    ItemSlot contents = Inventory.FirstNonEmptySlot;
    if (contents != null && contents.Itemstack.Collectible.TransitionableProps != null 
      && contents.Itemstack.Collectible.TransitionableProps.Length > 0)
    {
      // This was the best way I could figure out to get the spoilage info into a reasonable place
      // in the tooltip and maintain the chain of calling the parent method. It seems to work
      // pretty well, actually.
      dsc.Replace(contents.GetStackName(), BlockEntityShelf.PerishableInfoCompact(Api, contents, 0));
    }
  }
  
  void FoodCrate_SlotModified(int obj)
  {
    // Average the freshness of each stack over all the other stacks as if the contents 
    // were one enormous stack. This is what intuition says that crates should be doing 
    // anyway.
    
    bool isSpoiling = false;
    float totalTransitionedHours = 0;
    float maxFreshHours = 0;
    int totalItems = 0;
    int slotCount = 0;

    foreach (ItemSlot crateSlot in Inventory)
    {
      isSpoiling = false;
      if (crateSlot.Empty)
      {
        continue;
      }

      TransitionState perishState = crateSlot.Itemstack.Collectible.UpdateAndGetTransitionState(Api.World,
        crateSlot, EnumTransitionType.Perish);
      if (perishState == null)
      {
        // If there's no perish state, then there's nothing to do for the whole crate
        return;
      }

      TransitionableProperties props = perishState.Props;
      if (props.Type != EnumTransitionType.Perish)
      {
        // Shouldn't hit this, but I'm paranoid.
        continue;
      }

      if (perishState.TransitionLevel > 0.0)
      {
        // TransitionLevel > 0 means this stack has started to spoil
        isSpoiling = true;
        maxFreshHours = perishState.FreshHours;
      }

      // Weight values from a partial stack. 
      totalTransitionedHours += perishState.TransitionedHours * crateSlot.Itemstack.StackSize;
      totalItems += crateSlot.Itemstack.StackSize;

      ++slotCount;
    }

    float avgTransitionedHours = totalTransitionedHours / totalItems;

    if (isSpoiling)
    {
      // If anything is spoiling, everything is spoiling
      avgTransitionedHours = Math.Max(maxFreshHours, avgTransitionedHours);
    }

    foreach (ItemSlot crateSlot in Inventory)
    {
      if (crateSlot.Empty)
      {
        continue;
      }
      crateSlot.Itemstack.Collectible.SetTransitionState(crateSlot.Itemstack, EnumTransitionType.Perish,
        avgTransitionedHours);
    }
    MarkDirty();
  }
  
}