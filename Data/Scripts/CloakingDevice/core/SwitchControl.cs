/*
Copyright © 2016 Leto
This work is free. You can redistribute it and/or modify it under the
terms of the Do What The Fuck You Want To Public License, Version 2,
as published by Sam Hocevar. See http://www.wtfpl.net/ for more details.
*/

using System.Collections.Generic;
using System.Text;

using Sandbox.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.ModAPI.Interfaces.Terminal;

namespace LSE.Control
{
    public class SwitchControl<T> : BaseControl<T>
    {
        public bool DefaultValue;
        public string OnButton;
        public string OffButton;
        public string InternalNameOnAction;
        public string DisplayNameOnAction;

        public string InternalNameOffAction;
        public string DisplayNameOffAction;

        public string InternalNameToggleAction;
        public string DisplayNameToggleAction;

        public SwitchControl(
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
            : base(block, internalName, title)
        {
            OnButton = onButton;
            OffButton = offButton;
            InternalNameOnAction = internalNameOnAction;
            DisplayNameOnAction = displayNameOnAction;
            InternalNameOffAction = internalNameOffAction;
            DisplayNameOffAction = displayNameOffAction;
            InternalNameToggleAction = internalNameToggleAction;
            DisplayNameToggleAction = displayNameToggleAction;
            DefaultValue = defaultValue;

            MyAPIGateway.Utilities.GetVariable<bool>(block.EntityId.ToString() + InternalName, out defaultValue);
            MyAPIGateway.Utilities.SetVariable<bool>(block.EntityId.ToString() + InternalName, defaultValue);

            CreateUI();
        }

        public override void OnCreateUI()
        {
            var switchControl = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, T>(InternalName);
            switchControl.Visible = ShowControl;
            switchControl.Getter = Getter;
            switchControl.Setter = Setter;
            switchControl.Title = VRage.Utils.MyStringId.GetOrCompute(Title);
            switchControl.OnText = VRage.Utils.MyStringId.GetOrCompute(OnButton);
            switchControl.OffText = VRage.Utils.MyStringId.GetOrCompute(OffButton);
            MyAPIGateway.TerminalControls.AddControl<T>(switchControl);
            /*
            var onAction = MyAPIGateway.TerminalControls.CreateAction<T>(InternalNameOnAction);
            onAction.Action = OnAction;
            onAction.Name = new StringBuilder(DisplayNameOnAction);
            onAction.Writer = HotbarText;
            MyAPIGateway.TerminalControls.AddAction<T>(onAction);

            var offAction = MyAPIGateway.TerminalControls.CreateAction<T>(InternalNameOnAction);
            offAction.Action = OffAction;
            offAction.Name = new StringBuilder(DisplayNameOnAction);
            offAction.Writer = HotbarText;
            MyAPIGateway.TerminalControls.AddAction<T>(offAction);

            var toggleAction = MyAPIGateway.TerminalControls.CreateAction<T>(InternalNameOnAction);
            toggleAction.Action = ToggleAction;
            toggleAction.Name = new StringBuilder(DisplayNameOnAction);
            toggleAction.Writer = HotbarText;
            MyAPIGateway.TerminalControls.AddAction<T>(toggleAction);*/
        }

        public bool Getter(IMyTerminalBlock block)
        {
            bool value = true;
            MyAPIGateway.Utilities.GetVariable<bool>(block.EntityId.ToString() + InternalName, out value);
            return value;
        }

        public void Setter(IMyTerminalBlock block, bool newState)
        {
            MyAPIGateway.Utilities.SetVariable<bool>(block.EntityId.ToString() + InternalName, newState);
        }


        public void ToggleAction(IMyTerminalBlock block)
        {
            Setter(block, !Getter(block));
        }

        void OnAction(IMyTerminalBlock block)
        {
            Setter(block, true);
        }

        void OffAction(IMyTerminalBlock block)
        {
            Setter(block, true);
        }

        void HotbarText(IMyTerminalBlock block, StringBuilder hotbarText)
        {
            hotbarText.Clear();
            hotbarText.Append(Getter(block) ? OnButton : OffButton);
        }
    }
}