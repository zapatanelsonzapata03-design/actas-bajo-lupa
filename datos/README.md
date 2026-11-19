# Datos

El sistema trabaja con **datos sintéticos**. No se guardan datos personales ni datos reales del OEP.

## Archivos

- `ejemplo_minimo.csv`: 3 actas válidas para probar la importación.
- `ejemplo_con_errores.csv`: incluye un acta con emitidos > habilitados y una mesa duplicada (deben rechazarse).
- `generados/` (ignorado por git): salida de la app o de los scripts.

## Formato

Nueve columnas fijas y luego una columna por candidato:

```
Departamento,Municipio,Recinto,Mesa,Habilitados,Emitidos,Blancos,Nulos,EsFraudeSimulado,Candidato A,Candidato B,Candidato C,Candidato D
```

Reglas: enteros ≥ 0; `Habilitados` > 0; `Emitidos` ≤ `Habilitados`; `Emitidos` = votos + blancos + nulos;
clave `Departamento/Municipio/Recinto/Mesa` única; nombres con comas entre comillas.

## Datasets de prueba

| Uso | Cómo generarlo |
|---|---|
| Dataset estándar (2.000 mesas, 5 % fraude) | App: semilla 42; o `GeneradorService` con `ParametrosSimulacion(2000, 0.80, 0.05, 42)` |
| Sin fraude | `ParametrosSimulacion(1000, 0.80, 0.0, 11)` |
| Estrés (10.000) | `ParametrosSimulacion(10000, 0.80, 0.05, 7)` |
| Evidencia completa | `dotnet run --project src/AuditaActas.Evidencia` |

Los CSV de evidencia se escriben en `docs/evidencias/` (no en `datos/generados/`).
