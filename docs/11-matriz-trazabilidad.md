# 11. Matriz de trazabilidad

Una fila por necesidad. Si algo no tiene prueba o evidencia, no está terminado.

## 11.1 Problema → objetivo → requisito → tarea → prueba → evidencia

| # | Causa / necesidad | Objetivo | Requisito | Tarea | Prueba / verificación | Evidencia |
|---|---|---|---|---|---|---|
| 1 | No hay datos de prueba realistas ni reproducibles | OE1 | RF1 | T2, T6 | `GeneradorTests` (3) | Evidencias §1; semilla 42 |
| 2 | Carga manual con errores, campos vacíos, no numéricos | OE2 | RF2 | T7 | `ActaTests` (4), `InfraestructuraTests` (8) | Evidencias §4; `datos/ejemplo_con_errores.csv` |
| 3 | Duplicados que inflan totales | OE2 | RF2 | T7 | `RegistroTests` (5), `Csv_ConErrores...` | `clave UNIQUE`; mensaje “mesa duplicada” |
| 4 | Alteración posterior invisible | OE3 | RF3 | T3, T9 | `CadenaHashTests` (6) | Evidencias §1, §5 |
| 5 | No hay totales confiables por nivel | OE4 | RF4 | T10 | `ComputoTests` (2) | Pantalla “Resultados”; evidencia |
| 6 | Revisión subjetiva y lenta | OE5 | RF5 | T11 | `AuditoriaTests` (5), `EstadisticaTests` (3) | Evidencias §2 (95,3 % / 2,8 %) |
| 7 | No hay lista accionable | OE6 | RF6 | T13 | `ReporteCsv` + CSV | `evidencias/actas_sospechosas_semilla42.csv` |
| 8 | No se sabe si el detector es confiable | OE5 | RF7 | T16 | `EvaluacionTests` (3) | Pantalla “Evaluación”; evidencias §2 |
| 9 | Cualquiera puede auditar o alterar | — | RF8 | T17 | `SeguridadTests` (3) | Tabla de permisos; UI deshabilitada |
| 10 | No hay registro de quién hizo qué | — | RF8 | T17 | `BitacoraTests` (4) | Tabla `bitacora`; pantalla “Bitácora” |
| 11 | El sistema debe ser rápido | RNF2 | — | T9–T11 | `RendimientoTests` (1) | 10.000 actas en ≈34 ms |
| 12 | Debe ser reproducible | RNF4 | RF1 | T2, T6 | `MismaSemilla...` | Evidencias §1 |
| 13 | Debe poder ejecutarse sin servidor | RNF5/RNF9 | — | T1, T7 | `dotnet build/test`; SQLite local | README; CI |
| 14 | No debe usar datos personales | RNF6 | — | T6, T7 | Revisión de datasets y esquema | `datos/README.md`; alcance |
| 15 | La UI no debe contener lógica | RNF7 | — | T8, T12 | Revisión de PR + pruebas de servicios | Arquitectura §3.1 |

## 11.2 Requisito → pruebas (mapa ejecutable)

| Requisito | Pruebas principales |
|---|---|
| RF1 | `GeneradorTests.MismaSemilla_GeneraLosMismosDatos`, `TodasLasActasGeneradas_SonConsistentes`, `ProporcionDeFraude_EsAproximadamenteLaPedida` |
| RF2 | `ActaTests.*`, `RegistroTests.*`, `InfraestructuraTests.Csv_*` |
| RF3 | `CadenaHashTests.*`, `InfraestructuraTests.Repositorio_GuardaYRecupera...` |
| RF4 | `ComputoTests.TotalNacional_...`, `PorDepartamento_...` |
| RF5 | `AuditoriaTests.DatasetEstandar_...`, `DatosSinFraude_...`, `DatasetPequeno_...`, `UmbralMuyAlto_...`, `UmbralInvalido_...`; `EstadisticaTests.*` |
| RF6 | `ReporteCsv` + archivo de evidencias + demo |
| RF7 | `EvaluacionTests.Resumen_...`, `Resumen_SinDatos_...`, `BarridoDeUmbral_...` |
| RF8 | `SeguridadTests.*`, `BitacoraTests.*` |
| RNF2 | `RendimientoTests.AuditarDiezMilActas_...` |
| RNF4 | `GeneradorTests.MismaSemilla_...` + evidencias |

## 11.3 Tarea → entregable → estado

| Tarea | Entregable | Estado | Dónde se ve |
|---|---|---|---|
| T1 | Solución y README | Hecho | `AuditaActas.sln`, `README.md` |
| T2 | Generador + z-score | Hecho | `GeneradorService`, `AuditoriaService` |
| T3 | Hash con 10k | Hecho | Evidencias §3 |
| T4 | Spike ScottPlot | Cancelado | Decisión de postergar (T18) |
| T5 | Esquema y dominio | Hecho | `ActaRepositorio.Esquema`, `Acta.cs` |
| T6 | RF1 | Hecho | `GeneradorService` |
| T7 | RF2 | Hecho | `ActaCsv`, `RegistroService` |
| T8 | UI base | Hecho | `FormPrincipal` |
| T9 | RF3 | Hecho | `CadenaHashService` |
| T10 | RF4 | Hecho | `ComputoService` |
| T11 | RF5 | Hecho | `AuditoriaService` |
| T12 | Pantallas | Hecho | `FormPrincipal` |
| T13 | RF6 | Hecho | `ReporteCsv` |
| T14 | Integración MVP | Hecho | 47 pruebas; evidencias |
| T15 | Correcciones | Hecho | Build 0 advertencias |
| T16 | RF7 | Hecho | `EvaluacionService` |
| T17 | RF8 | Hecho | `Permisos`, `BitacoraService` |
| T18 | Dispersión | Pendiente (post-MVP) | — |
| T19 | Calibración | Hecho | Evidencias §2 |
| T20 | Documentación | Hecho | Docs 01–11 |
| T21 | Ensayo/entrega | En curso | Guion de demo |

## 11.4 Criterio de auditoría

Para cada fila de §11.1 se puede responder:

1. **¿Qué problema resuelve?** Columna 2.
2. **¿Qué requisito lo cubre?** Columna 4.
3. **¿Quién lo hizo?** [`04-plan-gantt.md`](04-plan-gantt.md) §4.3.
4. **¿Cómo se comprueba?** Columna 6.
5. **¿Dónde está la evidencia?** Columna 7.

Si una fila no tuviera evidencia, el requisito no se consideraría terminado.
