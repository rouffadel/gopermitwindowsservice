using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPermitService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;

            this.serviceInstaller1.Description = "Service For the Smartpermit";
            this.serviceInstaller1.DisplayName = "SmartPermitService";
            this.serviceInstaller1.ServiceName = "SmartPermitService";
        }
    }
}
