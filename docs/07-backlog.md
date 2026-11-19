# 7. Backlog y estado de las tareas

**Convención:** `[x]` = terminado y verificado con evidencia; `[ ]` = pendiente. Una tarea no se marca por
tiempo transcurrido, sino por entregable y prueba.

## 7.1 Semana 1 — Fundamentos

- [x] **T1** Repositorio, solución, `Directory.Build.props` y README (Alex) — `AuditaActas.sln`, build en verde.
- [x] **T2** Spike generador + z-score (Nelson) — `GeneradorService`, `AuditoriaService`, pruebas.
- [x] **T3** Spike hash encadenado con 10.000 registros (Alex) — evidencias §3 (27 ms).
- [ ] **T4** Spike ScottPlot en WinForms (Cristian) — **cancelado**: gráfico postergado a T18; no bloquea.
- [x] **T5** Esquema SQLite y dominio congelados (Alex) — `ActaRepositorio.Esquema`, `Acta.cs`.

## 7.2 Semana 2 — Datos y carga

- [x] **T6** Generador de datos RF1 (Nelson) — 3 pruebas, dataset reproducible.
- [x] **T7** Repositorio, validaciones y alta/importación RF2 (Alex) — `Acta.Validar`, `ActaCsv`, `RegistroService`.
- [x] **T8** Esqueleto de UI y navegación (Cristian) — `FormPrincipal`.

## 7.3 Semana 3 — Núcleo del valor

- [x] **T9** Cadena de hashes y verificación RF3 (Alex) — 6 pruebas.
- [x] **T10** Cómputo por niveles RF4 (Alex) — 2 pruebas.
- [x] **T11** Auditoría z-score RF5 (Nelson) — 5 pruebas, métricas.
- [x] **T12** Pantallas de carga, verificación y resultados (Cristian) — UI completa (sin gráfico).

## 7.4 Semana 4 — MVP

- [x] **T13** Reporte CSV de sospechosas RF6 (Cristian) — `ReporteCsv`, CSV de evidencias.
- [x] **T14** Integración MVP y pruebas de aceptación (Todos) — 47 pruebas, flujo completo.
- [x] **T15** Corrección de errores del MVP (Todos) — build 0 advertencias.

## 7.5 Semana 5 — Post-MVP adelantado

- [x] **T16** Evaluación del detector RF7 (Nelson) — `EvaluacionService`, pantalla, 3 pruebas.
- [x] **T17** Roles y bitácora RF8 (Alex) — `Permisos`, `BitacoraService`, 7 pruebas.
- [ ] **T18** Gráfico de dispersión y pulido de UI (Cristian) — pendiente; no bloquea el MVP.

## 7.6 Semana 6 — Cierre

- [x] **T19** Calibración de umbrales y análisis (Nelson) — barrido 2,0–4,0 en evidencias.
- [x] **T20** Documentación final y manual de usuario (Alex) — docs 01–11, README.
- [ ] **T21** Ensayo de demo y entrega (Todos) — en curso; guion listo, falta ensayar y confirmar fecha.

## 7.7 Backlog futuro (no comprometido)

| Idea | Valor | Por qué no está ahora |
|---|---|---|
| Gráfico de dispersión (T18) | Visualizar outliers | No cambia la decisión; cuesta tiempo en WinForms |
| Chi-cuadrado de homogeneidad | Otra prueba estadística | Requiere más datos por grupo y explicación; post-MVP |
| Exportar PDF con diseño | Presentación formal | El CSV cumple; PDF es estético |
| Importar imágenes/OCR | Automatizar carga | Fuera de alcance; alta complejidad |
| Firma digital del hash raíz | Integridad fuerte | Requiere infraestructura; se declara como límite |
| Autenticación con contraseña | Seguridad real | Prototipo local; roles simples |
| Web/móvil | Acceso remoto | Fuera de alcance del curso |
| Más tipos de fraude | Mayor realismo | Dos tipos cubren el objetivo; se puede extender |

## 7.8 Próximas tres tareas (si el equipo sigue mañana)

| Persona | Próxima tarea | Primer paso concreto |
|---|---|---|
| Alex | T21 + revisión de PR | Confirmar fecha de defensa y ensayar el guion |
| Nelson | T19 (análisis final) | Documentar por qué umbral 3,0 y no 3,5 |
| Cristian | T18 | Evaluar si el gráfico aporta; si no, cerrar la tarea como “no se hará” |

## 7.9 Definition of Done

Ver [`06-pruebas.md`](06-pruebas.md) §6.9. Resumen: build sin advertencias, 47+ pruebas en verde, prueba nueva
si cambió la lógica, UI sin lógica, documentación actualizada y PR revisado por otro integrante.
