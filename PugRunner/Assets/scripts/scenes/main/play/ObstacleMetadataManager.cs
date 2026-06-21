// Copyright (c) Whatgame Studios 2026
using UnityEngine;

namespace PugRunner {

    public class ObstacleMetadataManager {
        public const int MAX_LEVEL = 3;

        public ObstacleMetadata[] obstacleMetadata;
        private uint[] maxDropRate;

        private byte[] randomSeed;
        private uint obsNum;

        public ObstacleMetadataManager(Sprite[] obstacleSprites, uint gameDay, uint gameType, uint instanceOfGame) 
        {
            (obstacleMetadata, maxDropRate) = generateMetadata(obstacleSprites);
            randomSeed = SeedGen.GenerateSeed(gameDay, gameType, instanceOfGame);
            obsNum = 0;
        }


        private (ObstacleMetadata[], uint[]) generateMetadata(Sprite[] obstacleSprites) 
        {
            ObstacleMetadata[] allMetadata = new ObstacleMetadata[obstacleSprites.Length];
            int i = 0;
            foreach (Sprite obstacleSprite in obstacleSprites)
            {
                string name = obstacleSprite.texture.name;
                uint[] dropRate;
                ObstacleMetadata metadata;

                if (name == "ter01")
                {
                    dropRate = new uint[] {60, 10, 10, 10};
                    metadata = new ObstacleMetadata(name, dropRate);
                }
                else if (name == "ter02")
                {
                    dropRate = new uint[] {10, 10, 10, 10};
                    metadata = new ObstacleMetadata(name, dropRate);
                }
                else if (name == "ter03")
                {
                    dropRate = new uint[] {10, 10, 10, 10};
                    metadata = new ObstacleMetadata(name, dropRate);
                }
                else 
                {
                    AuditLog.Log($"ERROR: Unknown sprite: {name}");

                    dropRate = new uint[] {10, 10, 10, 10};
                    metadata = new ObstacleMetadata(name, dropRate);
                }

                allMetadata[i++] = metadata;
            }

            uint[] maxDropRate = new uint[MAX_LEVEL + 1];
            for (i = 0; i < maxDropRate.Length; i++)
            {
                maxDropRate[i] = 0;
            }
            for (i = 0; i < allMetadata.Length; i++)
            {
                maxDropRate[0] += allMetadata[i].GetDropRate(0);
                maxDropRate[1] += allMetadata[i].GetDropRate(1);
                maxDropRate[2] += allMetadata[i].GetDropRate(2);
                maxDropRate[3] += allMetadata[i].GetDropRate(3);
            }

            return (allMetadata, maxDropRate);
        }

        public int GetNextSpriteIndex(int level)
        {
            uint chosen = SeedGen.GetNextValue(randomSeed, obsNum++, maxDropRate[level]);
            AuditLog.Log($"GetNextSpriteIndex: Level: {level}, max drop rate: {maxDropRate[level]}, chosen: {chosen}");
            uint ofs = 0;
            for (int i = 0; i < obstacleMetadata.Length; i++)
            {
                uint oldOfs = ofs;
                ofs += obstacleMetadata[i].GetDropRate(level);
                //AuditLog.Log($"GetNextSpriteIndex: oldOfs: {oldOfs}, ofs: {ofs}");
                if (chosen > oldOfs && chosen < ofs)
                {
                    return i;
                }
            }
            //AuditLog.Log($"GetNextSpriteIndex: return default: {obstacleMetadata.Length - 1}");
            return obstacleMetadata.Length - 1;
        }
    }
}
