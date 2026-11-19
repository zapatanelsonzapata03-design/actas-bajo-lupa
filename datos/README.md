# Datos

El sistema trabaja con **datos sintéticos**. No se guardan datos personales.

- `ejemplo_minimo.csv`: 3 actas válidas para probar la importación.
- `ejemplo_con_errores.csv`: incluye una acta con emitidos > habilitados y una mesa duplicada (deben rechazarse).
- Para un dataset grande: abrir la app, **Generar datos** (semilla 42, 2.000 mesas) o usar `GeneradorService`.
- `datos/generados/` está ignorado por git.
