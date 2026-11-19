# 6. Estrategia de pruebas

| Nivel | Qué se prueba | Herramienta |
|---|---|---|
| Unitarias | Validaciones de acta, hash, mediana/z-score (contrastados con Excel o Python), cómputo | xUnit |
| Integración | Importar CSV → guardar en SQLite → computar → exportar | xUnit con `Data Source=:memory:` |
| Aceptación | Criterios de `docs/02-requisitos.md` con el dataset estándar (semilla 42) | Lista de verificación firmada |
| Evaluación | Precisión, recall y falsos positivos | `AuditoriaTests` y RF7 |

Las pruebas se escriben junto con cada funcionalidad. `dotnet test` debe estar en verde antes de cada PR.

## Lista de verificación de aceptación del MVP
- [ ] RF1: misma semilla produce los mismos datos
- [ ] RF2: se rechazan actas inválidas y duplicadas con mensaje por fila
- [ ] RF3: alterar un voto rompe la verificación y señala el acta
- [ ] RF4: totales iguales a Excel
- [ ] RF5: sobre 2.000 mesas, recall ≥ 80 % y falsos positivos ≤ 10 %
- [ ] RF6: CSV con puntaje y motivo
- [ ] La demo corre desde cero siguiendo el README en < 15 minutos
