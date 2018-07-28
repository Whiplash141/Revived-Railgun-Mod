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

namespace Whiplash.Railgun
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation | MyUpdateOrder.BeforeSimulation)]
    public class RailgunCore : MySessionComponentBase
    {
        public static bool IsServer;
        private static bool _init;
        private int _count;
        static List<ArmorPiercingProjectileSimulation> liveProjectiles = new List<ArmorPiercingProjectileSimulation>();

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
            //if (MyAPIGateway.Multiplayer.IsServer)
            SimulateProjectiles(MyAPIGateway.Multiplayer.IsServer);

            if (!MyAPIGateway.Utilities.IsDedicated)
                DrawProjectileTracers();

            //MyAPIGateway.Utilities.ShowNotification($"projectiles: {liveProjectiles.Count}", 16);
        }

        private static void SimulateProjectiles(bool isServer)
        {
            //projectile simulation
            for (int i = liveProjectiles.Count - 1; i >= 0; i--)
            {
                var projectile = liveProjectiles[i];
                projectile.Update(isServer);

                if (projectile.Killed)
                    liveProjectiles.RemoveAt(i);
            }
        }

        private static void DrawProjectileTracers()
        {
            foreach (var projectile in liveProjectiles)
            {
                projectile.DrawTracer();
            }
        }

        public static void AddProjectile(ArmorPiercingProjectileSimulation projectile)
        {
            liveProjectiles.Add(projectile);
        }

        private void Initialize()
        {
            IsServer = MyAPIGateway.Multiplayer.IsServer || MyAPIGateway.Session.OnlineMode == MyOnlineModeEnum.OFFLINE;
            Communication.Register();
        }

        protected override void UnloadData()
        {
            base.UnloadData();
            Communication.Unregister();
        }
    }
}
