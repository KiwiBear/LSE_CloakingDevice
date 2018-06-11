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
using VRage.Utils;

namespace LSE.Guns
{
    // Modified by Whiplash141 to account for ModAPI changes - 6/11/18

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LargeMissileTurret), false)]
    public class MissileTurretScrambler : TurretScrambler { }

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_InteriorTurret), false)]
    public class InteriorTurretScrambler : TurretScrambler { }

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LargeGatlingTurret), false)]
    public class GatlingTurretScrambler : TurretScrambler { }

    public class TurretScrambler : MyGameLogicComponent
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
            NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            var turret = (IMyLargeTurretBase)Entity; 
            var target = turret.Target;
            if (target != null)
            {
                try
                {
                    var targetGrid = ((IMyCubeBlock)target).CubeGrid;

                    var cubeGrid = LSE.CloakingDevice.CloakingDevice.Cloaked.FirstOrDefault<IMyEntity>((x) => x == targetGrid);
                    if (cubeGrid != null)
                    {
                        turret.ResetTargetingToDefault();
                    }
                }
                catch (Exception e)
                {
                    MyLog.Default.WriteLine(e);
                }
            }
        }
    }
}
