using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ScannerAgent;

public partial class StatusForm : Form
{
    private ScannerManager scannerManager = new();

    public StatusForm()
    {
        InitializeComponent();
        CargarScanners();
    }

    private void mostrarVentanaToolStripMenuItem_Click(object sender, EventArgs e)
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.ShowInTaskbar = true;
    }

    private void salirToolStripMenuItem_Click(object sender, EventArgs e)
    {
        notifyIcon.Visible = false;
        Application.Exit();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (this.WindowState == FormWindowState.Minimized)
        {
            this.Hide();
            notifyIcon.Visible = true;
        }
    }

    // =====

    public void AddFileRow(string fileName)
    {
        listBoxFiles.Items.Add(fileName);
    }

    private void CargarScanners()
    {
        List<string> scanners = scannerManager.GetScanners();
        comboBoxScanners.Items.Clear();

        if (scanners.Count == 0)
        {
            comboBoxScanners.Items.Add("No hay escáneres disponibles");
        }
        else
        {
            comboBoxScanners.Items.AddRange(scanners.ToArray());
        }

        comboBoxScanners.SelectedIndex = 0;
    }

    private void btnScan_Click(object sender, EventArgs e)
    {
        string selectedScanner = comboBoxScanners.SelectedItem?.ToString() ?? "No hay escáneres disponibles";

        if (selectedScanner != "No hay escáneres disponibles")
        {
            string filePath = scannerManager.Scan(selectedScanner);

            if (!string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Documento guardado en:\n" + filePath, "Escaneo Exitoso");
            }
        }
        else
        {
            MessageBox.Show("No hay escáner disponible para escanear.");
        }
    }

    private void btnScanDocument_Click(object sender, EventArgs e)
    {
        List<Image> scannedImages = new List<Image>();
        string selectedScanner = comboBoxScanners.SelectedItem?.ToString() ?? "No hay escáneres disponibles";

        while (true)
        {
            try
            {
                string tempPath = scannerManager.Scan(selectedScanner);
                scannedImages.Add(Image.FromFile(tempPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fin del escaneo o error: " + ex.Message);
                break;
            }

            var continueScan = MessageBox.Show("¿Deseas escanear otra página?", "Escaneo", MessageBoxButtons.YesNo);
            if (continueScan == DialogResult.No)
                break;
        }

        if (scannedImages.Count == 0)
        {
            MessageBox.Show("No se escanearon imágenes.");
            return;
        }

        string userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string outputPath = Path.Combine(userDocumentsPath, "TempData");


        SaveImagesAsPdf(scannedImages, outputPath);
    }

    void SaveImagesAsPdf(List<Image> images, string outputPath)
    {
        var document = new PdfDocument();

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        foreach (var img in images)
        {
            using var stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            var page = document.AddPage();
            page.Width = img.Width * 72 / img.HorizontalResolution;
            page.Height = img.Height * 72 / img.VerticalResolution;

            using XGraphics gfx = XGraphics.FromPdfPage(page);
            using XImage xImage = XImage.FromStream(stream);
            gfx.DrawImage(xImage, 0, 0);
        }

        document.Save(Path.Combine(outputPath, $"{Guid.NewGuid()}.pdf"));
    }
}
