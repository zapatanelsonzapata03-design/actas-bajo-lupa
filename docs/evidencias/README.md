# Evidencias de verificación

Esta carpeta contiene artefactos **generados** por el proyecto, no escritos a mano.

| Archivo | Qué contiene | Cómo se regenera |
|---|---|---|
| `reporte-verificacion.md` | Reproducibilidad, cadena de hashes, recall/FP, rendimiento, CSV, SQLite, bitácora y permisos | `dotnet run --project src/AuditaActas.Evidencia` |
| `actas_sospechosas_semilla42.csv` | Reporte de actas sospechosas del dataset estándar (2.000 mesas, semilla 42, 5 % fraude) | Igual que el anterior |

| `validacion-ui-windows.md` + `capturas/` | Capturas de la UI WinForms ejecutada en Windows (21/11/2026): flujo completo, importación CSV, guardar/cargar base, roles y bitácora | Manual: ejecutar la UI y capturar (ver el documento) |

El reporte incluye la fecha y hora de generación. Los valores pueden variar ligeramente entre ejecuciones
(por ejemplo, milisegundos); las **metas** y los **umbrales** no cambian. La semilla 42 hace que los datos y
las métricas sean reproducibles.

**Última verificación local:** `dotnet build` con 0 advertencias/0 errores, `dotnet test` con 47 pruebas,
todas las comprobaciones del reporte en OK.

Si el docente quiere verificar sin abrir la UI:

```bash
dotnet test
dotnet run --project src/AuditaActas.Evidencia
```

El segundo comando imprime el mismo resumen en consola y reescribe esta carpeta. El código de la consola está
en `src/AuditaActas.Evidencia/Program.cs` y es la fuente de verdad de cada número.
