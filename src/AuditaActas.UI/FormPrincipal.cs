using AuditaActas.Aplicacion;
using AuditaActas.Dominio;
using AuditaActas.Infraestructura;

namespace AuditaActas.UI;

/// <summary>
/// Ventana inicial (esqueleto del MVP). Regla de arquitectura: la UI NO calcula nada,
/// solo llama a los servicios de AuditaActas.Aplicacion.
/// </summary>
public class FormPrincipal : Form
{
    private readonly NumericUpDown _mesas = new() { Minimum = 100, Maximum = 20000, Value = 2000, Width = 70 };
    private readonly NumericUpDown _fraude = new() { Minimum = 0, Maximum = 50, Value = 5, Width = 50 };
    private readonly NumericUpDown _semilla = new() { Minimum = 0, Maximum = 99999, Value = 42, Width = 65 };
    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
    };
    private readonly Label _estado = new()
    {
        Dock = DockStyle.Bottom,
        Height = 32,
        TextAlign = ContentAlignment.MiddleLeft,
        Text = "Los resultados son indicios estadísticos, no prueba de fraude."
    };

    private List<Acta> _actas = new();
    private IReadOnlyList<ResultadoAuditoria> _resultados = Array.Empty<ResultadoAuditoria>();

    public FormPrincipal()
    {
        Text = "AuditaActas (prototipo)";
        Width = 1000;
        Height = 650;

        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, WrapContents = false };
        barra.Controls.Add(Etiqueta("Mesas:"));
        barra.Controls.Add(_mesas);
        barra.Controls.Add(Etiqueta("% fraude:"));
        barra.Controls.Add(_fraude);
        barra.Controls.Add(Etiqueta("Semilla:"));
        barra.Controls.Add(_semilla);
        barra.Controls.Add(Boton("Generar datos", Generar));
        barra.Controls.Add(Boton("Verificar integridad", Verificar));
        barra.Controls.Add(Boton("Alterar 1 acta (demo)", Alterar));
        barra.Controls.Add(Boton("Resultados", MostrarResultados));
        barra.Controls.Add(Boton("Auditar", Auditar));
        barra.Controls.Add(Boton("Exportar sospechosas", Exportar));

        Controls.Add(_grid);
        Controls.Add(_estado);
        Controls.Add(barra);
    }

    private static Label Etiqueta(string texto) => new() { Text = texto, AutoSize = true, Margin = new Padding(8, 10, 0, 0) };

    private static Button Boton(string texto, EventHandler accion)
    {
        var b = new Button { Text = texto, AutoSize = true, Margin = new Padding(6, 6, 0, 0) };
        b.Click += accion;
        return b;
    }

    private bool HayDatos()
    {
        if (_actas.Count > 0) return true;
        MessageBox.Show("Primero genere datos.", "AuditaActas");
        return false;
    }

    private void Generar(object? s, EventArgs e)
    {
        var parametros = new ParametrosSimulacion(
            (int)_mesas.Value, 0.80, (double)_fraude.Value / 100.0, (int)_semilla.Value);
        _actas = new GeneradorService().Generar(parametros);
        CadenaHashService.Encadenar(_actas);
        _resultados = Array.Empty<ResultadoAuditoria>();

        _grid.DataSource = _actas
            .Select(a => new { a.Departamento, a.Municipio, a.Recinto, a.Mesa, a.Habilitados, a.Emitidos, a.Blancos, a.Nulos })
            .ToList();
        _estado.Text = $"{_actas.Count} actas generadas y encadenadas.";
    }

    private void Verificar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        var indice = CadenaHashService.PrimeraActaAlterada(_actas);
        _estado.Text = indice is null
            ? "Integridad OK: la cadena de hashes es válida."
            : $"¡Cadena rota! Primera acta alterada: {_actas[indice.Value].Clave}";
    }

    private void Alterar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        var acta = _actas[_actas.Count / 2];
        var candidato = acta.Votos.Keys.First();
        acta.Votos[candidato]++;
        _estado.Text = $"Se alteró {acta.Clave} (+1 voto a {candidato}). Pulse 'Verificar integridad'.";
    }

    private void MostrarResultados(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        _grid.DataSource = ComputoService.Computar(_actas, NivelGeografico.Departamento)
            .Select(r => new
            {
                Departamento = r.Clave,
                r.Mesas,
                r.Habilitados,
                r.Emitidos,
                Votos = string.Join(" | ", r.Votos.Select(kv => $"{kv.Key}: {kv.Value}"))
            })
            .ToList();
        _estado.Text = "Resultados por departamento.";
    }

    private void Auditar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        _resultados = new AuditoriaService().Auditar(_actas);
        var sospechosas = _resultados.Where(r => r.EsSospechosa).OrderByDescending(r => r.Puntaje).ToList();

        _grid.DataSource = sospechosas
            .Select(r => new { Acta = r.Acta.Clave, Puntaje = Math.Round(r.Puntaje, 2), r.Motivo })
            .ToList();

        var m = EvaluacionService.Evaluar(_resultados);
        _estado.Text = $"{sospechosas.Count} actas sospechosas | Precisión {m.Precision:P0} | Recall {m.Recall:P0} | Falsos positivos {m.TasaFalsosPositivos:P1}";
    }

    private void Exportar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        if (_resultados.Count == 0) Auditar(null, EventArgs.Empty);

        using var dialogo = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "actas_sospechosas.csv" };
        if (dialogo.ShowDialog() != DialogResult.OK) return;

        ReporteCsv.Exportar(dialogo.FileName, _resultados.Where(r => r.EsSospechosa).OrderByDescending(r => r.Puntaje));
        _estado.Text = $"Reporte exportado a {dialogo.FileName}";
    }
}
