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

    private void button1_Click(object sender, EventArgs e)
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
}
