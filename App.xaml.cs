using System;
using System.Windows;
using System.Windows.Threading;

namespace RestoManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Handle unhandled exceptions
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // Diagnostic: Uncomment to verify app is starting
            // MessageBox.Show("Application is starting...", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"An unhandled exception occurred:\n\n{e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}",
                "Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            e.Handled = true; // Prevent application from crashing
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show(
                    $"A critical error occurred:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Critical Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}

