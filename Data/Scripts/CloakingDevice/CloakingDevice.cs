using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

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



namespace LSE.CloakingDevice
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_OreDetector), new string[] { "Cloaking_Device_Large", "Cloaking_Device_Small", "Cloaking_Device_Small_v2", "Cloaking_Device_v2" })]
    public class CloakingDevice : GameLogicComponent
    {
        public static HashSet<IMyEntity> Cloaked = new HashSet<IMyEntity>();

        public static float CLOAKING_RATIO = 9000.0f;
        public static float SHOOT_MULTIPLIER = 3.0f;

        public Control.SwitchControl<Sandbox.ModAPI.Ingame.IMyOreDetector> SwitchControl;

        public bool FirstStart = true;
        public long LastTick = 0;

        IMyCubeBlock CubeBlock;
        IMyCubeGrid LastCubeGrid;
        Sandbox.Game.EntityComponents.MyResourceSinkComponent Sink;
        MyDefinitionId PowerDefinitionId = new VRage.Game.MyDefinitionId(typeof(VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_GasProperties), "Electricity");

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            CubeBlock = Entity as IMyCubeBlock;
            if (!CubeBlock.BlockDefinition.SubtypeName.Contains("Cloaking_Device")) { return; }

            LastTick = DateTime.UtcNow.Ticks;
            LastCubeGrid = CubeBlock.CubeGrid;

            //Sink = new Sandbox.Game.EntityComponents.MyResourceSinkComponent();
            //CubeBlock.Components.Add<Sandbox.Game.EntityComponents.MyResourceSinkComponent>(Sink);
            CubeBlock.Components.TryGet<Sandbox.Game.EntityComponents.MyResourceSinkComponent>(out Sink);

            Sink.SetRequiredInputFuncByType(PowerDefinitionId, CalcRequiredPower);

            CubeBlock.IsWorkingChanged += WorkingChanged;
        }

        public void WorkingChanged(IMyCubeBlock block)
        {
            var cloak = block.GameLogic.GetAs<CloakingDevice>();
            if (!cloak.IsWorking())
            {
                LastCubeGrid.Render.Visible = true;
                block.CubeGrid.Render.Visible = true;
                Sink.Update();
            }
        }

        public bool IsWorking()
        {
            return CubeBlock.IsWorking && CubeBlock.IsFunctional;
        }
        
        public bool IsControlled() 
        {
			var cockpits= new List<IMySlimBlock>();
            CubeBlock.CubeGrid.GetBlocks(cockpits,
                (x) => (x.FatBlock != null && (
					x.FatBlock.BlockDefinition.TypeIdString.Contains("Cockpit") ||
					x.FatBlock.BlockDefinition.TypeIdString.Contains("RemoteControl")
                )));
			
			foreach (var cockpit in cockpits)
			{
				var controller = (Sandbox.ModAPI.Ingame.IMyShipController)cockpit.FatBlock;
				if (controller.IsUnderControl) {
					return true;
				}
			}
			return false;
        }

        public override void UpdateBeforeSimulation100()
        {
            base.UpdateBeforeSimulation100();
            if (!CubeBlock.BlockDefinition.SubtypeName.Contains("Cloaking_Device")) { return; }

            if (FirstStart)
            {
                CreateUI();
                FirstStart = false;
            }


            if (IsWorking() && ((IMyFunctionalBlock)Entity).Enabled)
            {
                var visible = false;
                if (!SwitchControl.Getter((IMyFunctionalBlock)Entity) && !IsControlled())
                {
                    ((IMyFunctionalBlock)Entity).RequestEnable(false);
                    LastCubeGrid.Render.Visible = true;
                    CubeBlock.CubeGrid.Render.Visible = true;
                }
                else
                {
                    LastCubeGrid.Render.Visible = true;
                    CubeBlock.CubeGrid.Render.Visible = false;
                }
    
                Cloaked.Remove(LastCubeGrid);
                if (visible)
                {
                    Cloaked.Remove(CubeBlock.CubeGrid);
                }
                else
                {
                    Cloaked.Add(CubeBlock.CubeGrid);
                }

                LastCubeGrid = CubeBlock.CubeGrid;
                Sink.Update();
            }
        }

        // a large reactor generates about 300 MW and weigths 70 kg
        // an heavy blocks weigths about 3,3 kg, an light block only 0.500kg
        // an normal ship weights about 1 tons plus one ton cargo. 2 tons with one large reactor.
        // 2000 kg -> 2000000 gramm / 300 MW = 6666,67
        // 1000 kg -> 1000000 gramm / 300 MW = 3333.33


        public float CalcRequiredPower()
        {
            float neededPower = 0.0001f;
            if (CubeBlock != null &&
                CubeBlock.CubeGrid != null &&
                CubeBlock.CubeGrid.Physics != null &&
                (CubeBlock as IMyFunctionalBlock).Enabled)
            {
                IsShooting();
                var shootMultiplier = IsShooting() ? SHOOT_MULTIPLIER : 1;
                neededPower = CubeBlock.CubeGrid.Physics.Mass * shootMultiplier / CLOAKING_RATIO;
            }
            LastTick = DateTime.UtcNow.Ticks;
            //if (Sink.IsPowerAvailable(PowerDefinitionId, neededPower))
            //{
            //}
            return neededPower;
        }
            

        public override void MarkForClose()
        {
            if (CubeBlock != null && CubeBlock.CubeGrid != null && CubeBlock.CubeGrid.Render != null)
            {
                CubeBlock.CubeGrid.Render.Visible = true;
            }
        }

        public bool IsShooting()
        {
            /// doesn't work with autogun fire
            
            if (CubeBlock != null &&
                CubeBlock.CubeGrid != null)
            {
                var turrets = new List<IMySlimBlock>();
                CubeBlock.CubeGrid.GetBlocks(turrets,
                    (x) => (x.FatBlock != null && (
                    x.FatBlock.BlockDefinition.TypeIdString.Contains("Turret") ||
                    x.FatBlock.BlockDefinition.TypeIdString.Contains("MissileLauncher") ||
                    x.FatBlock.BlockDefinition.TypeIdString.Contains("GatlingGun")
                    )));
                foreach (var turret in turrets)
                {
                    var ob = ((MyObjectBuilder_UserControllableGun)turret.FatBlock.GetObjectBuilderCubeBlock());
                    MyObjectBuilder_GunBase gunBase;
                    if (turret.FatBlock.BlockDefinition.TypeIdString.Contains("GatlingGun"))
                    {
                        gunBase = ((MyObjectBuilder_SmallGatlingGun)ob).GunBase;
                    }
                    else if (turret.FatBlock.BlockDefinition.TypeIdString.Contains("MissileLauncher"))
                    {
                        gunBase = ((MyObjectBuilder_SmallMissileLauncher)ob).GunBase;
                    }
                    else
                    {
                        gunBase = ((MyObjectBuilder_TurretBase)ob).GunBase;
                    }
                    
                    if (gunBase.LastShootTime > LastTick)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        bool ShowControlOreDetectorControls(IMyTerminalBlock block)
        {
            return block.BlockDefinition.SubtypeName.Contains("OreDetector");
        }



        void RemoveOreUI()
        {
            List<IMyTerminalAction> actions = new List<IMyTerminalAction>();
            MyAPIGateway.TerminalControls.GetActions<Sandbox.ModAPI.Ingame.IMyOreDetector>(out actions);
            var actionAntenna = actions.First((x) => x.Id.ToString() == "BroadcastUsingAntennas");
            actionAntenna.Enabled = ShowControlOreDetectorControls;

            List<IMyTerminalControl> controls = new List<IMyTerminalControl>();
            MyAPIGateway.TerminalControls.GetControls<Sandbox.ModAPI.Ingame.IMyOreDetector>(out controls);
            var antennaControl = controls.First((x) => x.Id.ToString() == "BroadcastUsingAntennas");
            antennaControl.Visible = ShowControlOreDetectorControls;

            try
            {
                var radiusControl = controls.First((x) => x.Id.ToString() == "Range");
                radiusControl.Visible = ShowControlOreDetectorControls;
            }
            catch
            {
            }

        }


        void CreateUI()
        {
            RemoveOreUI();
            SwitchControl = new ServerSwitch<Sandbox.ModAPI.Ingame.IMyOreDetector>(
                (IMyFunctionalBlock)Entity,
                "StayCloakOnOff",
                "Stay cloaked when not controlled:",
                "On",
                "Off",
                "StayCloakOnAction",
                "Stay cloaked when not controlled.",
                "StayCloakOnAction",
                "Cloak even when not controlled.",
                "StayCloakOnAction",
                "Toggle if it should stay cloaked.",
                false);

        }


        public IMyTerminalControl GetControl(string internalName)
        {
            return Controls.GetValueOrDefault(internalName);
        }
        Dictionary<string, IMyTerminalControl> Controls;

    }
}

class ServerSwitch<T> : LSE.Control.SwitchControl<T>
{
    public ServerSwitch(
         IMyTerminalBlock block,
         string internalName,
         string title,
         string onButton,
         string offButton,
         string internalNameOnAction,
         string displayNameOnAction,
         string internalNameOffAction,
         string displayNameOffAction,
         string internalNameToggleAction,
         string displayNameToggleAction,
         bool defaultValue = true)
        : base(block, internalName,
       title,
       onButton,
       offButton,
       internalNameOnAction,
       displayNameOnAction,
       internalNameOffAction,
       displayNameOffAction,
       internalNameToggleAction,
       displayNameToggleAction,
       defaultValue)
    {
    }


    public void Setter(IMyTerminalBlock block, bool newState)
    {
        base.Setter(block, newState);
        var message = new LSE.Network2.MessageSave();
        message.EntityId = block.EntityId;
        message.State = newState;
        LSE.Network2.MessageUtils.SendMessageToAll(message);
    }


}
