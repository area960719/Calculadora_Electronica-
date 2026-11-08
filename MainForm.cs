using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace CalculadoraElectronica
{
    public class MainForm : Form
    {
        private readonly WebView2 web = new WebView2();

        public MainForm()
        {
            Text = "Calculadora de electrónica";
            Size = new Size(540, 820);
            MinimumSize = new Size(380, 500);
            StartPosition = FormStartPosition.CenterScreen;

            web.Dock = DockStyle.Fill;
            Controls.Add(web);

            Load += async (s, e) => await IniciarAsync();
        }

        private async Task IniciarAsync()
        {
            try
            {
                // Carpeta de datos del navegador interno (evita problemas de permisos)
                string datos = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "CalculadoraElectronica");

                CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, datos);
                await web.EnsureCoreWebView2Async(env);

                web.CoreWebView2.Settings.IsStatusBarEnabled = false;
                web.CoreWebView2.Settings.AreDevToolsEnabled = false;
                web.NavigateToString(LeerHtml());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo iniciar la calculadora.\n\n" +
                    "Si falta WebView2 Runtime, instálalo desde:\n" +
                    "https://developer.microsoft.com/microsoft-edge/webview2/\n\n" +
                    "Detalle: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private static string LeerHtml()
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("calculadora.html"))
            {
                if (s == null)
                    throw new FileNotFoundException("El HTML no está incluido como recurso incrustado.");
                using (var r = new StreamReader(s, Encoding.UTF8))
                    return r.ReadToEnd();
            }
        }
    }
}
