namespace Whiplash.Railgun
{
    public struct RailgunConfig
    {
        public static RailgunConfig Default = new RailgunConfig()
        {
            VersionNumber = 2,
            ArtificialGravityMultiplier = 2,
            NaturalGravityMultiplier = 1,
            DrawProjectileTrails = true,
            PenetrationDamage = 33000,
            ExplosionRadius = 0f,
            ExplosionDamage = 0f,
            ShouldExplode = true,
            ShouldPenetrate = true,
            PenetrationRange = 100f,
            IdlePowerDrawFixed = 2f,
            IdlePowerDrawTurret = 20f,
            ReloadPowerDraw = 200f
        };

        public int VersionNumber;
        public float ArtificialGravityMultiplier;
        public float NaturalGravityMultiplier;
        public bool DrawProjectileTrails;
        public bool ShouldExplode;
        public float ExplosionRadius;
        public float ExplosionDamage;
        public bool ShouldPenetrate;
        public float PenetrationDamage;
        public float PenetrationRange;
        public float IdlePowerDrawFixed;
        public float IdlePowerDrawTurret;
        public float ReloadPowerDraw;
    }
}
