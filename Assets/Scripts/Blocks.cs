using System.Collections.Generic;
using UnityEngine;

public static class Blocks
{
    public static BlockData CurrentBlock = null;
    public static int CurrentBlockHealth = 0;
    public static IReadOnlyList<BlockData> BlocksList;
    public static void NewBlock()
    {
        CurrentBlock = GetRandomBlock();
        CurrentBlockHealth = CurrentBlock.health;
        Block_Text.SetText(Localization.Localize("block." + CurrentBlock.id));

        Block_Animator.Instance.SetSprite(AssetLoader.Images["b_"+CurrentBlock.id], CurrentBlock.outline);
        Block_Health.target = 1;
    }
    public static void Recalculate()
    {
        var blocks = new List<BlockData>(1);

        foreach (var item in AssetLoader.BiomesSub[Config.Instance.BiomeSubCur].blocksOrBiomes)
        {
            blocks.Add(AssetLoader.Blocks[item]);
        }
        BlocksList = blocks;
    }
    private static BlockData GetRandomBlock() // Thanks ROUNDS game source code :)
    {
        float num = 0f;
        for (int i = 0; i < BlocksList.Count; i++)
        {
            num += BlocksList[i].rarity;
        }
        float num2 = Random.Range(0f, num);
        for (int j = 0; j < BlocksList.Count; j++)
        {
            num2 -= BlocksList[j].rarity;
            if (num2 <= 0f)
            {
                return BlocksList[j];
            }
        }
        return BlocksList[0];
    }
}
