using System.Diagnostics;
using System.Globalization;
using System.Text;
using AuditaActas.Aplicacion;
using AuditaActas.Dominio;
using AuditaActas.Infraestructura;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

string raiz = BuscarRaiz();
string carpetaEvidencias = Path.Combine(raiz, "docs", "evidencias");
Directory.CreateDirectory(carpetaEvidencias);

var informe = new StringBuilder();
var fallos = new List<string>();
int paso = 0;

void Seccion(string titulo)
{
    paso++;
    Console.WriteLine();
    Console.WriteLine($"== {paso}. {titulo} ==");
    informe.AppendLine();
    informe.AppendLine($"## {paso}. {titulo}");
    informe.AppendLine();
}

void Veredicto(string nombre, bool cumple, string detalle)
{
    string marca = cumple ? "OK" : "FALLA";
    if (!cumple) fallos.Add(nombre);
    Console.WriteLine($"[{marca}] {nombre}: {detalle}");
    informe.AppendLine($"- **[{marca}] {nombre}**: {detalle}");
}

informe.AppendLine("# Evidencia de verificación — AuditaActas");
informe.AppendLine();
informe.AppendLine($"Generado por `src/AuditaActas.Evidencia` el {DateTime.Now:yyyy-MM-dd HH:mm:ss} (hora local).");
informe.AppendLine();
informe.AppendLine("Este archivo se regenera con:");
informe.AppendLine();
informe.AppendLine("```bash");
informe.AppendLine("dotnet run --project src/AuditaActas.Evidencia");
informe.AppendLine("```");

// ---------------------------------------------------------------------------
Seccion("Dataset estándar, reproducibilidad y cadena de hashes");
var parametros = new ParametrosSimulacion(Mesas: 2000, PorcentajeFraude: 0.05, Semilla: 42);
var generador = new GeneradorService();
var actas = generador.Generar(parametros);
var actas2 = generador.Generar(parametros);
CadenaHashService.Encadenar(actas);
CadenaHashService.Encadenar(actas2);

bool reproducibles = actas.Count == actas2.Count
    && actas.Zip(actas2).All(par => par.First.Hash == par.Second.Hash && par.First.Emitidos == par.Second.Emitidos);
Veredicto("RF1 misma semilla = mismos datos", reproducibles, $"{actas.Count} actas, hash final {actas[^1].Hash[..16]}…");

int fraudes = actas.Count(a => a.EsFraudeSimulado);
Veredicto("RF1 fraude etiquetado ≈ 5 %", fraudes is >= 60 and <= 140, $"{fraudes} actas etiquetadas ({fraudes / (double)actas.Count:P1}).");

bool consistentes = actas.All(a => a.Validar().Count == 0);
Veredicto("RF1 actas internamente consistentes", consistentes, $"{actas.Count} actas sin errores de validación.");

var verificacion = CadenaHashService.Verificar(actas);
Veredicto("RF3 cadena íntegra recién creada", verificacion.Integra, verificacion.Mensaje);

int indiceAlterado = actas.Count / 2;
var actaAlterada = actas[indiceAlterado];
string candidato = actaAlterada.Votos.Keys.First();
actaAlterada.Votos[candidato]++;
var rota = CadenaHashService.Verificar(actas);
Veredicto("RF3 alterar un voto rompe la cadena en esa acta",
    !rota.Integra && rota.Indice == indiceAlterado && rota.Clave == actaAlterada.Clave,
    rota.Mensaje);
actaAlterada.Votos[candidato]--;
var restaurada = CadenaHashService.Verificar(actas);
Veredicto("RF3 al restaurar el dato la cadena vuelve a ser válida", restaurada.Integra, restaurada.Mensaje);

// ---------------------------------------------------------------------------
Seccion("Detección (RF5) y evaluación (RF7) sobre 2.000 mesas");
var auditoria = new AuditoriaService();
var resultados = auditoria.Auditar(actas);
var resumen = EvaluacionService.Resumir(resultados);

informe.AppendLine("| Métrica | Valor | Meta | Estado |");
informe.AppendLine("|---|---:|---:|---|");
void FilaMetrica(string nombre, string valor, string meta, bool cumple) =>
    informe.AppendLine($"| {nombre} | {valor} | {meta} | {(cumple ? "OK" : "FALLA")} |");

FilaMetrica("Recall", resumen.Recall.ToString("P1"), "≥ 80 %", resumen.Recall >= 0.80);
FilaMetrica("Tasa de falsos positivos", resumen.TasaFalsosPositivos.ToString("P1"), "≤ 10 %", resumen.TasaFalsosPositivos <= 0.10);
FilaMetrica("Precisión", resumen.Precision.ToString("P1"), "informativa", true);
FilaMetrica("Exactitud", resumen.Exactitud.ToString("P1"), "informativa", true);
FilaMetrica("F1", resumen.F1.ToString("P1"), "informativa", true);
FilaMetrica("Alertas emitidas", resumen.Alertas.ToString(), "informativa", true);

informe.AppendLine();
informe.AppendLine("Matriz de confusión:");
informe.AppendLine();
informe.AppendLine("| VP | FP | FN | VN |");
informe.AppendLine("|---:|---:|---:|---:|");
informe.AppendLine($"| {resumen.Matriz.VerdaderosPositivos} | {resumen.Matriz.FalsosPositivos} | {resumen.Matriz.FalsosNegativos} | {resumen.Matriz.VerdaderosNegativos} |");
informe.AppendLine();

Veredicto("RF5 recall ≥ 80 %", resumen.Recall >= 0.80, resumen.Recall.ToString("P1"));
Veredicto("RF5 falsos positivos ≤ 10 %", resumen.TasaFalsosPositivos <= 0.10, resumen.TasaFalsosPositivos.ToString("P1"));

informe.AppendLine();
informe.AppendLine("### Detección por tipo de fraude inyectado");
informe.AppendLine();
informe.AppendLine("| Tipo | Actas | Detectadas | Recall |");
informe.AppendLine("|---|---:|---:|---:|");
foreach (var grupo in resultados.Where(r => r.Acta.EsFraudeSimulado).GroupBy(r => r.Acta.TipoFraudeSimulado))
{
    int total = grupo.Count();
    int detectadas = grupo.Count(r => r.EsSospechosa);
    informe.AppendLine($"| {grupo.Key} | {total} | {detectadas} | {detectadas / (double)total:P1} |");
    Console.WriteLine($"[INFO] {grupo.Key}: {detectadas}/{total} ({detectadas / (double)total:P1})");
}

informe.AppendLine();
informe.AppendLine("### Barrido de umbral (calibración)");
informe.AppendLine();
informe.AppendLine("| Umbral z | Alertas | Recall | Falsos positivos | Precisión |");
informe.AppendLine("|---:|---:|---:|---:|---:|");
foreach (var r in EvaluacionService.BarrerUmbral(actas, new[] { 2.0, 2.5, 3.0, 3.5, 4.0 }))
{
    informe.AppendLine($"| {r.Umbral:F1} | {r.Alertas} | {r.Recall:P1} | {r.TasaFalsosPositivos:P1} | {r.Precision:P1} |");
}

string rutaSospechosas = Path.Combine(carpetaEvidencias, "actas_sospechosas_semilla42.csv");
ReporteCsv.Exportar(rutaSospechosas, resultados.Where(r => r.EsSospechosa).OrderByDescending(r => r.Puntaje));

// ---------------------------------------------------------------------------
Seccion("Rendimiento (10.000 actas)");
var reloj = Stopwatch.StartNew();
var grandes = generador.Generar(new ParametrosSimulacion(Mesas: 10000, PorcentajeFraude: 0.05, Semilla: 7));
reloj.Stop();
long msGeneracion = reloj.ElapsedMilliseconds;

reloj.Restart();
CadenaHashService.Encadenar(grandes);
reloj.Stop();
long msHash = reloj.ElapsedMilliseconds;

reloj.Restart();
var resultadosGrandes = auditoria.Auditar(grandes);
reloj.Stop();
long msAuditoria = reloj.ElapsedMilliseconds;

Veredicto("Calidad: 10.000 actas auditadas en < 10 s", msAuditoria < 10_000,
    $"{msAuditoria} ms (generación {msGeneracion} ms, hash {msHash} ms, {resultadosGrandes.Count} resultados).");

// ---------------------------------------------------------------------------
Seccion("Importación CSV (RF2)");
string rutaMinimo = Path.Combine(raiz, "datos", "ejemplo_minimo.csv");
string rutaErrores = Path.Combine(raiz, "datos", "ejemplo_con_errores.csv");

var (minimas, erroresMinimo) = ActaCsv.Importar(rutaMinimo);
Veredicto("CSV de ejemplo válido se importa completo", minimas.Count == 3 && erroresMinimo.Count == 0,
    $"{minimas.Count} actas, {erroresMinimo.Count} errores.");

var (conErrores, erroresDetectados) = ActaCsv.Importar(rutaErrores);
Veredicto("CSV con errores rechaza las filas inválidas", conErrores.Count == 1 && erroresDetectados.Count == 2,
    $"{conErrores.Count} acta válida, {erroresDetectados.Count} rechazos: {string.Join(" | ", erroresDetectados)}");

// ---------------------------------------------------------------------------
Seccion("Persistencia SQLite y bitácora (RF2, RF3, RF8)");
using (var repo = new ActaRepositorio("Data Source=:memory:"))
{
    var lote = actas.Take(500).ToList();
    reloj.Restart();
    repo.InsertarVarias(lote);
    reloj.Stop();
    var recuperadas = repo.ObtenerTodas();
    bool ordenOk = recuperadas.Select(a => a.Clave).SequenceEqual(lote.Select(a => a.Clave));
    var cadenaBd = CadenaHashService.Verificar(recuperadas);

    Veredicto("Persistencia: 500 actas guardadas y recuperadas en orden", recuperadas.Count == 500 && ordenOk,
        $"{recuperadas.Count} actas en {reloj.ElapsedMilliseconds} ms.");
    Veredicto("Persistencia: la cadena de hashes sobrevive al viaje a SQLite", cadenaBd.Integra, cadenaBd.Mensaje);

    repo.InsertarBitacora(new RegistroBitacora(1, DateTime.UtcNow, "evidencia", RolUsuario.Administrador, "Prueba de bitácora", "detalle"));
    var bitacora = repo.ListarBitacora();
    Veredicto("RF8 bitácora registra usuario, rol, acción y hora", bitacora.Count == 1 && bitacora[0].Usuario == "evidencia",
        $"{bitacora.Count} entrada(s), rol {bitacora[0].Rol}.");
}

// ---------------------------------------------------------------------------
Seccion("Roles y permisos (RF8)");
Veredicto("Operador no audita", !Permisos.PuedeAuditar(RolUsuario.Operador), "PuedeAuditar(Operador) = false");
Veredicto("Auditor audita y exporta", Permisos.PuedeAuditar(RolUsuario.Auditor) && Permisos.PuedeExportar(RolUsuario.Auditor),
    "PuedeAuditar/Auditor = true");
Veredicto("Solo el Administrador ve la bitácora", Permisos.PuedeVerBitacora(RolUsuario.Administrador) && !Permisos.PuedeVerBitacora(RolUsuario.Auditor),
    "PuedeVerBitacora(Admin) = true, (Auditor) = false");

informe.AppendLine();
informe.AppendLine("| Rol | Registrar | Auditar | Exportar | Ver bitácora |");
informe.AppendLine("|---|---:|---:|---:|---:|");
foreach (var rol in Enum.GetValues<RolUsuario>())
{
    informe.AppendLine($"| {rol} | {SiNo(Permisos.PuedeRegistrar(rol))} | {SiNo(Permisos.PuedeAuditar(rol))} | " +
                       $"{SiNo(Permisos.PuedeExportar(rol))} | {SiNo(Permisos.PuedeVerBitacora(rol))} |");
}

// ---------------------------------------------------------------------------
Seccion("Resultado final");
informe.AppendLine();
if (fallos.Count == 0)
{
    informe.AppendLine("**Todas las verificaciones pasaron.**");
    Console.WriteLine("Todas las verificaciones pasaron.");
}
else
{
    informe.AppendLine($"**{fallos.Count} verificación(es) fallaron:** {string.Join(", ", fallos)}.");
    Console.WriteLine($"{fallos.Count} verificación(es) fallaron: {string.Join(", ", fallos)}");
}

informe.AppendLine();
informe.AppendLine($"Reporte de actas sospechosas: `{Path.GetRelativePath(raiz, rutaSospechosas).Replace('\\', '/')}`.");

string rutaInforme = Path.Combine(carpetaEvidencias, "reporte-verificacion.md");
File.WriteAllText(rutaInforme, informe.ToString(), new UTF8Encoding(false));
Console.WriteLine($"Informe escrito en {rutaInforme}");

return fallos.Count == 0 ? 0 : 1;

static string SiNo(bool valor) => valor ? "sí" : "no";

static string BuscarRaiz()
{
    var actual = new DirectoryInfo(AppContext.BaseDirectory);
    while (actual is not null && !File.Exists(Path.Combine(actual.FullName, "AuditaActas.sln")))
        actual = actual.Parent;
    return actual?.FullName ?? Environment.CurrentDirectory;
}
