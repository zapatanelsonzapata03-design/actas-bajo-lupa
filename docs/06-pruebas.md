# 6. Estrategia y evidencia de pruebas

## 6.1 Principios

1. **La lógica se prueba sin abrir ventanas.** La UI solo orquesta; los servicios de Aplicacion se prueban con
   xUnit.
2. **Cada requisito tiene al menos una prueba o un artefacto de evidencia.**
3. **Los datos son reproducibles.** Semilla 42 para el dataset estándar; cualquier persona obtiene lo mismo.
4. **`main` siempre compila y pasa las pruebas.** El CI lo verifica en cada push/PR.
5. **Se declara lo que no se prueba.** La UI no tiene pruebas automatizadas de interacción; se valida con el
   guion de demo.

## 6.2 Niveles de prueba

| Nivel | Qué se prueba | Herramienta | Dónde |
|---|---|---|---|
| Unitarias | Validaciones de acta, hash, mediana/MAD, cómputo, registro | xUnit | `tests/AuditaActas.Tests` |
| Integración | CSV → actas → SQLite → lectura → cadena; bitácora | xUnit + `Data Source=:memory:` | `InfraestructuraTests`, `BitacoraTests` |
| Aceptación | Criterios de RF1–RF8 y métricas del objetivo general | xUnit + reporte de evidencias | `AuditoriaTests`, `EvaluacionTests`, evidencias |
| Rendimiento | 10.000 actas en < 10 s | Stopwatch + xUnit | `RendimientoTests` |
| Manual/UI | Flujo completo, permisos visibles, exportación | Guion de demo | [`10-guion-demo.md`](10-guion-demo.md) |

## 6.3 Suite automatizada: 47 pruebas

| Archivo | Pruebas | Qué cubre |
|---|---:|---|
| `ActaTests.cs` | 4 | Acta válida, emitidos > habilitados, suma ≠ emitidos, negativos |
| `GeneradorTests.cs` | 3 | Reproducibilidad, consistencia, proporción de fraude |
| `CadenaHashTests.cs` | 6 | Cadena íntegra, alteración, génesis, verificación detallada, extensión, hash inicial |
| `ComputoTests.cs` | 2 | Total nacional y suma por departamento |
| `EstadisticaTests.cs` | 3 | Mediana par/impar, outlier, MAD = 0 |
| `AuditoriaTests.cs` | 5 | Metas de detección, sin fraude, dataset pequeño, umbral alto, umbral inválido |
| `EvaluacionTests.cs` | 3 | Matriz/métricas, sin datos, barrido de umbral |
| `RegistroTests.cs` | 5 | Alta válida, duplicado, inválida, etiqueta, cadena al final |
| `SeguridadTests.cs` | 3 | Permisos por rol |
| `BitacoraTests.cs` | 4 | Bitácora de sesión, SQLite, conteo/claves, duplicado en repositorio |
| `InfraestructuraTests.cs` | 8 | SQLite, CSV ida/vuelta, filas inválidas, comillas/comas, encabezado, no numérico, vacío |
| `RendimientoTests.cs` | 1 | 10.000 actas < 10 s |
| **Total** | **47** | |

## 6.4 Cómo ejecutar

```bash
dotnet restore
dotnet build              # 0 advertencias, 0 errores
dotnet test               # 47 pruebas aprobadas
dotnet run --project src/AuditaActas.Evidencia   # genera docs/evidencias/reporte-verificacion.md
```

Salida esperada de `dotnet test`:

```
Correctas! - Con error: 0, Superado: 47, Omitido: 0, Total: 47
```

El CI (`.github/workflows/ci.yml`) repite `restore`, `build -warnaserror`, `test`, evidencia y publica el
reporte como artefacto.

## 6.5 Evidencia de aceptación del objetivo general

Dataset: 2.000 mesas, participación media 0,80, 5 % de fraude, semilla 42.

| Métrica | Meta | Resultado | Estado |
|---|---:|---:|---|
| Recall (detección) | ≥ 80 % | **95,3 %** | Cumple |
| Tasa de falsos positivos | ≤ 10 % | **2,8 %** | Cumple |
| Precisión | Informativa | 65,6 % | — |
| F1 | Informativa | 77,7 % | — |
| Alertas | Informativa | 154 de 2.000 | — |
| Auditoría de 10.000 actas | < 10 s | **≈34 ms** | Cumple |

- Reporte completo: [`evidencias/reporte-verificacion.md`](evidencias/reporte-verificacion.md)
- CSV de actas sospechosas: [`evidencias/actas_sospechosas_semilla42.csv`](evidencias/actas_sospechosas_semilla42.csv)
- Script reproducible: `src/AuditaActas.Evidencia/Program.cs`

**Interpretación honesta:** estas métricas miden el detector contra el fraude que el propio simulador inyecta.
Demuestran que el método funciona en ese escenario controlado; **no** demuestran validez contra datos reales.
Esa limitación se declara en la UI, el README y la defensa.

## 6.6 Lista de verificación de aceptación del MVP

- [x] **RF1:** misma semilla produce los mismos datos (`GeneradorTests.MismaSemilla_GeneraLosMismosDatos`).
- [x] **RF1:** todas las actas generadas son consistentes (`TodasLasActasGeneradas_SonConsistentes`).
- [x] **RF2:** se rechazan actas inválidas y duplicadas con mensaje por fila (`Csv_ConErrores_...`, `Agregar_MesaDuplicada...`).
- [x] **RF2:** se conservan nombres de candidato con comas (`Csv_ConNombreDeCandidatoEntreComillas...`).
- [x] **RF3:** alterar un voto rompe la verificación y señala el acta (`AlterarUnVoto_RompeLaCadenaEnEsaActa`).
- [x] **RF3:** la cadena sobrevive a SQLite (`Repositorio_GuardaYRecupera...`).
- [x] **RF4:** totales por nivel coinciden con la suma (`ComputoTests`).
- [x] **RF5:** sobre 2.000 mesas, recall ≥ 80 % y FP ≤ 10 % (`AuditoriaTests`, evidencias).
- [x] **RF6:** CSV con puntaje y motivo (`ReporteCsv` + archivo de evidencias).
- [x] **RF7:** matriz y métricas sin dividir por cero (`EvaluacionTests`).
- [x] **RF8:** el Operador no audita; solo el Administrador ve la bitácora (`SeguridadTests`).
- [x] **RF8:** la bitácora guarda usuario, rol, acción y hora (`BitacoraTests`).
- [x] **RNF2:** 10.000 actas auditadas en < 10 s (`RendimientoTests`, ≈34 ms).
- [x] **RNF7:** build sin advertencias y 47 pruebas en verde.
- [x] **RNF4:** la demo se reproduce con el README y la semilla 42.

## 6.7 Datos de prueba

| Archivo / semilla | Contenido | Uso |
|---|---|---|
| Semilla 42, 2.000 mesas, 5 % fraude | Dataset estándar | Métricas de aceptación |
| Semilla 11, 1.000 mesas, 0 % fraude | Datos sin fraude | Control de falsos positivos |
| Semilla 7, 10.000 mesas | Estrés | Rendimiento |
| `datos/ejemplo_minimo.csv` | 3 actas válidas | Importación feliz |
| `datos/ejemplo_con_errores.csv` | 1 válida + 1 inconsistente + 1 duplicada | Rechazo por fila |

## 6.8 Qué NO se prueba (y cómo se mitiga)

| No cubierto | Riesgo | Mitigación |
|---|---|---|
| Interacción real de la UI (clics, layout) | Un botón puede fallar en otra resolución | Demo manual, layout con `Dock`/`AutoScroll`, guion de demo |
| Gráfico ScottPlot | No implementado (T18) | Post-MVP; no afecta el objetivo |
| Datos reales del OEP | Sin permisos y fuera de alcance | Se declara; no se usan datos personales |
| Autenticación real y cifrado | Prototipo local | Se declara como límite; roles simples |
| Ataque con control total del archivo | La cadena se puede recalcular | Firma digital/anclaje externo para producción |
| Pruebas de carga concurrente | Un solo usuario local | Fuera de alcance (sin servidor) |

## 6.9 Criterio para aceptar un cambio (Definition of Done)

Un cambio está terminado cuando:

1. `dotnet build` no tiene errores ni advertencias.
2. `dotnet test` pasa (47+ pruebas, 0 fallos).
3. Se agregó o actualizó al menos una prueba si cambió la lógica.
4. La UI sigue sin lógica de negocio.
5. La documentación afectada se actualizó (requisito, plan, README o guion).
6. Otro integrante revisó el PR y respondió la lista de verificación.
