using System.ServiceProcess;
using System.Windows.Forms;

namespace Timer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            //return;
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CheckStatusEmail()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}