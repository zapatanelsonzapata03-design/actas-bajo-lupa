# 3. Arquitectura

```
UI (WinForms)  →  Aplicacion (servicios)  →  Dominio  ←  Infraestructura
 FormPrincipal     GeneradorService          Acta         ActaRepositorio (SQLite)
                   CadenaHashService         Modelos      ActaCsv / ReporteCsv
                   ComputoService
                   AuditoriaService / Estadistica
                   EvaluacionService
```
**Regla:** la UI nunca calcula; solo llama a servicios. Así la lógica se prueba sin abrir formularios.

## Decisiones tecnológicas
| Decisión | Justificación |
|---|---|
| C# / .NET 8 | Lenguaje del curso y del equipo |
| Windows Forms | Conocido por el equipo; suficiente para el prototipo |
| SQLite | Sin servidor que instalar; el docente ejecuta sin configurar; SQL estándar |
| z-score robusto (mediana y MAD) | Los outliers no inflan la desviación y enmascaran el fraude |
| SHA-256 encadenado | Detecta cualquier alteración posterior |
| xUnit | Estándar de pruebas en .NET |
| MathNet.Numerics (ya referenciado) | Reservado para chi-cuadrado y distribuciones |
| ScottPlot (pendiente, T4) | `dotnet add src/AuditaActas.UI package ScottPlot.WinForms` |

## Partes difíciles / inciertas
1. Modelo de datos sintéticos realista. 2. Calibrar umbrales. 3. Gráficos en WinForms. 4. Orden de actas en la cadena con importación masiva.

## Esquema de datos
Tablas `actas` (inserción solamente) y `bitacora`; el SQL está en `ActaRepositorio.Esquema`.
Cada acta guarda `hash_anterior` y `hash`. No hay datos personales.
