using AuditaActas.Dominio;

namespace AuditaActas.UI;

/// <summary>Diálogo de alta manual de un acta (RF2). Valida antes de devolver el objeto.</summary>
public class FormActa : Form
{
    private readonly TextBox _departamento = new() { Text = "D01", Width = 160 };
    private readonly TextBox _municipio = new() { Text = "M001", Width = 160 };
    private readonly TextBox _recinto = new() { Text = "R0001", Width = 160 };
    private readonly NumericUpDown _mesa = new() { Minimum = 1, Maximum = 99999, Value = 1, Width = 160 };
    private readonly NumericUpDown _habilitados = new() { Minimum = 1, Maximum = 100000, Value = 200, Width = 160 };
    private readonly NumericUpDown _emitidos = new() { Minimum = 0, Maximum = 100000, Value = 160, Width = 160 };
    private readonly NumericUpDown _blancos = new() { Minimum = 0, Maximum = 100000, Value = 4, Width = 160 };
    private readonly NumericUpDown _nulos = new() { Minimum = 0, Maximum = 100000, Value = 6, Width = 160 };
    private readonly NumericUpDown _votoA = new() { Minimum = 0, Maximum = 100000, Value = 60, Width = 160 };
    private readonly NumericUpDown _votoB = new() { Minimum = 0, Maximum = 100000, Value = 45, Width = 160 };
    private readonly NumericUpDown _votoC = new() { Minimum = 0, Maximum = 100000, Value = 30, Width = 160 };
    private readonly NumericUpDown _votoD = new() { Minimum = 0, Maximum = 100000, Value = 15, Width = 160 };

    public Acta? Resultado { get; private set; }

    public FormActa()
    {
        Text = "Registrar acta";
        Width = 460;
        Height = 520;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(12),
            AutoScroll = true
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));

        AgregarFila(tabla, "Departamento", _departamento);
        AgregarFila(tabla, "Municipio", _municipio);
        AgregarFila(tabla, "Recinto", _recinto);
        AgregarFila(tabla, "Mesa", _mesa);
        AgregarFila(tabla, "Habilitados", _habilitados);
        AgregarFila(tabla, "Emitidos", _emitidos);
        AgregarFila(tabla, "Blancos", _blancos);
        AgregarFila(tabla, "Nulos", _nulos);
        AgregarFila(tabla, "Candidato A", _votoA);
        AgregarFila(tabla, "Candidato B", _votoB);
        AgregarFila(tabla, "Candidato C", _votoC);
        AgregarFila(tabla, "Candidato D", _votoD);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, FlowDirection = FlowDirection.RightToLeft };
        var aceptar = new Button { Text = "Registrar", Width = 100, DialogResult = DialogResult.None };
        var cancelar = new Button { Text = "Cancelar", Width = 100, DialogResult = DialogResult.Cancel };
        aceptar.Click += Aceptar;
        botones.Controls.Add(aceptar);
        botones.Controls.Add(cancelar);

        Controls.Add(tabla);
        Controls.Add(botones);
        AcceptButton = aceptar;
        CancelButton = cancelar;
    }

    private static void AgregarFila(TableLayoutPanel tabla, string etiqueta, Control control)
    {
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        tabla.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 0, 0) });
        tabla.Controls.Add(control);
    }

    private void Aceptar(object? sender, EventArgs e)
    {
        var acta = new Acta
        {
            Departamento = _departamento.Text,
            Municipio = _municipio.Text,
            Recinto = _recinto.Text,
            Mesa = (int)_mesa.Value,
            Habilitados = (int)_habilitados.Value,
            Emitidos = (int)_emitidos.Value,
            Blancos = (int)_blancos.Value,
            Nulos = (int)_nulos.Value,
            Votos = new Dictionary<string, int>
            {
                ["Candidato A"] = (int)_votoA.Value,
                ["Candidato B"] = (int)_votoB.Value,
                ["Candidato C"] = (int)_votoC.Value,
                ["Candidato D"] = (int)_votoD.Value
            }
        };

        var errores = acta.Validar();
        if (errores.Count > 0)
        {
            MessageBox.Show(string.Join(Environment.NewLine, errores), "Acta inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Resultado = acta;
        DialogResult = DialogResult.OK;
        Close();
    }
}
