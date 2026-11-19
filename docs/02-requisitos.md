# 2. Usuarios, requisitos y criterios de aceptación

## 2.1 Objetivo general (medible)

> Desarrollar en 6 semanas un prototipo de escritorio en C# que registre actas electorales simuladas,
> calcule resultados por nivel geográfico y detecte actas anómalas, logrando sobre **2.000 mesas con fraude
> inyectado** una **detección (recall) ≥ 80 %** con **≤ 10 % de falsos positivos**, y auditando **10.000 actas
> en menos de 10 segundos**.

**Cómo se comprueba:** `dotnet run --project src/AuditaActas.Evidencia` genera un reporte con las métricas y
los tiempos. Resultado actual: **recall 95,3 %**, **falsos positivos 2,8 %**, **10.000 actas en ≈32 ms**
(ver [`evidencias/reporte-verificacion.md`](evidencias/reporte-verificacion.md)).

## 2.2 Objetivos específicos

| # | Objetivo | Indicador verificable | Estado |
|---|---|---|---|
| OE1 | Generar datasets sintéticos reproducibles con y sin fraude | Misma semilla produce los mismos hashes y totales; fraude etiquetado ≈ 5 % | Cumplido |
| OE2 | Registrar/importar actas con validaciones | Rechaza suma ≠ emitidos, emitidos > habilitados, mesa duplicada, campos vacíos y valores no numéricos, con error por fila | Cumplido |
| OE3 | Garantizar integridad con cadena de hashes | Alterar un dato rompe la verificación y señala la primera acta alterada | Cumplido |
| OE4 | Computar por mesa/recinto/municipio/departamento/nacional | Totales por nivel suman exactamente el total nacional | Cumplido |
| OE5 | Detectar actas anómalas con z-score robusto | Recall ≥ 80 % y falsos positivos ≤ 10 % sobre 2.000 mesas | Cumplido (95,3 % / 2,8 %) |
| OE6 | Exportar un reporte accionable | CSV con acta, puntaje, z-scores y motivo | Cumplido |

## 2.3 Usuarios y necesidades

| Usuario | Contexto | Necesidad principal | Funcionalidades que la cubren |
|---|---|---|---|
| **Operador de carga** | Personal de apoyo con prisa y poca formación técnica | Cargar actas rápido y sin errores; que el sistema rechace datos inconsistentes | RF1, RF2, RF3 (verificar), persistencia |
| **Auditor/Analista** | Observador o delegado que debe decidir qué revisar | Ver integridad, cómputo y una lista priorizada con motivos | RF3, RF4, RF5, RF6, RF7 |
| **Administrador** | Responsable del prototipo | Controlar permisos y saber quién hizo qué | RF8, bitácora, persistencia |

**Necesidades transversales:** reproducibilidad (misma semilla), explicabilidad (motivo por acta), bajo costo
de instalación (sin servidor), privacidad (sin datos personales) y funcionamiento sin internet.

## 2.4 Historias de usuario

| ID | Historia | Prioridad |
|---|---|---|
| HU1 | Como auditor, quiero generar un dataset de prueba con fraude conocido para medir el detector sin usar datos reales. | Must |
| HU2 | Como operador, quiero importar un CSV y que las filas inválidas se rechacen con el número de fila y el motivo. | Must |
| HU3 | Como operador, quiero registrar un acta manualmente y que no se permita duplicar una mesa. | Must |
| HU4 | Como auditor, quiero verificar que ninguna acta cambió después de registrarse. | Must |
| HU5 | Como auditor, quiero ver los totales por recinto, municipio, departamento y país. | Must |
| HU6 | Como auditor, quiero una lista de actas sospechosas ordenada por puntaje, con el motivo. | Must |
| HU7 | Como auditor, quiero exportar esa lista a CSV para trabajarla en Excel. | Must |
| HU8 | Como auditor, quiero ver precisión, recall y falsos positivos del detector para saber si es confiable. | Should |
| HU9 | Como administrador, quiero que el Operador no acceda a la auditoría y que toda acción quede registrada. | Should |
| HU10 | Como docente, quiero reproducir la demo y las métricas con comandos claros. | Must |

## 2.5 Requisitos funcionales y criterios de aceptación

Los criterios están escritos como **Dado / Cuando / Entonces** para que otro integrante pueda programarlos sin
adivinar. La columna “Evidencia” apunta a la prueba o al artefacto que demuestra el cumplimiento.

### RF1 — Generador de datos sintéticos (Must)

**Descripción:** genera N actas con departamento, municipio, recinto, mesa, habilitados, emitidos, blancos,
nulos y votos por candidato. Parámetros: mesas, participación media, % de fraude y semilla. Inyecta dos tipos
de fraude etiquetado: **relleno de urna** (participación 100 %) y **transferencia de votos** al candidato
favorecido.

**Criterios de aceptación**

- Dado el mismo `ParametrosSimulacion` (mesas, semilla, % fraude), cuando se genera dos veces, entonces los
  datos y la cadena de hashes son idénticos.
- Dado un dataset generado, cuando se valida cada acta, entonces no hay errores de consistencia.
- Dado un 5 % de fraude solicitado sobre 2.000 mesas, entonces se etiquetan entre 3 % y 7 % de actas
  (tolerancia por muestreo aleatorio).
- Dado un acta fraudulenta, entonces `EsFraudeSimulado = true` y `TipoFraudeSimulado` indica el tipo.

**Evidencia:** `GeneradorService`, `GeneradorTests` (3 pruebas), reporte de evidencias §1.

### RF2 — Registro e importación de actas (Must)

**Descripción:** alta manual mediante formulario e importación CSV. Cada acta se valida antes de entrar.
Formato CSV: nueve columnas fijas (`Departamento, Municipio, Recinto, Mesa, Habilitados, Emitidos, Blancos,
Nulos, EsFraudeSimulado`) seguidas de una columna por candidato.

**Criterios de aceptación**

- Dado un acta con suma de votos + blancos + nulos ≠ emitidos, cuando se registra, entonces se rechaza con
  mensaje que muestra ambas cifras.
- Dado un acta con emitidos > habilitados, cuando se registra, entonces se rechaza.
- Dada una mesa cuya clave `Departamento/Municipio/Recinto/Mesa` ya existe (sin distinguir mayúsculas),
  cuando se registra, entonces se rechaza y no se modifica el acta original.
- Dado un CSV con una fila no numérica, entonces se rechaza esa fila y el mensaje indica la columna.
- Dado un CSV con encabezado inválido, entonces no se importa ninguna acta y se explica el formato esperado.
- Dado un CSV con nombres de candidato entre comillas y comas, entonces se conserva el nombre completo.
- Dado un CSV con filas válidas e inválidas, entonces se importan las válidas y se informa cada rechazo con su
  número de fila.

**Evidencia:** `Acta.Validar`, `ActaCsv.ImportarTexto`, `RegistroService`, pruebas `ActaTests` (4),
`RegistroTests` (5), `InfraestructuraTests` (8).

### RF3 — Cadena de hashes SHA-256 (Must)

**Descripción:** cada acta guarda `hash_anterior` y `hash = SHA-256(datos + hash_anterior)`. La primera acta
usa `GENESIS`. La verificación recorre la cadena y detecta el primer eslabón roto. Las actas son de solo
inserción: no hay edición ni borrado.

**Criterios de aceptación**

- Dada una cadena recién creada, cuando se verifica, entonces es íntegra.
- Dado un cambio en cualquier campo hasheado de un acta, cuando se verifica, entonces se informa el índice y
  la clave del **primer** acta alterada.
- Dado que se restaura el dato original, entonces la verificación vuelve a ser íntegra.
- Dada una base SQLite con actas encadenadas, cuando se leen, entonces la cadena sigue siendo válida.
- Dado un lote nuevo que continúa otro, entonces se puede indicar el hash inicial en lugar de `GENESIS`.

**Evidencia:** `CadenaHashService`, `CadenaHashTests` (6), `InfraestructuraTests`, evidencias §1 y §5.

### RF4 — Cómputo por niveles (Must)

**Descripción:** agrega mesas, habilitados, emitidos, blancos, nulos y votos por candidato en cuatro niveles:
recinto, municipio, departamento y nacional.

**Criterios de aceptación**

- Dado un conjunto de actas, cuando se computa el nivel nacional, entonces sus emitidos son la suma de todos
  los emitidos y sus votos son la suma de todos los votos.
- Dado el nivel departamento, entonces la suma de emitidos de todos los departamentos coincide con el total
  nacional.
- Dado un nivel, entonces cada fila incluye la clave geográfica y la cantidad de mesas.

**Evidencia:** `ComputoService`, `ComputoTests` (2), pantalla “Resultados”.

### RF5 — Auditoría estadística (Must)

**Descripción:** agrupa las actas por municipio (con respaldo a departamento y nacional si hay menos de 8
actas) y calcula z-scores robustos de participación y de proporción de voto por candidato. El puntaje es el
máximo valor absoluto. Supera el umbral (por defecto 3,0) ⇒ sospechosa, con motivo textual. La alerta es un
**indicio**, no una prueba.

**Criterios de aceptación**

- Dado el dataset estándar (2.000 mesas, semilla 42, 5 % fraude), entonces recall ≥ 80 % y falsos positivos
  ≤ 10 %.
- Dado un dataset sin fraude, entonces las alertas son una minoría (prueba: < 100 sobre 1.000 actas).
- Dado un umbral mayor, entonces la cantidad de alertas no aumenta.
- Dado un dataset de menos de 8 actas, entonces no se lanza una excepción y se usa el respaldo.
- Dado un umbral ≤ 0, entonces se rechaza con `ArgumentOutOfRangeException`.

**Evidencia:** `AuditoriaService`, `Estadistica`, `AuditoriaTests` (5), `EstadisticaTests` (3), evidencias §2.

### RF6 — Reporte de sospechosas (Must)

**Descripción:** exporta a CSV las actas marcadas, ordenadas por puntaje, con clave, ubicación, participación,
puntaje, z-scores y motivo.

**Criterios de aceptación**

- Dado un conjunto auditado, cuando se exporta, entonces el CSV tiene encabezado y una fila por acta
  sospechosa.
- Cada fila incluye el motivo que disparó la alerta.
- Los campos con comas o comillas se escapan correctamente.
- El archivo se abre en Excel (UTF-8 con BOM).

**Evidencia:** `ReporteCsv`, botón “Exportar sospechosas”, archivo
[`evidencias/actas_sospechosas_semilla42.csv`](evidencias/actas_sospechosas_semilla42.csv).

### RF7 — Evaluación del detector (Should, posterior al MVP)

**Descripción:** compara las alertas con las etiquetas del simulador y muestra la matriz de confusión y las
métricas: precisión, recall, tasa de falsos positivos, exactitud y F1.

**Criterios de aceptación**

- Dados resultados con etiquetas, entonces VP + FP + FN + VN = total de actas.
- Dados 0 negativos o 0 positivos, entonces las métricas devuelven 0 y no dividen por cero.
- Dado el dataset estándar, entonces la pantalla y el reporte muestran las mismas métricas.

**Evidencia:** `EvaluacionService`, `EvaluacionTests` (3), pantalla “Evaluación del detector”.

### RF8 — Roles y bitácora (Should, posterior al MVP)

**Descripción:** tres roles. El **Operador** carga y verifica, pero no audita, evalúa, exporta ni altera. El
**Auditor** audita, evalúa y exporta. El **Administrador** además consulta la bitácora. Cada acción se registra
con usuario, rol, acción, detalle y hora UTC.

**Criterios de aceptación**

- Dado el rol Operador, entonces los botones de auditoría/evaluación/exportación están deshabilitados y el
  servicio también rechaza la operación.
- Dado el rol Auditor, entonces puede auditar y exportar, pero no ver la bitácora.
- Dado el rol Administrador, entonces puede ver la bitácora.
- Dada una acción, entonces queda un registro con fecha UTC, usuario y rol.
- Al guardar la base, las entradas de la sesión se copian a la tabla `bitacora`.

**Evidencia:** `Permisos`, `BitacoraService`, `ActaRepositorio.ListarBitacora`, `SeguridadTests` (3),
`BitacoraTests` (4), evidencias §6.

## 2.6 Requisitos de calidad (no funcionales)

| ID | Calidad | Requisito medible | Cómo se verifica | Estado |
|---|---|---|---|---|
| RNF1 | Integridad | Actas inmutables; cualquier cambio posterior se detecta | Cadena SHA-256 y prueba de alteración | Cumple |
| RNF2 | Rendimiento | 10.000 actas auditadas en < 10 s | `RendimientoTests`; evidencia: ≈32 ms | Cumple |
| RNF3 | Usabilidad | Flujo completo de demo en < 5 min | Guion de demo cronometrado | Cumple |
| RNF4 | Reproducibilidad | Misma semilla ⇒ mismos datos y métricas | `GeneradorTests`, evidencia §1 | Cumple |
| RNF5 | Compatibilidad | Windows 10/11 + .NET 8; núcleo multiplataforma | `dotnet build`/`dotnet test`; UI solo Windows | Cumple |
| RNF6 | Privacidad | Sin datos personales ni datos reales | Revisión de datasets y esquema | Cumple |
| RNF7 | Mantenibilidad | Capas separadas, UI sin lógica, pruebas automatizadas | Arquitectura + 47 pruebas + 0 advertencias | Cumple |
| RNF8 | Explicabilidad | Cada alerta incluye el motivo y el valor z | Reporte y pantalla de auditoría | Cumple |
| RNF9 | Disponibilidad local | Funciona sin servidor ni internet (tras restaurar paquetes) | Ejecución local | Cumple |
| RNF10 | Seguridad de acceso | Permisos por rol y bitácora | `Permisos`, pruebas y UI | Cumple (prototipo) |

**Limitaciones de calidad declaradas:** no hay autenticación con contraseña, cifrado en reposo ni protección
contra un atacante que reescriba toda la base y recalcule la cadena. Para datos reales se necesitaría firma
digital, anclaje externo del hash y control de acceso real. Ver [`03-arquitectura.md`](03-arquitectura.md).

## 2.7 Definición del MVP

**MVP = RF1 + RF2 + RF3 + RF4 + RF5 + RF6 + persistencia SQLite + flujo de UI + pruebas + documentación.**

El MVP permite: generar o importar actas, verificar integridad, computar resultados, auditar, ver sospechosas
y exportar el reporte. Es una parte real del problema: ya reduce el universo de revisión y detecta alteraciones.

**Fuera del MVP (implementado como extensión temprana):** RF7 (evaluación) y RF8 (roles y bitácora). Se
adelantaron porque el núcleo ya estaba estable y aportan evidencia para la defensa; no cambian el alcance del
MVP ni la prioridad de entrega.

**Posterior (no implementado):** gráfico de dispersión, chi-cuadrado, PDF con diseño, autenticación real,
importación de imágenes/OCR.

## 2.8 Fuera de alcance y supuestos

Ver [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.8–1.9. Resumen: no votación, no OCR, no datos
reales, no web/móvil, no blockchain, no conclusiones legales.

## 2.9 Trazabilidad resumida

| Problema | Objetivo | Requisito | Tarea | Prueba | Evidencia |
|---|---|---|---|---|---|
| No hay datos de prueba realistas | OE1 | RF1 | T6 | `GeneradorTests` | Evidencias §1 |
| Carga manual con errores/duplicados | OE2 | RF2 | T7 | `ActaTests`, `RegistroTests`, `InfraestructuraTests` | Evidencias §4 |
| Alteración posterior invisible | OE3 | RF3 | T9 | `CadenaHashTests` | Evidencias §1, §5 |
| No hay totales confiables | OE4 | RF4 | T10 | `ComputoTests` | Pantalla Resultados |
| Revisión subjetiva | OE5 | RF5, RF7 | T11, T16 | `AuditoriaTests`, `EvaluacionTests` | Evidencias §2 |
| No hay lista accionable | OE6 | RF6 | T13 | `ReporteCsv` + demo | CSV de evidencias |
| Sin control de acceso | — | RF8 | T17 | `SeguridadTests`, `BitacoraTests` | Evidencias §6 |

Matriz completa en [`11-matriz-trazabilidad.md`](11-matriz-trazabilidad.md).
