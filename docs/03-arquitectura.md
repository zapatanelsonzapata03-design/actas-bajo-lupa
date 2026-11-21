# 3. Arquitectura y viabilidad técnica

## 3.1 Vista general

```
┌─────────────────────────────────────────────────────────────────────┐
│ AuditaActas.UI (WinForms, net8.0-windows)                           │
│ FormPrincipal · FormActa · FormBitacora                             │
│  - muestra datos, captura acciones, aplica permisos visuales        │
│  - NO calcula: delega en Aplicacion y persistencia                  │
└───────────────┬───────────────────────────────┬─────────────────────┘
                │                               │
                ▼                               ▼
┌───────────────────────────────┐   ┌─────────────────────────────────┐
│ AuditaActas.Aplicacion        │   │ AuditaActas.Infraestructura     │
│ GeneradorService             │   │ ActaCsv (import/export)         │
│ CadenaHashService            │   │ ReporteCsv                      │
│ ComputoService               │   │ ActaRepositorio (SQLite)        │
│ Estadistica                  │   │                                 │
│ AuditoriaService             │   │                                 │
│ EvaluacionService            │   │                                 │
│ RegistroService              │   │                                 │
│ BitacoraService              │   │                                 │
└───────────────┬───────────────┘   └───────────────┬─────────────────┘
                │                                   │
                ▼                                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│ AuditaActas.Dominio (net8.0)                                        │
│ Acta · ResultadoAgregado · ResultadoAuditoria · MatrizConfusion     │
│ ResumenEvaluacion · NivelGeografico · RolUsuario · Permisos         │
│ RegistroBitacora                                                    │
└─────────────────────────────────────────────────────────────────────┘
```

**Regla de dependencias:** `UI → Aplicacion/Infraestructura → Dominio`. El Dominio no conoce a nadie; la
Aplicación no conoce WinForms ni SQLite; la UI no contiene fórmulas ni validaciones de negocio.

**Proyecto adicional de evidencia:** `src/AuditaActas.Evidencia` (consola `net8.0`) referencia Aplicacion,
Infraestructura y Dominio, ejecuta el dataset estándar y escribe
[`docs/evidencias/reporte-verificacion.md`](evidencias/reporte-verificacion.md). No es parte del producto;
es el mecanismo reproducible para que el docente compruebe las métricas.

## 3.2 Proyectos y responsabilidades

| Proyecto | Framework | Responsabilidad | Archivos clave |
|---|---|---|---|
| `AuditaActas.Dominio` | net8.0 | Modelo, validaciones del acta, métricas, permisos | `Acta.cs`, `Modelos.cs`, `Seguridad.cs` |
| `AuditaActas.Aplicacion` | net8.0 | Casos de uso y cálculos | `GeneradorService.cs`, `CadenaHashService.cs`, `ComputoService.cs`, `Estadistica.cs`, `AuditoriaService.cs`, `EvaluacionService.cs`, `RegistroService.cs`, `BitacoraService.cs` |
| `AuditaActas.Infraestructura` | net8.0 | CSV y SQLite | `ActaCsv.cs`, `ReporteCsv.cs`, `ActaRepositorio.cs` |
| `AuditaActas.UI` | net8.0-windows | Interfaz WinForms | `FormPrincipal.cs`, `FormActa.cs`, `FormBitacora.cs`, `Program.cs` |
| `AuditaActas.Tests` | net8.0 | Pruebas xUnit | 12 archivos, 47 pruebas |
| `AuditaActas.Evidencia` | net8.0 | Reporte reproducible de verificación | `Program.cs` |
| `datos/` | — | CSV de ejemplo | `ejemplo_minimo.csv`, `ejemplo_con_errores.csv` |

## 3.3 Flujos principales

### Flujo A — Generar y guardar

1. `FormPrincipal` lee mesas, % fraude y semilla.
2. `GeneradorService.Generar` crea las actas; `Random(semilla)` garantiza reproducibilidad.
3. `CadenaHashService.Encadenar` calcula `hash_anterior`/`hash` de cada acta en orden.
4. La UI muestra las actas.
5. “Guardar base” abre `ActaRepositorio`, inserta en una transacción y copia la bitácora de la sesión.

### Flujo B — Importar CSV

1. `ActaCsv.Importar` lee el archivo con BOM, separa campos respetando comillas y valida el encabezado.
2. Por cada fila: parsea números, construye `Acta`, ejecuta `Acta.Validar`, detecta duplicados por clave.
3. Devuelve `(actasVálidas, errores)`; la UI informa fila y motivo.
4. Las actas válidas se encadenan y reemplazan el conjunto en memoria (la base guardada no se toca).

### Flujo C — Verificar integridad

1. `CadenaHashService.Verificar` recorre desde `GENESIS` (o un hash inicial).
2. Recalcula el hash de cada acta con sus datos actuales y compara con el almacenado.
3. Si hay diferencia, devuelve índice, clave y mensaje; la UI resalta la fila en rojo.

### Flujo D — Auditar y exportar

1. `AuditoriaService.Auditar` agrupa por municipio; si hay < 8 actas, usa departamento y luego nacional.
2. `Estadistica.ZRobusto` calcula mediana y MAD (escala 1,4826) de participación y de proporción por candidato.
3. Cada acta recibe `ZParticipacion`, `ZApoyo`, `Puntaje`, `EsSospechosa` y `Motivo`.
4. `EvaluacionService.Resumir` compara con las etiquetas del simulador (si existen).
5. `ReporteCsv.Exportar` escribe el CSV de sospechosas.

### Flujo E — Roles y bitácora

1. La UI selecciona usuario y rol (Operador/Auditor/Administrador).
2. `Permisos` decide qué botones se habilitan y los métodos vuelven a verificar antes de ejecutar.
3. `BitacoraService` registra cada acción en memoria; al guardar, se copia a la tabla `bitacora`.

## 3.4 Modelo de datos

### Dominio

`Acta`: Departamento, Municipio, Recinto, Mesa, Habilitados, Emitidos, Blancos, Nulos,
`Dictionary<string,int> Votos`, `EsFraudeSimulado` (etiqueta, no se hashea), `TipoFraudeSimulado`,
`HashAnterior`, `Hash`. Propiedades derivadas: `Clave`, `Participacion`, `VotosValidos`. Método `Validar()`.

`ResultadoAgregado`: clave, mesas, habilitados, emitidos, blancos, nulos, votos por candidato.
`ResultadoAuditoria`: acta, z de participación, z de apoyo, puntaje, es sospechosa, motivo.
`MatrizConfusion` / `ResumenEvaluacion`: VP, FP, FN, VN y métricas derivadas.
`RegistroBitacora`: id, fecha UTC, usuario, rol, acción, detalle.

### SQLite (solo inserción)

```sql
CREATE TABLE actas (
    orden INTEGER PRIMARY KEY AUTOINCREMENT,
    clave TEXT NOT NULL UNIQUE,
    departamento TEXT NOT NULL,
    municipio TEXT NOT NULL,
    recinto TEXT NOT NULL,
    mesa INTEGER NOT NULL,
    habilitados INTEGER NOT NULL,
    emitidos INTEGER NOT NULL,
    blancos INTEGER NOT NULL,
    nulos INTEGER NOT NULL,
    votos_json TEXT NOT NULL,
    es_fraude_simulado INTEGER NOT NULL DEFAULT 0,
    hash_anterior TEXT NOT NULL,
    hash TEXT NOT NULL
);

CREATE TABLE bitacora (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha_utc TEXT NOT NULL,
    usuario TEXT NOT NULL,
    rol TEXT NOT NULL DEFAULT 'Operador',
    accion TEXT NOT NULL,
    detalle TEXT
);
```

- `clave UNIQUE` impide duplicados a nivel de base.
- No se exponen `UPDATE` ni `DELETE` en `ActaRepositorio`.
- `votos_json` guarda el diccionario de candidatos; permite candidatos variables sin migrar el esquema.
- `orden` conserva la secuencia de la cadena de hashes.

## 3.5 Decisiones tecnológicas (con justificación y alternativas)

| # | Decisión | Alternativa descartada | Por qué |
|---|---|---|---|
| D1 | C# + .NET 8 | Python/Java | Lenguaje del curso, tipado fuerte, un solo SDK y pruebas integradas. |
| D2 | WinForms | WPF, web, consola | El equipo lo conoce, es suficiente para escritorio y no requiere servidor. La consola se usó solo para evidencia. |
| D3 | SQLite | SQL Server, PostgreSQL, archivos | Sin servidor que instalar; el docente ejecuta sin configuración; transacciones y `UNIQUE`. |
| D4 | z-score robusto (mediana/MAD) | media/desviación estándar, ML, Benford | Los outliers inflan la media y el detector se enmascara; mediana/MAD es explicable y reproducible. |
| D5 | SHA-256 encadenado | hash por acta aislado, blockchain | Un hash aislado no protege el orden ni detecta borrados; blockchain es sobredimensionado y no aporta al prototipo. |
| D6 | xUnit | NUnit, MSTest | Estándar de facto en .NET, integración con `dotnet test` y CI. |
| D7 | CSV como intercambio | Excel binario, PDF | Legible, versionable, sin dependencias y abrible en Excel. |
| D8 | Código WinForms sin diseñador | `.Designer.cs` | Menos archivos, revisable en PR, sin problemas de merge; el layout es explícito. |
| D9 | Evaluación con etiquetas del simulador | Validación con datos reales | No hay datos reales disponibles ni permiso; el simulador permite medir y auditar el método. |
| D10 | Roles simples en memoria + bitácora SQLite | ASP.NET Identity, OAuth | El prototipo es local y académico; autenticación real está fuera de alcance. |

**MathNet.Numerics** se retiró de las dependencias: estaba reservado para chi-cuadrado, que no se implementa. Se agregará
solo si esa prueba entra al alcance.

## 3.6 Partes técnicamente más difíciles o inciertas

| # | Dificultad | Por qué | Cómo se redujo |
|---|---|---|---|
| 1 | Simulador realista | Si el fraude es trivial, el detector “funciona” sin demostrar nada | Dos tipos de fraude, ruido de participación, pesos por municipio, semilla reproducible y métricas honestas |
| 2 | Calibrar el umbral | Un umbral bajo dispara falsos positivos; uno alto pierde casos | Barrido de umbral documentado (2,0→4,0) y umbral 3,0 elegido con recall 95,3 % / FP 2,8 % |
| 3 | Grupos pequeños | Mediana/MAD no es representativa con pocas actas | Respaldo municipio → departamento → nacional (mínimo 8 actas) |
| 4 | Orden de la cadena en importación masiva | Si el orden cambia, la cadena se rompe | Se encadena en el orden de la lista; SQLite conserva `orden`; prueba de ida y vuelta |
| 5 | Gráficos en WinForms | ScottPlot agrega dependencia y tiempo | Se posterga (T18); el reporte CSV y la tabla cumplen el objetivo |
| 6 | UI sin lógica | Es fácil poner fórmulas en el formulario | Servicios en Aplicacion; la UI solo orquesta; pruebas cubren la lógica sin abrir ventanas |

## 3.7 Validación técnica realizada (spikes)

| Spike | Resultado | Evidencia |
|---|---|---|
| Generador + z-score | Métricas sobre 2.000 mesas | Evidencias §2 (recall 95,3 %, FP 2,8 %) |
| Hash encadenado con 10.000 registros | Cadena íntegra y rápida | Evidencias §3 (hash 26 ms) |
| SQLite ida y vuelta | 500 actas recuperadas en orden y cadena válida | Evidencias §5 |
| CSV con comas y BOM | Importación correcta | `InfraestructuraTests` |
| Rendimiento | 10.000 actas auditadas en ≈34 ms | `RendimientoTests`, evidencias §3 |
| UI WinForms | Compila y ejecuta en Windows | `dotnet build`, `dotnet run --project src/AuditaActas.UI` |

**Validado el 21/11/2026:** la UI WinForms se ejecutó en Windows con el flujo completo (ver [`evidencias/validacion-ui-windows.md`](evidencias/validacion-ui-windows.md)).

**No validado todavía:** gráfico ScottPlot (postergado), ejecución en el equipo del docente. El riesgo se
mitiga con el README y una instalación limpia (T20/T21).

## 3.8 Dependencias externas

| Dependencia | Versión | Uso | ¿Bloquea el inicio? |
|---|---|---|---|
| .NET SDK | 8.0 (probado también con 10.0.400) | Compilar y ejecutar | No; se instala una vez |
| Windows 10/11 | — | UI WinForms | Solo para la UI; el núcleo corre en cualquier SO |
| Microsoft.Data.Sqlite | 8.0.10 | Persistencia | No; se restaura con NuGet |
| xUnit + runner | 2.9.2 / 2.8.2 | Pruebas | No; se restaura con NuGet |
| NuGet.org | — | Descarga de paquetes la primera vez | Solo la primera restauración; después funciona offline |

No hay APIs, credenciales, servidores ni permisos externos.

## 3.9 Seguridad, privacidad y límites

**Controles implementados**

- Actas de solo inserción: el repositorio no expone edición ni borrado.
- `clave UNIQUE` y validaciones para evitar duplicados e inconsistencias.
- Cadena SHA-256 que detecta cambios accidentales o parciales.
- Roles y bitácora con usuario, rol, acción y hora UTC.
- Datos sintéticos: no hay información personal.

**Límites que se declaran en la defensa**

- La cadena detecta alteraciones, pero **no** protege contra un atacante que tenga control total del archivo y
  recalcule todos los hashes. Para producción se necesitaría firma digital y anclaje externo (por ejemplo,
  publicar el hash raíz en un medio independiente).
- No hay autenticación con contraseña; el rol se elige en la interfaz. Es un prototipo académico.
- SQLite no está cifrado. No se deben cargar datos personales o reales.
- La bitácora se persiste al guardar la base; si la aplicación se cierra sin guardar, queda solo en memoria.
- El reporte puede contener actas con fraude inyectado (etiqueta del simulador); no son datos reales.

## 3.10 Estructura del repositorio

```
AuditaActas/
├─ AuditaActas.sln
├─ Directory.Build.props          # net8.0, nullable, implicit usings
├─ README.md
├─ CONTRIBUTING.md
├─ docs/                          # problema, requisitos, arquitectura, plan, riesgos, pruebas,
│  ├─ 08-cuestionario-aprobacion.md   # respuestas a las 52 preguntas
│  ├─ 09-manual-usuario.md
│  ├─ 10-guion-demo.md
│  ├─ 11-matriz-trazabilidad.md
│  └─ evidencias/                 # reporte generado + CSV de sospechosas
├─ datos/                         # CSV de ejemplo
├─ src/
│  ├─ AuditaActas.Dominio/
│  ├─ AuditaActas.Aplicacion/
│  ├─ AuditaActas.Infraestructura/
│  ├─ AuditaActas.UI/
│  └─ AuditaActas.Evidencia/
└─ tests/AuditaActas.Tests/
```

## 3.11 Cómo se mantiene la arquitectura

- La UI no contiene cálculos; solo llama a servicios (revisión en PR).
- Las pruebas de Aplicacion no referencian WinForms.
- `Directory.Build.props` activa `Nullable` e `ImplicitUsings`; el build debe quedar sin advertencias.
- `dotnet test` y el reporte de evidencias se ejecutan en CI (`.github/workflows/ci.yml`).
- Cualquier decisión nueva se agrega a la tabla §3.5 y se refleja en el README.
