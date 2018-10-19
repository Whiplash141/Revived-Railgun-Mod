using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using Rexxar;
using Rexxar.Communication;
using Whiplash.ArmorPiercingProjectiles;
using VRage.ModAPI;
using Sandbox.Game.Entities;
using VRageMath;

namespace Whiplash.Railgun
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation | MyUpdateOrder.BeforeSimulation)]
    public class RailgunCore : MySessionComponentBase
    {
        public static bool IsServer;
        private static bool _init;
        private int _count;
        static List<ArmorPiercingProjectile> liveProjectiles = new List<ArmorPiercingProjectile>();
        static Dictionary<long, RailgunProjectileData> railgunDataDict = new Dictionary<long, RailgunProjectileData>();
        static HashSet<MyPlanet> _planets = new HashSet<MyPlanet>();

        public static int CountRegisteredRailguns()
        {
            return railgunDataDict.Count;
        }

        public override void UpdateAfterSimulation()
        {
            if (!_init)
            {
                _init = true;
                Initialize();
            }

            if (++_count % 10 == 0)
                Settings.SyncSettings();
        }

        public override void UpdateBeforeSimulation()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
                SimulateProjectiles();

            //MyAPIGateway.Utilities.ShowNotification($"projectiles: {liveProjectiles.Count}", 16);
        }

        private static void SimulateProjectiles()
        {
            //projectile simulation
            for (int i = liveProjectiles.Count - 1; i >= 0; i--)
            {
                var projectile = liveProjectiles[i];
                projectile.Update();

                if (projectile.Killed)
                    liveProjectiles.RemoveAt(i);

                var tracerData = projectile.GetTracerData();

                foreach (var tracer in tracerData)
                {
                    RailgunMessage.SendToClients(tracer);
                }
            }
        }

        public static void ShootProjectileServer(RailgunFireData fireData)
        {
            RailgunProjectileData projectileData;
            bool registered = railgunDataDict.TryGetValue(fireData.ShooterID, out projectileData);
            if (!registered)
                return;

            var projectile = new ArmorPiercingProjectile(fireData, projectileData);
            AddProjectile(projectile);
        }

        public static void DrawProjectileClient(RailgunTracerData tracerData)
        {
            RailgunProjectileData projectileData;
            bool registered = railgunDataDict.TryGetValue(tracerData.ShooterID, out projectileData);
            if (!registered)
                return;

            var projectile = new ArmorPiercingProjectileClient(tracerData, projectileData);
            projectile.DrawTracer();
        }

        public static void AddProjectile(ArmorPiercingProjectile projectile)
        {
            liveProjectiles.Add(projectile);
        }

        public static void RegisterRailgun(long entityID, RailgunProjectileData data)
        {
            railgunDataDict[entityID] = data;
        }

        public static void UnregisterRailgun(long entityID)
        {
            if (railgunDataDict.ContainsKey(entityID))
                railgunDataDict.Remove(entityID);
        }

        private void Initialize()
        {
            IsServer = MyAPIGateway.Multiplayer.IsServer || MyAPIGateway.Session.OnlineMode == MyOnlineModeEnum.OFFLINE;
            Communication.Register();
        }

        private void AddPlanet(IMyEntity entity)
        {
            var planet = entity as MyPlanet;
            if (planet != null)
                _planets.Add(planet);
        }

        private void RemovePlanet(IMyEntity entity)
        {
            var planet = entity as MyPlanet;
            if (planet != null)
                _planets.Remove(planet);
        }

        public static Vector3D GetNaturalGravityAtPoint(Vector3D point)
        {
            var gravity = Vector3D.Zero;
            foreach (var planet in _planets)
            {
                IMyGravityProvider gravityProvider = planet.Components.Get<MyGravityProviderComponent>();
                if (gravityProvider != null)
                    gravity += gravityProvider.GetWorldGravity(point);
            }
            return gravity;
        }

        public override void LoadData()
        {
            MyAPIGateway.Entities.OnEntityAdd += AddPlanet;
            MyAPIGateway.Entities.OnEntityRemove += RemovePlanet;

            base.LoadData();
        }


        protected override void UnloadData()
        {
            base.UnloadData();
            Communication.Unregister();
            MyAPIGateway.Entities.OnEntityAdd -= AddPlanet;
            MyAPIGateway.Entities.OnEntityRemove -= RemovePlanet;
        }
    }
}
