using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuayaquilBank.Infrastructure.Common.Settings
{
    public class InfrastructureSettings
    {
        public string DatabaseProvider { get; set; } = null!;
        public SecuritySettings Security { get; set; } = new();
        public CorsSettings Cors { get; set; } = new();
    }
}
