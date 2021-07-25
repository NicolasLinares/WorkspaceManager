
using Microsoft.Shell;
using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace INVOXWorkspaceManager.Views {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp {
        private static readonly string APP_CONTEXT = "AppConfig.xml";
        private static readonly String APP_UUID = "Z3RA1088-3F99-4044-9C86-B5A0AAC35A6F";

        private static IApplicationContext context;

        private static App instance = null;

        private App() {
            instance = this;
        }

        public static App GetInstance() {
            return instance;
        }

        public static void Main(string[] args) {
            if (SingleInstance<App>.InitializeAsFirstInstance(APP_UUID)) {
                try {

                    App app = new App();
                    app.InitializeComponent();
                    app.Run(new MainWindow());
                } finally {
                    SingleInstance<App>.Cleanup();
                }
            }
        }

        /// <summary>
        /// Ejecutado cuando se inicia la aplicación, así que este es el lugar para inicializar
        /// el contenedor Spring y delegar al control principal.
        /// </summary>
        public void AppStartup(object sender, StartupEventArgs args) {
            var Args = args.Args;
            Console.CancelKeyPress += Console_CancelKeyPress;
            InitLogics();

        }

        /// <summary>
        /// Ejecutado cuando se cierra la aplicación, así que aquí es donde se debe para el contenedor Spring.
        /// </summary>
        public void AppExit(object sender, ExitEventArgs args) {
            context.Dispose();
            ContextRegistry.Clear();
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
        }

        public void InitLogics() {
            try {
                string runningAppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string appContext = Path.Combine(runningAppDir, APP_CONTEXT);
                appContext = "encfile://" + appContext;
                context = new XmlApplicationContext(appContext);
                if (!ContextRegistry.IsContextRegistered(context.Name)) {
                    ContextRegistry.RegisterContext(context);
                }

            } catch (Exception e) {
                Console.WriteLine("Error al iniciar contexto Spring", e);
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args) {
            throw new NotImplementedException();
        }
    }
}
