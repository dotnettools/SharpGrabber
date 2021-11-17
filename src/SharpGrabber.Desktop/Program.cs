using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Builder;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Threading.Tasks;

namespace SharpGrabber.Desktop
{
    class Program
    {
        public static MainWindow MainWindow { get; private set; }

        public static IBlackWidowService BlackWidow { get; private set; }

        public static async Task<int> Main(string[] args)
        {
            var app = BuildAvaloniaApp(args);
            await AppMain(app.Instance);
            return 0;
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp(string[] args)
        {
            var lifetime = new ClassicDesktopStyleApplicationLifetime
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose,
                Args = args,
            };
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .SetupWithLifetime(lifetime)
                .LogToTrace();
        }

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static async Task AppMain(Application app)
        {
            BlackWidow = await BlackWidowBuilder.New()
                .ConfigureInterpreterService(icfg => icfg.AddJint())
                .ConfigureLocalRepository(cfg => cfg.UsePhysical(@"blackwidow/repo"))
                .ConfigureRemoteRepository(cfg => cfg.UseOfficial())
                .BuildAsync();

            app.Run(MainWindow = new MainWindow());
        }
    }
}
