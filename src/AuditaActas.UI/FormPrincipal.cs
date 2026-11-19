using AuditaActas.Aplicacion;
using AuditaActas.Dominio;
using AuditaActas.Infraestructura;

namespace AuditaActas.UI;

public sealed record FilaMetrica(string Metrica, string Valor);

/// <summary>
/// Ventana principal del MVP. Regla de arquitectura: la UI NO calcula nada,
/// solo llama a los servicios de AuditaActas.Aplicacion y a la persistencia.
/// </summary>
public class FormPrincipal : Form
{
    private readonly NumericUpDown _mesas = new() { Minimum = 100, Maximum = 20000, Value = 2000, Width = 70 };
    private readonly NumericUpDown _fraude = new() { Minimum = 0, Maximum = 50, Value = 5, Width = 50 };
    private readonly NumericUpDown _semilla = new() { Minimum = 0, Maximum = 99999, Value = 42, Width = 65 };
    private readonly ComboBox _nivel = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110 };
    private readonly ComboBox _rol = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110 };
    private readonly TextBox _usuario = new() { Width = 110, Text = Environment.UserName };

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
        Height = 34,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(6, 0, 0, 0),
        Text = "Listo. Los resultados son indicios estadísticos, no prueba de fraude."
    };

    private readonly Button _btnAuditar = new();
    private readonly Button _btnEvaluacion = new();
    private readonly Button _btnExportar = new();
    private readonly Button _btnAlterar = new();
    private readonly Button _btnBitacora = new();

    private readonly List<Acta> _actas = new();
    private readonly BitacoraService _bitacora = new();
    private IReadOnlyList<ResultadoAuditoria> _resultados = Array.Empty<ResultadoAuditoria>();
    private string? _rutaBd;

    public FormPrincipal()
    {
        Text = "AuditaActas (prototipo académico)";
        Width = 1100;
        Height = 700;
        MinimumSize = new Size(900, 550);
        StartPosition = FormStartPosition.CenterScreen;

        _nivel.DataSource = Enum.GetValues<NivelGeografico>();
        _nivel.SelectedItem = NivelGeografico.Departamento;
        _rol.DataSource = Enum.GetValues<RolUsuario>();
        _rol.SelectedItem = RolUsuario.Auditor;
        _rol.SelectedIndexChanged += (_, _) => ActualizarPermisos();

        var fila1 = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, WrapContents = false, AutoScroll = true };
        fila1.Controls.Add(Etiqueta("Mesas:"));
        fila1.Controls.Add(_mesas);
        fila1.Controls.Add(Etiqueta("% fraude:"));
        fila1.Controls.Add(_fraude);
        fila1.Controls.Add(Etiqueta("Semilla:"));
        fila1.Controls.Add(_semilla);
        fila1.Controls.Add(Boton("Generar datos", Generar));
        fila1.Controls.Add(Boton("Importar CSV", Importar));
        fila1.Controls.Add(Boton("Registrar acta", RegistrarActa));
        fila1.Controls.Add(Boton("Guardar base", GuardarBd));
        fila1.Controls.Add(Boton("Cargar base", CargarBd));

        var fila2 = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, WrapContents = false, AutoScroll = true };
        Configurar(_btnAlterar, "Alterar 1 acta (demo)", Alterar);
        Configurar(_btnAuditar, "Auditar", Auditar);
        Configurar(_btnEvaluacion, "Evaluación del detector", MostrarEvaluacion);
        Configurar(_btnExportar, "Exportar sospechosas", Exportar);
        Configurar(_btnBitacora, "Bitácora", MostrarBitacora);

        fila2.Controls.Add(Boton("Verificar integridad", Verificar));
        fila2.Controls.Add(_btnAlterar);
        fila2.Controls.Add(Etiqueta("Nivel:"));
        fila2.Controls.Add(_nivel);
        fila2.Controls.Add(Boton("Resultados", MostrarResultados));
        fila2.Controls.Add(_btnAuditar);
        fila2.Controls.Add(_btnEvaluacion);
        fila2.Controls.Add(_btnExportar);
        fila2.Controls.Add(Etiqueta("Usuario:"));
        fila2.Controls.Add(_usuario);
        fila2.Controls.Add(Etiqueta("Rol:"));
        fila2.Controls.Add(_rol);
        fila2.Controls.Add(_btnBitacora);

        var barra = new Panel { Dock = DockStyle.Top, Height = 86 };
        barra.Controls.Add(fila2);
        barra.Controls.Add(fila1);

        Controls.Add(_grid);
        Controls.Add(_estado);
        Controls.Add(barra);

        ActualizarPermisos();
        _bitacora.Registrar(UsuarioActual, RolActual, "Inicio de sesión", "Aplicación abierta");
    }

    private static Label Etiqueta(string texto) => new() { Text = texto, AutoSize = true, Margin = new Padding(8, 10, 0, 0) };

    private static Button Boton(string texto, EventHandler accion)
    {
        var b = new Button { Text = texto, AutoSize = true, Margin = new Padding(6, 6, 0, 0) };
        b.Click += accion;
        return b;
    }

    private static void Configurar(Button boton, string texto, EventHandler accion)
    {
        boton.Text = texto;
        boton.AutoSize = true;
        boton.Margin = new Padding(6, 6, 0, 0);
        boton.Click += accion;
    }

    private RolUsuario RolActual => _rol.SelectedItem is RolUsuario rol ? rol : RolUsuario.Auditor;

    private string UsuarioActual => string.IsNullOrWhiteSpace(_usuario.Text) ? "desconocido" : _usuario.Text.Trim();

    private void Registrar(string accion, string? detalle = null) =>
        _bitacora.Registrar(UsuarioActual, RolActual, accion, detalle);

    private void ActualizarPermisos()
    {
        bool audita = Permisos.PuedeAuditar(RolActual);
        _btnAuditar.Enabled = audita;
        _btnEvaluacion.Enabled = audita;
        _btnExportar.Enabled = Permisos.PuedeExportar(RolActual);
        _btnAlterar.Enabled = Permisos.PuedeEjecutarDemoDeAlteracion(RolActual);
        _btnBitacora.Enabled = Permisos.PuedeVerBitacora(RolActual);

        _estado.Text = $"Rol {RolActual}: " + (audita
            ? "puede auditar, evaluar y exportar."
            : "solo carga y verifica actas; la auditoría está restringida (RF8).");
    }

    private bool HayDatos()
    {
        if (_actas.Count > 0) return true;
        MessageBox.Show("Primero genere o importe actas.", "AuditaActas");
        return false;
    }

    private bool HayPermiso(bool permitido, string mensaje)
    {
        if (permitido) return true;
        MessageBox.Show(mensaje, "Permiso denegado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    private void MostrarActas()
    {
        _grid.DataSource = null;
        _grid.DataSource = _actas
            .Select(a => new
            {
                a.Departamento,
                a.Municipio,
                a.Recinto,
                a.Mesa,
                a.Habilitados,
                a.Emitidos,
                Participacion = a.Participacion.ToString("P1"),
                a.Blancos,
                a.Nulos,
                Etiqueta = a.EsFraudeSimulado ? a.TipoFraudeSimulado : string.Empty,
                Hash = a.Hash[..Math.Min(12, a.Hash.Length)]
            })
            .ToList();
    }

    private void Generar(object? s, EventArgs e)
    {
        if (_actas.Count > 0 &&
            MessageBox.Show("Esto reemplaza las actas en memoria (la base guardada no se toca). ¿Continuar?",
                "AuditaActas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var parametros = new ParametrosSimulacion(
            (int)_mesas.Value, 0.80, (double)_fraude.Value / 100.0, (int)_semilla.Value);

        _actas.Clear();
        _actas.AddRange(new GeneradorService().Generar(parametros));
        CadenaHashService.Encadenar(_actas);
        _resultados = Array.Empty<ResultadoAuditoria>();
        MostrarActas();

        int fraudes = _actas.Count(a => a.EsFraudeSimulado);
        _estado.Text = $"{_actas.Count} actas generadas y encadenadas | {fraudes} con fraude etiquetado | semilla {(int)_semilla.Value}.";
        Registrar("Generar datos", $"mesas={_actas.Count}, fraude={fraudes}, semilla={(int)_semilla.Value}");
    }

    private void Importar(object? s, EventArgs e)
    {
        using var dialogo = new OpenFileDialog
        {
            Filter = "CSV (*.csv)|*.csv|Todos (*.*)|*.*",
            InitialDirectory = Environment.CurrentDirectory
        };
        if (dialogo.ShowDialog() != DialogResult.OK) return;

        var (importadas, errores) = ActaCsv.Importar(dialogo.FileName);
        if (importadas.Count == 0)
        {
            MessageBox.Show(
                "No se importó ninguna acta." + Environment.NewLine +
                string.Join(Environment.NewLine, errores.Take(12)),
                "Importación rechazada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Registrar("Importar CSV", $"rechazado: {dialogo.FileName}");
            return;
        }

        _actas.Clear();
        _actas.AddRange(importadas);
        CadenaHashService.Encadenar(_actas);
        _resultados = Array.Empty<ResultadoAuditoria>();
        MostrarActas();

        _estado.Text = $"{_actas.Count} actas importadas y encadenadas | {errores.Count} problema(s) detectado(s).";
        Registrar("Importar CSV", $"{dialogo.FileName}: {_actas.Count} válidas, {errores.Count} problemas");

        if (errores.Count > 0)
        {
            string detalle = string.Join(Environment.NewLine, errores.Take(12));
            if (errores.Count > 12) detalle += $"{Environment.NewLine}… y {errores.Count - 12} más.";
            MessageBox.Show(detalle, "Filas rechazadas (RF2)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void RegistrarActa(object? s, EventArgs e)
    {
        using var dialogo = new FormActa();
        if (dialogo.ShowDialog(this) != DialogResult.OK || dialogo.Resultado is null) return;

        var resultado = RegistroService.Agregar(_actas, dialogo.Resultado);
        MessageBox.Show(resultado.Mensaje, "Registro de acta",
            MessageBoxButtons.OK, resultado.Exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (!resultado.Exito) return;

        _resultados = Array.Empty<ResultadoAuditoria>();
        MostrarActas();
        _estado.Text = resultado.Mensaje;
        Registrar("Registrar acta", dialogo.Resultado.Clave);
    }

    private void GuardarBd(object? s, EventArgs e)
    {
        if (!HayDatos()) return;

        using var dialogo = new SaveFileDialog
        {
            Filter = "SQLite (*.db)|*.db",
            FileName = "audita.db",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        if (dialogo.ShowDialog() != DialogResult.OK) return;

        try
        {
            using var repo = new ActaRepositorio($"Data Source={dialogo.FileName}");
            repo.InsertarVarias(_actas);
            Registrar("Guardar base de datos", dialogo.FileName);
            foreach (var registro in _bitacora.Registros) repo.InsertarBitacora(registro);

            _rutaBd = dialogo.FileName;
            _estado.Text = $"{_actas.Count} actas guardadas en {_rutaBd}. Las actas son de solo inserción.";
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo guardar la base: " + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CargarBd(object? s, EventArgs e)
    {
        using var dialogo = new OpenFileDialog
        {
            Filter = "SQLite (*.db)|*.db",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        if (dialogo.ShowDialog() != DialogResult.OK) return;

        try
        {
            using var repo = new ActaRepositorio($"Data Source={dialogo.FileName}");
            var leidas = repo.ObtenerTodas();
            if (leidas.Count == 0)
            {
                MessageBox.Show("La base no contiene actas.", "AuditaActas");
                return;
            }

            _actas.Clear();
            _actas.AddRange(leidas);
            _resultados = Array.Empty<ResultadoAuditoria>();
            _rutaBd = dialogo.FileName;
            MostrarActas();

            var verificacion = CadenaHashService.Verificar(_actas);
            _estado.Text = $"{_actas.Count} actas cargadas de {_rutaBd} | {verificacion.Mensaje}";
            Registrar("Cargar base de datos", $"{_rutaBd}: {verificacion.Mensaje}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo cargar la base: " + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Verificar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;

        var verificacion = CadenaHashService.Verificar(_actas);
        _estado.Text = verificacion.Mensaje;
        Registrar("Verificar integridad", verificacion.Integra ? "OK" : verificacion.Clave);

        MostrarActas();
        if (verificacion.Indice is int indice && indice >= 0 && indice < _grid.Rows.Count)
        {
            _grid.Rows[indice].DefaultCellStyle.BackColor = Color.MistyRose;
            _grid.FirstDisplayedScrollingRowIndex = indice;
            _grid.Rows[indice].Selected = true;
        }
    }

    private void Alterar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        if (!HayPermiso(Permisos.PuedeEjecutarDemoDeAlteracion(RolActual),
                "El Operador no puede alterar actas, ni siquiera en la demo (RF8).")) return;

        var acta = _actas[_actas.Count / 2];
        var candidato = acta.Votos.Keys.First();
        acta.Votos[candidato]++;
        _resultados = Array.Empty<ResultadoAuditoria>();

        _estado.Text = $"Se alteró {acta.Clave} (+1 voto a {candidato}). Pulse 'Verificar integridad' para ver la cadena rota.";
        Registrar("Alterar acta (demo)", $"{acta.Clave}, +1 a {candidato}");
    }

    private void MostrarResultados(object? s, EventArgs e)
    {
        if (!HayDatos()) return;

        var nivel = _nivel.SelectedItem is NivelGeografico n ? n : NivelGeografico.Departamento;
        var resultados = ComputoService.Computar(_actas, nivel);

        _grid.DataSource = null;
        _grid.DataSource = resultados
            .Select(r => new
            {
                Clave = r.Clave,
                r.Mesas,
                r.Habilitados,
                r.Emitidos,
                Participacion = r.Habilitados == 0 ? "0.0 %" : ((double)r.Emitidos / r.Habilitados).ToString("P1"),
                r.Blancos,
                r.Nulos,
                Votos = string.Join(" | ", r.Votos.Select(kv => $"{kv.Key}: {kv.Value}"))
            })
            .ToList();

        _estado.Text = $"Cómputo por {nivel}: {resultados.Count} fila(s), {_actas.Count} actas sumadas.";
        Registrar("Ver resultados", nivel.ToString());
    }

    private IReadOnlyList<ResultadoAuditoria> AsegurarAuditoria()
    {
        if (_resultados.Count == 0 && _actas.Count > 0)
            _resultados = new AuditoriaService().Auditar(_actas);
        return _resultados;
    }

    private void Auditar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        if (!HayPermiso(Permisos.PuedeAuditar(RolActual),
                "El rol Operador no accede a la auditoría estadística (RF8). Cambie a Auditor o Administrador.")) return;

        var resultados = AsegurarAuditoria();
        var sospechosas = resultados.Where(r => r.EsSospechosa).OrderByDescending(r => r.Puntaje).ToList();

        _grid.DataSource = null;
        _grid.DataSource = sospechosas
            .Select(r => new
            {
                Acta = r.Acta.Clave,
                Departamento = r.Acta.Departamento,
                Participacion = r.Acta.Participacion.ToString("P1"),
                Puntaje = Math.Round(r.Puntaje, 2),
                r.Motivo
            })
            .ToList();

        string texto = $"{sospechosas.Count} actas sospechosas de {_actas.Count} (umbral {AuditoriaService.UmbralPorDefecto:F1}).";
        if (_actas.Any(a => a.EsFraudeSimulado))
        {
            var resumen = EvaluacionService.Resumir(resultados);
            texto += $" Recall {resumen.Recall:P1} | Falsos positivos {resumen.TasaFalsosPositivos:P1} | Precisión {resumen.Precision:P1}.";
        }
        else
        {
            texto += " Sin etiquetas de fraude: no se calculan métricas.";
        }

        _estado.Text = texto;
        Registrar("Auditar", $"{sospechosas.Count} sospechosas de {_actas.Count}");
    }

    private void MostrarEvaluacion(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        if (!HayPermiso(Permisos.PuedeAuditar(RolActual),
                "El rol Operador no accede a la evaluación del detector (RF8).")) return;

        var resultados = AsegurarAuditoria();
        if (!_actas.Any(a => a.EsFraudeSimulado))
        {
            MessageBox.Show(
                "El dataset actual no tiene etiquetas de fraude (por ejemplo, un CSV real). " +
                "Genere datos con el simulador para calcular precisión y recall.",
                "Sin etiquetas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var r = EvaluacionService.Resumir(resultados);
        var filas = new List<FilaMetrica>
        {
            new("Actas evaluadas", r.Total.ToString()),
            new("Actas con fraude etiquetado", r.FraudesEtiquetados.ToString()),
            new("Alertas emitidas", r.Alertas.ToString()),
            new("Verdaderos positivos (VP)", r.Matriz.VerdaderosPositivos.ToString()),
            new("Falsos positivos (FP)", r.Matriz.FalsosPositivos.ToString()),
            new("Falsos negativos (FN)", r.Matriz.FalsosNegativos.ToString()),
            new("Verdaderos negativos (VN)", r.Matriz.VerdaderosNegativos.ToString()),
            new("Precisión", r.Precision.ToString("P1")),
            new("Recall (sensibilidad)", r.Recall.ToString("P1")),
            new("Tasa de falsos positivos", r.TasaFalsosPositivos.ToString("P1")),
            new("Exactitud", r.Exactitud.ToString("P1")),
            new("F1", r.F1.ToString("P1")),
            new("Umbral de z-score", r.Umbral.ToString("F1"))
        };

        _grid.DataSource = null;
        _grid.DataSource = filas;
        _estado.Text = $"Evaluación del detector (RF7): recall {r.Recall:P1}, falsos positivos {r.TasaFalsosPositivos:P1}.";
        Registrar("Evaluación del detector", $"recall={r.Recall:P1}, fp={r.TasaFalsosPositivos:P1}");
    }

    private void Exportar(object? s, EventArgs e)
    {
        if (!HayDatos()) return;
        if (!HayPermiso(Permisos.PuedeExportar(RolActual),
                "El rol Operador no puede exportar el reporte de auditoría (RF8).")) return;

        var resultados = AsegurarAuditoria();
        using var dialogo = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = "actas_sospechosas.csv",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        if (dialogo.ShowDialog() != DialogResult.OK) return;

        ReporteCsv.Exportar(dialogo.FileName,
            resultados.Where(r => r.EsSospechosa).OrderByDescending(r => r.Puntaje));

        _estado.Text = $"Reporte exportado a {dialogo.FileName}.";
        Registrar("Exportar reporte", dialogo.FileName);
    }

    private void MostrarBitacora(object? s, EventArgs e)
    {
        if (!HayPermiso(Permisos.PuedeVerBitacora(RolActual),
                "Solo el Administrador puede consultar la bitácora (RF8).")) return;

        using var ventana = new FormBitacora(_bitacora.Registros);
        ventana.ShowDialog(this);
    }
}
