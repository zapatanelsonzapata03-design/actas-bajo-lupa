# Evidencia de verificación — AuditaActas

Generado por `src/AuditaActas.Evidencia` el 2026-11-19 13:23:44 (hora local).

Este archivo se regenera con:

```bash
dotnet run --project src/AuditaActas.Evidencia
```

## 1. Dataset estándar, reproducibilidad y cadena de hashes

- **[OK] RF1 misma semilla = mismos datos**: 2000 actas, hash final D7E63436033A7413…
- **[OK] RF1 fraude etiquetado ≈ 5 %**: 106 actas etiquetadas (5.3 %).
- **[OK] RF1 actas internamente consistentes**: 2000 actas sin errores de validación.
- **[OK] RF3 cadena íntegra recién creada**: Integridad OK: 2000 actas verificadas, la cadena es válida.
- **[OK] RF3 alterar un voto rompe la cadena en esa acta**: Cadena rota en la posición 1001: el acta D03/M021/R0101/1 no coincide con su hash.
- **[OK] RF3 al restaurar el dato la cadena vuelve a ser válida**: Integridad OK: 2000 actas verificadas, la cadena es válida.

## 2. Detección (RF5) y evaluación (RF7) sobre 2.000 mesas

| Métrica | Valor | Meta | Estado |
|---|---:|---:|---|
| Recall | 95.3 % | ≥ 80 % | OK |
| Tasa de falsos positivos | 2.8 % | ≤ 10 % | OK |
| Precisión | 65.6 % | informativa | OK |
| Exactitud | 97.1 % | informativa | OK |
| F1 | 77.7 % | informativa | OK |
| Alertas emitidas | 154 | informativa | OK |

Matriz de confusión:

| VP | FP | FN | VN |
|---:|---:|---:|---:|
| 101 | 53 | 5 | 1841 |

- **[OK] RF5 recall ≥ 80 %**: 95.3 %
- **[OK] RF5 falsos positivos ≤ 10 %**: 2.8 %

### Detección por tipo de fraude inyectado

| Tipo | Actas | Detectadas | Recall |
|---|---:|---:|---:|
| transferencia_votos | 48 | 48 | 100.0 % |
| relleno_urna | 58 | 53 | 91.4 % |

### Barrido de umbral (calibración)

| Umbral z | Alertas | Recall | Falsos positivos | Precisión |
|---:|---:|---:|---:|---:|
| 2.0 | 505 | 100.0 % | 21.1 % | 21.0 % |
| 2.5 | 254 | 99.1 % | 7.9 % | 41.3 % |
| 3.0 | 154 | 95.3 % | 2.8 % | 65.6 % |
| 3.5 | 108 | 81.1 % | 1.2 % | 79.6 % |
| 4.0 | 91 | 78.3 % | 0.4 % | 91.2 % |

## 3. Rendimiento (10.000 actas)

- **[OK] Calidad: 10.000 actas auditadas en < 10 s**: 34 ms (generación 86 ms, hash 26 ms, 10000 resultados).

## 4. Importación CSV (RF2)

- **[OK] CSV de ejemplo válido se importa completo**: 3 actas, 0 errores.
- **[OK] CSV con errores rechaza las filas inválidas**: 1 acta válida, 2 rechazos: Fila 3: Votos emitidos (120) superan a los habilitados (100). | Fila 4: la mesa D01/M001/R0001/1 está duplicada en el archivo.

## 5. Persistencia SQLite y bitácora (RF2, RF3, RF8)

- **[OK] Persistencia: 500 actas guardadas y recuperadas en orden**: 500 actas en 23 ms.
- **[OK] Persistencia: la cadena de hashes sobrevive al viaje a SQLite**: Integridad OK: 500 actas verificadas, la cadena es válida.
- **[OK] RF8 bitácora registra usuario, rol, acción y hora**: 1 entrada(s), rol Administrador.

## 6. Roles y permisos (RF8)

- **[OK] Operador no audita**: PuedeAuditar(Operador) = false
- **[OK] Auditor audita y exporta**: PuedeAuditar/Auditor = true
- **[OK] Solo el Administrador ve la bitácora**: PuedeVerBitacora(Admin) = true, (Auditor) = false

| Rol | Registrar | Auditar | Exportar | Ver bitácora |
|---|---:|---:|---:|---:|
| Operador | sí | no | no | no |
| Auditor | sí | sí | sí | no |
| Administrador | sí | sí | sí | sí |

## 7. Resultado final


**Todas las verificaciones pasaron.**

Reporte de actas sospechosas: `docs/evidencias/actas_sospechosas_semilla42.csv`.
