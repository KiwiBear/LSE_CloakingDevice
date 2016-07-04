using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Collections;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.ModAPI;
using VRage.Game.ModAPI.Interfaces;
using VRage;
using VRage.ObjectBuilders;
using VRage.Game;
using VRage.ModAPI;
using VRage.Game.Components;
using VRageMath;
using Sandbox.Engine.Multiplayer;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game.ModAPI;




namespace LSE.Guns.Missile
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LargeMissileTurret))]
    public class MissileGunScrambler : MyGameLogicComponent
    {
        private MyObjectBuilder_EntityBase m_objectBuilder;
        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return copy ? (MyObjectBuilder_EntityBase)m_objectBuilder.Clone() : m_objectBuilder;
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            m_objectBuilder = objectBuilder;
        }


        public override void UpdateAfterSimulation()
        {
            var ob = (MyObjectBuilder_UserControllableGun)((IMyCubeBlock)Entity).GetObjectBuilderCubeBlock();
            var turretBuilder = ((MyObjectBuilder_TurretBase)ob);
            if (turretBuilder.Target != 0)
            {
                IMyEntity target;
                MyAPIGateway.Entities.TryGetEntityById(turretBuilder.Target, out target);
                if (target != null)
                {
                    try
                    {
                        var targetGrid = ((IMyCubeBlock)target).CubeGrid;

                        var cubeGrid = LSE.CloakingDevice.CloakingDevice.Cloaked.FirstOrDefault<IMyEntity>((x) => x == targetGrid);
                        if (cubeGrid != null)
                        {
                            ((Sandbox.ModAPI.IMyLargeTurretBase)Entity).ResetTargetingToDefault();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}


namespace LSE.Guns.Interior
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_InteriorTurret))]
    public class MissileGunScrambler : MyGameLogicComponent
    {
        private MyObjectBuilder_EntityBase m_objectBuilder;
        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return copy ? (MyObjectBuilder_EntityBase)m_objectBuilder.Clone() : m_objectBuilder;
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            m_objectBuilder = objectBuilder;
        }



        public override void UpdateAfterSimulation()
        {
            var ob = (MyObjectBuilder_UserControllableGun)((IMyCubeBlock)Entity).GetObjectBuilderCubeBlock();
            var turretBuilder = ((MyObjectBuilder_TurretBase)ob);
            if (turretBuilder.Target != 0)
            {
                IMyEntity target;
                MyAPIGateway.Entities.TryGetEntityById(turretBuilder.Target, out target);
                if (target != null)
                {
                    try
                    {
                        var targetGrid = ((IMyCubeBlock)target).CubeGrid;

                        var cubeGrid = LSE.CloakingDevice.CloakingDevice.Cloaked.FirstOrDefault<IMyEntity>((x) => x == targetGrid);
                        if (cubeGrid != null)
                        {
                            ((Sandbox.ModAPI.IMyLargeTurretBase)Entity).ResetTargetingToDefault();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}


namespace LSE.Guns.Gatling
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LargeGatlingTurret))]
    public class MissileGunScrambler : MyGameLogicComponent
    {
        private MyObjectBuilder_EntityBase m_objectBuilder;
        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return copy ? (MyObjectBuilder_EntityBase)m_objectBuilder.Clone() : m_objectBuilder;
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            m_objectBuilder = objectBuilder;
        }



        public override void UpdateAfterSimulation()
        {
            var ob = (MyObjectBuilder_UserControllableGun)((IMyCubeBlock)Entity).GetObjectBuilderCubeBlock();
            var turretBuilder = ((MyObjectBuilder_TurretBase)ob);
            if (turretBuilder.Target != 0)
            {
                IMyEntity target;
                MyAPIGateway.Entities.TryGetEntityById(turretBuilder.Target, out target);
                if (target != null)
                {
                    try
                    {
                        var targetGrid = ((IMyCubeBlock)target).CubeGrid;

                        var cubeGrid = LSE.CloakingDevice.CloakingDevice.Cloaked.FirstOrDefault<IMyEntity>((x) => x == targetGrid);
                        if (cubeGrid != null)
                        {
                            ((Sandbox.ModAPI.IMyLargeTurretBase)Entity).ResetTargetingToDefault();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}


