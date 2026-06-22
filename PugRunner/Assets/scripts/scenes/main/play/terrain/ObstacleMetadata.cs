// Copyright (c) Whatgame Studios 2026
namespace PugRunner {

    public class ObstacleMetadata {
        public string Name;

        private uint[] dropRate;

        public ObstacleMetadata(string name, uint[] rate)
        {
            Name = name;
            dropRate = rate;
        }

        public uint GetDropRate(int level) 
        {
            if (level > dropRate.Length - 1)
            {
                AuditLog.Log($"Drop rate not available: Name: {Name}, Level: {level}");
                return 0;
            }
            else 
            {
                return dropRate[level];
            }
        }
    }
}
