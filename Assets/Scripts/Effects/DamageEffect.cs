namespace Effects {
    public struct DamageEffect {
        public float damage { get; }
        public bool overTime { get; }
        public bool affectedByArmor { get; }
        public bool affectedByResistance { get; }
        public bool affectedByWeakness { get; }
        public int ticksRemaining { get; set; }
        public float tickTimer { get; set; }

        public DamageEffect(float damage,
                            bool overTime = false,
                            bool affectedByArmor = true,
                            bool affectedByResistance = true,
                            bool affectedByWeakness = true,
                            int ticksRemaining = 0
        ) {
            this.damage = damage;
            this.overTime = overTime;
            this.affectedByArmor = affectedByArmor;
            this.affectedByResistance = affectedByResistance;
            this.affectedByWeakness = affectedByWeakness;
            this.ticksRemaining = ticksRemaining;
            tickTimer = 0;
        }
    }
}