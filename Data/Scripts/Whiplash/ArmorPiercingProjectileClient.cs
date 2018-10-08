using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using Sandbox.Game.Weapons;
using VRage.Game.ModAPI;
using VRageMath;
using Sandbox.Game;
using VRage.Game.Entity;
using Sandbox.Game.Entities;
using VRage.Game.ModAPI.Interfaces;
using Sandbox.Definitions;
using Whiplash.Railgun;
using VRage.Collections;
using VRage.Voxels;

namespace Whiplash.ArmorPiercingProjectiles
{
    public class ArmorPiercingProjectileClient
    {
        readonly bool _drawTracer;
        Vector4 _lineColor;
        readonly MyStringId _material = MyStringId.GetOrCompute("WeaponLaser");
        readonly MyStringId _bulletMaterial = MyStringId.GetOrCompute("ProjectileTrailLine");
        readonly Vector3 _tracerColor;
        readonly float _tracerScale;
        readonly Vector3D _from;
        readonly Vector3D _to;
        readonly Vector3D _projectileVelocity;
        readonly bool _shouldDraw;

        public ArmorPiercingProjectileClient(RailgunTracerData tracerData, RailgunProjectileData projectileData)
        {
            //weapon data
            _tracerColor = projectileData.ProjectileTrailColor;
            _tracerScale = projectileData.ProjectileTrailScale;
            _drawTracer = projectileData.DrawTracer;

            //tracer data
            _lineColor = tracerData.LineColor;
            _from = tracerData.LineFrom;
            _to = tracerData.LineTo;
            _projectileVelocity = tracerData.ProjectileDirection;
            _shouldDraw = tracerData.ShouldDraw;
        }

        public void DrawTracer()
        {
            if (MyAPIGateway.Utilities.IsDedicated)
                return;

            // Draw tracer
            if (_shouldDraw)
            {
                float scaleFactor = MyParticlesManager.Paused ? 1f : MyUtils.GetRandomFloat(1f, 2f);
                float lengthMultiplier = 40f * _tracerScale;
                lengthMultiplier *= MyParticlesManager.Paused ? 0.6f : MyUtils.GetRandomFloat(0.6f, 0.8f);
                var thisDirection = Vector3D.Normalize(_projectileVelocity);
                var startPoint = _to - thisDirection * lengthMultiplier;
                float thickness = (MyParticlesManager.Paused ? 0.2f : MyUtils.GetRandomFloat(0.2f, 0.3f)) * _tracerScale;
                thickness *= MathHelper.Lerp(0.2f, 0.8f, 1f);

                MyTransparentGeometry.AddLineBillboard(_bulletMaterial, new Vector4(_tracerColor * scaleFactor * 10f, 1f), startPoint, thisDirection, lengthMultiplier, thickness);
            }

            // Draw tracer line
            MySimpleObjectDraw.DrawLine(_from, _to, _material, ref _lineColor, _tracerScale * 0.1f);
        }
    }
}
