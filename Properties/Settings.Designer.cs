﻿
namespace ConnectFour.Properties
{
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsOpponentComputer
        {
            get
            {
                return ((bool)(this["IsOpponentComputer"]));
            }
            set
            {
                this["IsOpponentComputer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsOpponentChipYellow
        {
            get
            {
                return ((bool)(this["IsOpponentChipYellow"]));
            }
            set
            {
                this["IsOpponentChipYellow"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsSoundMuted
        {
            get
            {
                return ((bool)(this["IsSoundMuted"]));
            }
            set
            {
                this["IsSoundMuted"] = value;
            }
        }
    }
}