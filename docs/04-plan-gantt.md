# 4. Plan de desarrollo (6 semanas)

Equipo de tres personas. Inicio declarado: 5 de noviembre de 2026; según el equipo, al 19 de noviembre ya transcurrieron dos semanas. Las fechas son una proyección de seis semanas desde ese inicio, no una fecha de entrega acordada. La fecha de entrega o defensa y los requisitos del docente siguen pendientes. Las tareas de periodos anteriores no se marcan como concluidas sin evidencia.

| Periodo (proyección) | ID | Tarea | Resp. | Horas | Depende de |
|---|---|---|---|---|---|
| 05–11 nov. 2026 | T1 | Repositorio, estructura de solución y README | Alex | 3 | — |
| 05–11 nov. 2026 | T2 | Spike: generador multinomial + z-score en consola | Nelson | 6 | T1 |
| 05–11 nov. 2026 | T3 | Spike: hash encadenado con 10.000 registros | Alex | 4 | T1 |
| 05–11 nov. 2026 | T4 | Spike: gráfico ScottPlot en WinForms | Cristián | 3 | T1 |
| 05–11 nov. 2026 | T5 | Esquema SQLite y modelo de dominio (se congela) | Alex | 5 | T1 |
| 12–18 nov. 2026 | T6 | Generador de datos (RF1) | Nelson | 10 | T2, T5 |
| 12–18 nov. 2026 | T7 | Repositorio de actas y validaciones (RF2) | Alex | 10 | T5 |
| 12–18 nov. 2026 | T8 | Esqueleto de UI y navegación | Cristián | 8 | T4 |
| 19–25 nov. 2026 | T9 | Cadena de hashes y verificación (RF3) | Alex | 8 | T7 |
| 19–25 nov. 2026 | T10 | Cómputo por niveles (RF4) | Alex | 8 | T7 |
| 19–25 nov. 2026 | T11 | Auditoría z-score (RF5) | Nelson | 10 | T6, T10 |
| 19–25 nov. 2026 | T12 | Pantallas de carga, verificación y resultados | Cristián | 10 | T8, T9, T10 |
| 26 nov.–02 dic. 2026 | T13 | Reporte CSV de sospechosas (RF6) | Cristián | 6 | T11 |
| 26 nov.–02 dic. 2026 | T14 | **Integración MVP** y pruebas de aceptación | Todos | 10 | T9–T13 |
| 26 nov.–02 dic. 2026 | T15 | Corrección de errores del MVP | Todos | 8 | T14 |
| 03–09 dic. 2026 | T16 | Evaluación del detector (RF7, posterior al MVP) | Nelson | 8 | T14 |
| 03–09 dic. 2026 | T17 | Roles y bitácora (RF8, posterior al MVP) | Alex | 8 | T14 |
| 03–09 dic. 2026 | T18 | Gráfico de dispersión y pulido de UI (posterior al MVP) | Cristián | 8 | T14 |
| 10–16 dic. 2026 | T19 | Calibración de umbrales y análisis de resultados | Nelson | 6 | T16 |
| 10–16 dic. 2026 | T20 | Documentación final y manual de usuario | Alex | 8 | T14 |
| 10–16 dic. 2026 | T21 | Ensayo de la demo y entrega | Todos | 6 | T15–T20 |

**Tareas que bloquean:** T5 (esquema), T7 (repositorio/validaciones) y T14 (integración). T5 se cierra y congela en la semana 1.

> Nota: el código inicial del ZIP adelanta una primera versión de T5, T6, T7 (parcial), T9, T10, T11, T13 y T16. En esta máquina, `dotnet build` finalizó sin advertencias ni errores y `dotnet test` aprobó 20 pruebas; esos resultados no validan por sí solos las métricas de detección ni los criterios funcionales completos.
