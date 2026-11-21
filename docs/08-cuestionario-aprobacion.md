# 8. Cuestionario de aprobación: respuestas y evidencia

Este documento responde las **52 preguntas** del cuestionario del Ing. Diego Cárdenas (Programación II – P2).
Cada respuesta es corta para la exposición y termina con la **evidencia** que se puede abrir en el repositorio
o ejecutar en la demo. La regla es: **no se afirma nada que no esté en el código, en una prueba o en un
documento**.

> **Cómo usar este documento en la defensa:** lea la respuesta en negrita; si el docente pide “muéstreme”,
> abra la evidencia indicada. El guion cronometrado está en [`10-guion-demo.md`](10-guion-demo.md).

## Dictamen actual

**Resultado: APROBADO PARA INICIAR Y CONTINUAR EL DESARROLLO, con condiciones de cierre menores.**

La auditoría de preparación cumple las 52 preguntas. La evidencia ejecutable actual confirma:

| Verificación | Resultado |
|---|---|
| Compilación | Correcta, 0 advertencias y 0 errores |
| Pruebas automatizadas | 47 de 47 aprobadas |
| MVP RF1–RF6 | Implementado y verificable |
| Métrica de detección | 95,3 % recall y 2,8 % falsos positivos |
| Rendimiento | 10.000 actas auditadas en ≈34 ms |

No se encontraron decisiones fundamentales bloqueantes ni requisitos críticos sin comprender. Las siguientes
acciones son condiciones de cierre, no correcciones que invaliden la aprobación:

| Prioridad | Corrección o acción pendiente | Responsable sugerido |
|---|---|---|
| Alta | Ensayar la demo completa y confirmar fecha/formato de entrega o defensa | Todos; Alex coordina |
| Media | UI validada en un Windows limpio el 21/11/2026 (flujo completo, CSV, roles, bitácora, guardar/cargar base; ver [`evidencias/validacion-ui-windows.md`](evidencias/validacion-ui-windows.md)). **Falta** repetirla en el equipo del docente | Cristian |
| Media | Confirmar formalmente si el docente acepta datos sintéticos, escritorio WinForms y SQLite | Equipo completo |
| Cerrada | T18 (gráfico): documentada como **no comprometida** (backlog §7.7) | Cristian |

**Resultado técnico:** el proyecto no requiere detenerse para corregir el núcleo. Debe completar la
validación externa y ensayar la defensa antes de entregar.

## Resumen de cumplimiento

| Sección | Preguntas | Estado | Dónde está la prueba |
|---|---|---|---|
| 1. Problema y valor | 1–6 | Cumple | Docs 01, README |
| 2. Objetivos y éxito | 7–10 | Cumple | Docs 02, evidencias |
| 3. Usuarios y requisitos | 11–15 | Cumple | Docs 02, pruebas |
| 4. Alcance | 16–20 | Cumple | Docs 01 §1.8–1.10 |
| 5. MVP | 21–24 | Cumple | Docs 02 §2.7 |
| 6. Diseño y viabilidad | 25–30 | Cumple | Docs 03 |
| 7. Datos, seguridad y calidad | 31–34 | Cumple | Docs 03, 06 |
| 8. Plan de desarrollo | 35–40 | Cumple | Docs 04, 07 |
| 9. Equipo y riesgos | 41–45 | Cumple | Docs 04, 05 |
| 10. Preparación | 46–50 | Cumple | Este documento, README |
| 11. Prueba de fracaso | 51–52 | Cumple | Docs 05 §5.5 |

---

# 1. Problema y valor

### 1. ¿Pueden explicar el problema en menos de 30 segundos sin mencionar ninguna tecnología?

Sí. El resultado de una elección se arma sumando miles de actas llenadas a mano. Si un acta se altera
después de cerrarse, se duplica o se llena con números inventados, hoy detectarlo depende de que alguien lo
note revisando papeles. El problema es doble: **no hay forma de demostrar que lo contado sigue siendo lo que
decía el acta** y **no hay un criterio objetivo para decidir cuáles de miles de actas revisar primero**. La
revisión manual es lenta, cara y subjetiva; el resultado es desconfianza y conflictos.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.1–1.2; README, sección “El problema
en 30 segundos”.

### 2. ¿Quién tiene el problema y quién utilizará la solución?

Lo tienen los **observadores electorales, delegados de partido y tribunales**: deben cuidar la integridad del
conteo y responder a cuestionamientos con evidencia. También lo sufre la ciudadanía, que no puede verificar
por sí misma. La solución la usan tres perfiles: **Operador de carga** (registra/importa actas), **Auditor o
analista** (verifica integridad, computa, audita y exporta) y **Administrador** (controla accesos y revisa la
bitácora). El docente y los compañeros son usuarios de la demo y de la documentación.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.3; [`02-requisitos.md`](02-requisitos.md)
§2.3; roles en `src/AuditaActas.Dominio/Seguridad.cs`.

### 3. ¿Qué evidencia demuestra que el problema es real, relevante o necesario?

Hay tres tipos de evidencia, usados con cuidado para no exagerar:

1. **Contexto internacional:** la auditoría de la OEA a las elecciones de 2019 documentó la interrupción del
   TREP al 83,85 %, problemas de cadena de custodia y que más del 5 % de las imágenes de actas pasaron directo
   al cómputo oficial.
2. **Volumen y operación real:** el TSE informó que SCORC procesó el 100 % de **35.253 actas** en la segunda
   vuelta de 2025; revisar eso a mano en plazos cortos es inviable.
3. **Cobertura del cómputo:** la prensa reportó avances parciales y cierres por departamento, lo que muestra
   la necesidad de leer resultados parciales con trazabilidad.

**Aclaración obligatoria:** estas fuentes contextualizan el problema; **no** demuestran fraude en 2025 ni
validan nuestro detector. El prototipo usa datos sintéticos.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.4, con enlaces a OEA, OEP y El Deber.

### 4. ¿Qué consecuencias tiene actualmente ese problema?

Revisión manual lenta y costosa; priorización subjetiva (dos revisores eligen actas distintas); alteraciones
posteriores invisibles si nadie guardó una huella de los datos originales; duplicados que inflan totales;
desconfianza ciudadana aunque el conteo sea correcto; conflictos postelectorales, impugnaciones y pérdida de
legitimidad. En el aula, la consecuencia es que un proyecto “bonito” sin evidencia no se puede defender.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.5.

### 5. ¿Qué valor concreto aportará su solución al usuario?

Tres valores medibles:

1. **Integridad verificable:** si un dato cambia, la verificación señala la primera acta alterada (RF3).
2. **Priorización objetiva:** sobre 2.000 mesas simuladas, el detector encuentra el **95,3 %** de las actas
   manipuladas con solo **2,8 %** de falsos positivos; el auditor revisa una lista ordenada, no miles de actas
   al azar (RF5, RF7).
3. **Trabajo accionable y trazable:** reporte CSV con puntaje y motivo (RF6) y bitácora de quién hizo qué
   (RF8).

**Evidencia:** [`evidencias/reporte-verificacion.md`](evidencias/reporte-verificacion.md); `AuditoriaService`;
`ReporteCsv`; pantalla “Auditar”.

### 6. ¿Pueden explicar claramente por qué su solución es una respuesta adecuada al problema y no simplemente una aplicación interesante?

Porque ataca las **dos causas** del problema, no una: la alteración posterior (cadena de hashes) y los datos
anómalos (z-score robusto). Además agrega validaciones para duplicados e inconsistencias, cómputo por niveles
y un reporte accionable. Y porque **no promete lo que no puede**: no prueba fraude, no reemplaza al órgano
electoral, no usa datos personales y no decide por el auditor. Su salida es una lista de indicios para
revisión humana; eso es exactamente lo que un observador necesita para sustentar una solicitud de revisión.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.7; aviso visible en la UI (“Los
resultados son indicios estadísticos, no prueba de fraude”).

---

# 2. Objetivos y éxito

### 7. ¿El objetivo general describe un resultado concreto que puede comprobarse?

Sí. El objetivo es: prototipo de escritorio que registre actas simuladas, compute por nivel geográfico y
detecte actas anómalas, logrando sobre **2.000 mesas** con fraude inyectado **recall ≥ 80 %**, **falsos
positivos ≤ 10 %** y **10.000 actas auditadas en < 10 s**. Cada número se comprueba con una prueba o con el
reporte de evidencias. Resultado actual: **95,3 % / 2,8 % / ≈34 ms**.

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.1; [`06-pruebas.md`](06-pruebas.md) §6.5;
`AuditoriaTests`, `RendimientoTests`.

### 8. ¿Los objetivos específicos permiten dividir y comprobar el avance hacia ese resultado?

Sí. Hay seis objetivos específicos (OE1–OE6), cada uno con un indicador verificable:

| OE | Indicador | Estado |
|---|---|---|
| OE1 datos reproducibles | misma semilla ⇒ mismos hashes | Cumple |
| OE2 registro validado | rechazos con fila y motivo | Cumple |
| OE3 integridad | alterar rompe la cadena | Cumple |
| OE4 cómputo | sumas por nivel = nacional | Cumple |
| OE5 detección | recall/FP sobre 2.000 mesas | Cumple |
| OE6 reporte | CSV con puntaje y motivo | Cumple |

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.2; pruebas por archivo en
[`06-pruebas.md`](06-pruebas.md) §6.3.

### 9. ¿Existe una relación clara entre problema → objetivos → solución → resultados esperados?

Sí, y está trazada en una matriz. Ejemplo: el problema “alteración invisible” → OE3 → RF3 (cadena SHA-256) →
T9 → prueba `AlterarUnVoto_RompeLaCadenaEnEsaActa` → reporte de evidencias. Lo mismo ocurre con datos
anómalos (OE5/RF5/T11), duplicados (OE2/RF2/T7), cómputo (OE4/RF4/T10) y reporte (OE6/RF6/T13).

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.9 y [`11-matriz-trazabilidad.md`](11-matriz-trazabilidad.md).

### 10. ¿Cómo sabrán al final del proyecto que realmente tuvieron éxito?

Con cuatro condiciones verificables:

1. **Criterios de aceptación del MVP al 100 %** (lista marcada en [`06-pruebas.md`](06-pruebas.md) §6.6).
2. **Métricas alcanzadas:** recall 95,3 % (meta ≥ 80 %), FP 2,8 % (meta ≤ 10 %), 10.000 actas en ≈34 ms.
3. **Demo ejecutable desde cero** con el README en < 15 minutos y el flujo completo en < 5 minutos.
4. **Evidencia reproducible:** `dotnet test` (47 pruebas) y `dotnet run --project src/AuditaActas.Evidencia`
   generan el reporte que el docente puede revisar.

**Evidencia:** [`06-pruebas.md`](06-pruebas.md); [`10-guion-demo.md`](10-guion-demo.md);
[`evidencias/reporte-verificacion.md`](evidencias/reporte-verificacion.md).

---

# 3. Usuarios y requisitos

### 11. ¿Los usuarios y sus necesidades están suficientemente definidos para determinar qué debe hacer el sistema?

Sí. Hay tres roles con necesidades y permisos distintos, diez historias de usuario y criterios de aceptación
por funcionalidad. El Operador necesita cargar sin errores; el Auditor necesita verificar, computar, priorizar
y exportar; el Administrador necesita controlar acceso y auditar acciones. Cada necesidad se mapea a un
requisito funcional.

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.3–2.4; `Permisos` en `Seguridad.cs`.

### 12. ¿Las funcionalidades principales están directamente relacionadas con necesidades reales de los usuarios?

Sí. La tabla de mapeo es explícita: cargar actas (RF1/RF2) ← Operador; verificar integridad (RF3) ← Auditor;
computar (RF4) ← Auditor/Tribunal; priorizar (RF5/RF7) ← Auditor; exportar (RF6) ← Auditor; roles y bitácora
(RF8) ← Administrador. No hay funcionalidad “de adorno” en el MVP.

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.3 y §2.5.

### 13. ¿Las funcionalidades principales están suficientemente descritas para que otro integrante pueda comenzar a desarrollarlas sin tener que adivinar qué deben hacer?

Sí. Cada RF tiene descripción, prioridad, criterios **Dado/Cuando/Entonces**, evidencia esperada y archivos
involucrados. Además existen pruebas que sirven de contrato ejecutable: otro integrante puede leer
`AuditoriaTests` y saber exactamente qué debe cumplir el detector. La arquitectura indica en qué capa va cada
cosa.

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.5; [`03-arquitectura.md`](03-arquitectura.md) §3.2;
código y pruebas.

### 14. ¿Cada funcionalidad principal tiene criterios de aceptación verificables?

Sí. RF1–RF8 tienen criterios verificables y cada uno tiene al menos una prueba o artefacto:

| RF | Verificación |
|---|---|
| RF1 | `GeneradorTests` (3) |
| RF2 | `ActaTests`, `RegistroTests`, `InfraestructuraTests` |
| RF3 | `CadenaHashTests` (6) |
| RF4 | `ComputoTests` (2) |
| RF5 | `AuditoriaTests` (5) + evidencias |
| RF6 | `ReporteCsv` + CSV de evidencias |
| RF7 | `EvaluacionTests` (3) + pantalla |
| RF8 | `SeguridadTests` (3), `BitacoraTests` (4) |

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.5; [`06-pruebas.md`](06-pruebas.md) §6.3 y §6.6.

### 15. ¿Existen requisitos de calidad además de las funcionalidades, como seguridad, rendimiento, disponibilidad, facilidad de uso o compatibilidad, cuando sean relevantes?

Sí. Hay diez RNF con métrica y forma de verificación: integridad (inmutabilidad + hash), rendimiento
(10.000 actas < 10 s), usabilidad (demo < 5 min), reproducibilidad (semilla), compatibilidad (Windows 10/11 +
.NET 8), privacidad (sin datos personales), mantenibilidad (capas + 47 pruebas + 0 advertencias),
explicabilidad (motivo por alerta), disponibilidad local (sin servidor) y seguridad de acceso (roles +
bitácora). También se declaran los límites (sin cifrado ni autenticación real).

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.6; [`03-arquitectura.md`](03-arquitectura.md) §3.9.

---

# 4. Alcance

### 16. ¿Está claramente definido qué desarrollará el equipo?

Sí. El alcance “sí” incluye: simulador, registro/importación con validaciones, cadena de hashes, cómputo por
niveles, auditoría estadística, reporte CSV, evaluación del detector, roles/bitácora, persistencia SQLite,
pruebas, evidencias y documentación. Está enumerado por requisito y por tarea.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.8; [`07-backlog.md`](07-backlog.md).

### 17. ¿Está claramente definido qué NO desarrollará?

Sí. **No** se desarrolla: sistema de votación, reemplazo de Sirepre/SCORC, OCR de imágenes, datos personales o
reales, blockchain, web/móvil, integración con sistemas oficiales, conclusiones legales de fraude. Está en la
sección “No se desarrolla” y se repite en la defensa para evitar expectativas incorrectas.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.8.

### 18. ¿Han identificado funcionalidades que podrían parecer interesantes pero que no son necesarias para cumplir el objetivo?

Sí: mapa de calor, ley de Benford, machine learning, PDF con diseño, datos reales del OEP, gráfico de
dispersión y chi-cuadrado. Se analizó por qué no aportan al objetivo o cuestan demasiado para el tiempo
disponible; están en la tabla de “tentaciones descartadas”. Si sobra tiempo, T18 (dispersión) es la única
candidata, y no bloquea el MVP.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.8; [`07-backlog.md`](07-backlog.md) §7.7.

### 19. ¿El alcance es realista considerando el tiempo, conocimientos y recursos disponibles?

Sí. Son 153 h planificadas para 3 personas en 6 semanas (~8,5 h/semana por persona), con tareas de 3–10 h y
suplentes. El MVP (109 h) ya está implementado y verificado: build sin advertencias, 47 pruebas, métricas por
encima de la meta. Las tecnologías son las del curso (C#, .NET, WinForms, SQLite, xUnit) y no hay servidores ni
permisos externos.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.3; `dotnet build`/`dotnet test`; evidencias.

### 20. Si tuvieran que reducir el proyecto un 40 %, ¿qué conservarían y qué eliminarían?

**Conservaría** el flujo que resuelve el problema: RF1 (datos), RF2 (carga/validación), RF3 (integridad), RF4
(cómputo), RF5 (z-score) y RF6 (reporte). **Eliminaría**, en orden: RF8 (roles/bitácora), pantalla RF7,
gráfico de dispersión/chi-cuadrado y, en el extremo, SQLite (trabajando en memoria + CSV). **No** eliminaría la
cadena de hashes ni el cómputo: sin ellos el proyecto pierde su razón de ser.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.10; [`04-plan-gantt.md`](04-plan-gantt.md) §4.6.

---

# 5. MVP

### 21. ¿Existe una primera versión funcional claramente definida?

Sí. El MVP es **RF1 + RF2 + RF3 + RF4 + RF5 + RF6 + persistencia SQLite + flujo de UI + pruebas +
documentación**. Permite generar o importar actas, verificar integridad, computar por niveles, auditar,
ver sospechosas y exportar el reporte. Está implementado y se puede ejecutar hoy.

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.7; `dotnet run --project src/AuditaActas.UI`.

### 22. ¿El MVP resuelve una parte real del problema y entrega valor al usuario?

Sí. Entrega dos cosas que el usuario no tenía: **verificación de integridad** (si un acta cambió, se sabe
cuál) y **priorización** (lista ordenada con motivo). Eso reduce el universo de revisión y permite sustentar
una solicitud con evidencia. No resuelve todo el problema electoral, pero resuelve la parte que el usuario
puede usar mañana.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.6–1.7; flujo de demo.

### 23. ¿El MVP puede estar terminado dentro del tiempo disponible?

Sí, ya está terminado y verificado antes de la fecha proyectada. Las tareas T1–T15 (109 h) están completas,
con build en verde y 47 pruebas. Lo que queda (T18 gráfico, T21 ensayo) es post-MVP y no bloquea la entrega.
Si el docente confirma una fecha distinta, el MVP se puede entregar tal como está.

**Evidencia:** [`07-backlog.md`](07-backlog.md); `dotnet test`; evidencias.

### 24. ¿Las funcionalidades posteriores están claramente separadas del MVP?

Sí. El MVP termina en RF6 + persistencia + UI. RF7 (evaluación) y RF8 (roles/bitácora) estaban planificados
para después y se adelantaron porque el núcleo ya era estable; se documentan como **extensión temprana**, no
como condición del MVP. El gráfico de dispersión, chi-cuadrado, PDF, OCR y autenticación real quedan
explícitamente fuera. Esta separación permite recortar sin tocar el valor central.

**Evidencia:** [`02-requisitos.md`](02-requisitos.md) §2.7; [`04-plan-gantt.md`](04-plan-gantt.md) §4.6;
[`07-backlog.md`](07-backlog.md) §7.7.

---

# 6. Diseño y viabilidad técnica

### 25. ¿La solución propuesta es técnicamente viable con las tecnologías, conocimientos y recursos que tiene el equipo?

Sí. El equipo usa C#/.NET, WinForms, SQLite y xUnit, tecnologías del curso y conocidas por los tres. No hay
servidores, APIs externas, credenciales ni datos personales. Los puntos inciertos se resolvieron con spikes y
pruebas: generador/z-score, hash con 10.000 registros, SQLite ida y vuelta, CSV con comas, rendimiento y UI.
El build y las 47 pruebas lo demuestran.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.7; evidencias; `dotnet build`/`dotnet test`.

### 26. ¿Han identificado las partes técnicamente más difíciles o inciertas del proyecto?

Sí, seis: (1) realismo del simulador, (2) calibración del umbral, (3) grupos pequeños donde mediana/MAD no es
representativa, (4) orden de la cadena en importación masiva, (5) gráficos en WinForms y (6) mantener la UI
sin lógica. Cada una tiene mitigación: dos tipos de fraude y semilla; barrido de umbral; respaldo
municipio→departamento→nacional; encadenar en orden y conservar `orden` en SQLite; postergar el gráfico;
servicios en Aplicacion y pruebas sin UI.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.6; `AuditoriaService` (respaldo);
`ActaRepositorio` (orden); `RendimientoTests`.

### 27. ¿Han validado, o tienen previsto validar antes de depender de ellas, las tecnologías o integraciones críticas?

Sí, las críticas ya están validadas: SQLite (prueba de 500 actas ida y vuelta), hash (10.000 registros),
CSV (comas/BOM), rendimiento (≈34 ms para 10.000), UI (compila y ejecuta en Windows). **Pendiente:** ejecutar
en el equipo del docente (la UI ya se validó en un Windows limpio el 21/11/2026, ver `evidencias/validacion-ui-windows.md`) y el gráfico ScottPlot quedó como no comprometido (T18); ninguna de las dos bloquea el
MVP. No hay integraciones externas que validar.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.7; `InfraestructuraTests`; evidencias §3 y §5.

### 28. ¿Las principales decisiones tecnológicas tienen una justificación relacionada con las necesidades del proyecto?

Sí. Hay diez decisiones documentadas con alternativa descartada y razón: C#/.NET 8 (lenguaje del curso),
WinForms (conocido, sin servidor), SQLite (sin instalación), z-score robusto (los outliers no deben enmascarar
el fraude), SHA-256 encadenado (detecta alteración y orden), xUnit (estándar y CI), CSV (intercambio simple),
UI sin diseñador (PR revisables), evaluación con etiquetas del simulador (no hay datos reales) y roles simples
(prototipo local). Cada una responde a una necesidad, no a una moda.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.5.

### 29. ¿Han identificado las dependencias externas necesarias para construir y ejecutar el sistema?

Sí: .NET 8 SDK, Windows 10/11 para la UI, paquetes NuGet `Microsoft.Data.Sqlite` 8.0.10
5.0.0 (reservado), `xUnit` 2.9.2 y runner 2.8.2. La primera restauración requiere internet; después funciona
offline. No hay APIs, credenciales, servidores ni permisos. La consola de evidencias corre en cualquier SO con
.NET 8.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.8; archivos `.csproj`; `dotnet restore`.

### 30. ¿La solución tiene una estructura técnica inicial suficientemente clara para comenzar el desarrollo?

Sí. La solución tiene seis proyectos con dependencias en una sola dirección (UI → Aplicacion/Infraestructura →
Dominio), un proyecto de pruebas y uno de evidencia. Cada clase tiene una responsabilidad nombrada; el esquema
SQLite está congelado; los flujos A–E están documentados. Un integrante nuevo sabe dónde va cada cambio.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.1–3.4; `AuditaActas.sln`; estructura del repo.

---

# 7. Datos, seguridad y calidad

### 31. ¿Han identificado qué información manejará el sistema y de dónde provendrá?

Sí. Maneja actas electorales **sintéticas**: departamento, municipio, recinto, mesa, habilitados, emitidos,
blancos, nulos, votos por candidato, etiqueta de fraude simulado y hashes. Provienen de `GeneradorService`
(semilla), de CSV de ejemplo o de una carga manual. Se guardan en SQLite local. **No** se usan datos
personales, padrón, imágenes ni datos reales del OEP. El CSV de errores y el de ejemplo están en `datos/` y
documentados.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.4; `datos/README.md`; `Acta.cs`.

### 32. ¿Han identificado restricciones importantes relacionadas con datos, privacidad, seguridad, permisos o acceso?

Sí. Restricciones: solo datos sintéticos; sin datos personales; actas de solo inserción (no editar/borrar);
`clave UNIQUE`; roles con permisos; bitácora con usuario/hora; base local sin cifrar; sin autenticación real;
la cadena detecta alteraciones parciales pero no a un atacante con control total del archivo. Estas
limitaciones se declaran en la UI, el README y la arquitectura; para producción se necesitaría firma digital,
anclaje externo, cifrado y autenticación.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.9; `Permisos`; `ActaRepositorio` (sin UPDATE/DELETE).

### 33. ¿Han definido cómo comprobarán que las funcionalidades principales funcionan correctamente?

Sí, con tres capas: (1) pruebas unitarias/integración para cada RF (47 pruebas); (2) evidencia reproducible
con métricas y tiempos (`src/AuditaActas.Evidencia`); (3) lista de aceptación manual del MVP y guion de demo.
Cada criterio de aceptación tiene una prueba o un artefacto asociado. `dotnet test` y el reporte se ejecutan
en CI.

**Evidencia:** [`06-pruebas.md`](06-pruebas.md); [`evidencias/reporte-verificacion.md`](evidencias/reporte-verificacion.md);
`.github/workflows/ci.yml`.

### 34. ¿Existe una estrategia inicial de pruebas que acompañará el desarrollo?

Sí. Pirámide: muchas unitarias (validaciones, hash, estadística, cómputo), integración (CSV→SQLite→cadena),
aceptación (métricas), rendimiento y manual/UI. La regla es “se escribe la prueba junto con la funcionalidad”,
`main` siempre verde y PR con build/test. Hay 47 pruebas y CI en Windows. Lo no cubierto (UI, datos reales,
seguridad) se declara con su mitigación.

**Evidencia:** [`06-pruebas.md`](06-pruebas.md); `tests/AuditaActas.Tests`; CI.

---

# 8. Plan de desarrollo

### 35. ¿El Gantt está compuesto por tareas concretas que realmente habrá que ejecutar?

Sí. Son 21 tareas con entregable verificable: repositorio, spikes, esquema, generador, repositorio/validaciones,
UI, hash, cómputo, auditoría, pantallas, reporte, integración, correcciones, evaluación, roles/bitácora,
gráfico, calibración, documentación y ensayo. No hay tareas genéricas tipo “programar” o “probar”; cada una
tiene un resultado que se puede abrir.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.3; [`07-backlog.md`](07-backlog.md).

### 36. ¿Cada tarea tiene responsable?

Sí. Cada fila del Gantt tiene un responsable (Alex, Nelson o Cristian) y los trabajos compartidos (integración,
correcciones, ensayo) dicen “Todos”. Además hay suplentes definidos (A↔B, B↔C, C↔A) para que ninguna tarea
quede sin dueño si alguien falta.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.2–4.3.

### 37. ¿Las tareas tienen estimaciones razonables?

Sí. Las estimaciones suman 153 h (109 h del MVP), con tareas de 3–10 h. Se basan en spikes previos y en el
tamaño real de los archivos/pruebas; el MVP ya está implementado, lo que confirma que el camino crítico no está
subestimado. Si una tarea se desvía > 30 %, se ajusta el alcance, no la fecha.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.3 (columna Horas y “de dónde salen”).

### 38. ¿Las dependencias entre tareas están identificadas?

Sí. El Gantt tiene columna “Depende de” y un diagrama del camino crítico: T5 → T7 → T9/T10 → T14 → T21, con
T2/T6/T11 y T8/T12/T13 en paralelo. Las dependencias externas (confirmación del docente, entorno) están
separadas y no bloquean el núcleo.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.5.

### 39. ¿El Gantt contempla desarrollo, integración, pruebas, correcciones y documentación?

Sí. Desarrollo en T6–T13; integración en T14; correcciones en T15; pruebas en todas las tareas y en T14;
documentación en T20; ensayo/entrega en T21. Además hay spikes (T2–T5) y calibración (T19). La suite y el
reporte de evidencias son parte de T14/T20.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.3; [`06-pruebas.md`](06-pruebas.md).

### 40. ¿Han identificado qué tareas podrían bloquear al resto del equipo?

Sí. **T5** (esquema/dominio) bloquea T6/T7; **T7** (repositorio/validaciones) bloquea T9–T13; **T14**
(integración) bloquea T16–T21. Por eso T5 se congela en la semana 1, T7 tiene prioridad y T14 tiene una ruta
de emergencia (datos en memoria, recorte de T18). También se identificó el riesgo de esperar la confirmación
del docente, que no bloquea porque el producto es autocontenido.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.5; [`05-riesgos.md`](05-riesgos.md) §5.2.

---

# 9. Equipo y riesgos

### 41. ¿Cada integrante tiene una responsabilidad concreta y verificable?

Sí:

| Integrante | Responsabilidad | Se verifica en |
|---|---|---|
| Alex Vásquez | Coordinación, arquitectura, integración, persistencia y hash | `ActaRepositorio`, `CadenaHashService`, `ComputoService`, `AuditaActas.sln` |
| Nelson Zapata | Datos, simulación, estadística y evaluación | `GeneradorService`, `Estadistica`, `AuditoriaService`, `EvaluacionService` |
| Cristian García | UI, reportes y calidad/pruebas | `FormPrincipal`, `FormActa`, `FormBitacora`, `ReporteCsv`, suite |

Cada módulo tiene pruebas y documentación, así que la responsabilidad no es una declaración oral: se puede
revisar en el repositorio.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.2; archivos y pruebas citados.

### 42. ¿El equipo tiene un mecanismo acordado para revisar e integrar el trabajo?

Sí. `main` siempre compila y pasa pruebas; nadie hace push directo. Una rama por funcionalidad
(`feature/rf3-hash`), commits pequeños en presente, PR con plantilla y aprobación de otro integrante, build y
test en verde antes de abrir el PR, CI en Windows y revisión cruzada. Hay reunión de 15 minutos dos veces por
semana y el backlog se actualiza.

**Evidencia:** `CONTRIBUTING.md`; `.github/pull_request_template.md`; `.github/workflows/ci.yml`.

### 43. ¿Cuáles son los tres riesgos que más podrían hacer fracasar o retrasar el proyecto?

1. **R1:** que el detector no alcance las métricas o genere demasiados falsos positivos.
2. **R2:** retrasos por poca disponibilidad o conflictos de integración (equipo pequeño, calendario corto).
3. **R3:** problemas de entorno o librerías (.NET, NuGet, Windows) que impidan ejecutar la demo.

**Evidencia:** [`05-riesgos.md`](05-riesgos.md) §5.1.

### 44. ¿Cada uno de esos riesgos tiene una estrategia de mitigación?

Sí:

- **R1:** dos tipos de fraude, z-score robusto, barrido de umbral, pruebas de aceptación. Contingencia: umbral
  3,5 (recall 81,1 %, FP 1,2 %) o análisis honesto si no se llega.
- **R2:** tareas pequeñas, PR frecuentes, `main` verde, suplentes. Contingencia: recorte del 40 % y prioridad
  al MVP.
- **R3:** versiones fijas, README, CI en Windows, evidencia en consola. Contingencia: binario o máquina propia
  en la defensa.

**Evidencia:** [`05-riesgos.md`](05-riesgos.md) §5.1–5.3.

### 45. ¿Qué harían si una persona clave del equipo deja de estar disponible?

Se activa la suplencia: Alex↔Nelson, Nelson↔Cristian, Cristian↔Alex. El suplente hereda código probado,
documentación y pruebas; no empieza de cero. Además se recorta alcance si es necesario (T18 y chi-cuadrado) y se mantiene
el MVP. La fuente de verdad es el repositorio: README, docs, pruebas y CI. En la defensa, cualquier integrante
puede explicar el módulo de otro porque hubo revisión cruzada.

**Evidencia:** [`05-riesgos.md`](05-riesgos.md) §5.4; [`04-plan-gantt.md`](04-plan-gantt.md) §4.2.

---

# 10. Preparación para desarrollar

### 46. ¿Existe actualmente alguna decisión fundamental que todavía no hayan tomado y que pueda bloquear el inicio del desarrollo?

No. Las decisiones fundamentales están tomadas y documentadas: lenguaje/framework (C#/.NET 8), UI (WinForms),
persistencia (SQLite), algoritmo (z-score robusto), integridad (SHA-256 encadenado), alcance (RF1–RF8), MVP
(RF1–RF6), datos (sintéticos), métricas (recall/FP/rendimiento) y equipo/suplentes. Lo único pendiente es
**externo y no bloqueante**: la fecha de entrega/defensa y el formato exacto que pida el docente. El producto
se puede entregar como repositorio + demo + documentación, y si el docente pide otro formato se adapta la
entrega, no el núcleo.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.5; [`04-plan-gantt.md`](04-plan-gantt.md) §4.1;
README, “Datos pendientes de confirmar”.

### 47. ¿Existe algún requisito importante que todavía no comprendan suficientemente?

No hay un requisito crítico sin comprender. Las posibles ambigüedades están resueltas con supuestos explícitos:
datos simulados, escritorio, sin datos personales, reporte CSV. La única ambigüedad real es si el docente
exigirá un formato distinto (por ejemplo, web o una base específica); se mitiga con una arquitectura portable
y un alcance declarado. Si exigiera datos reales, sería un **cambio de alcance** que requiere permisos,
privacidad y validación legal, y se conversaría antes de aceptarlo.

**Evidencia:** [`01-problema-y-alcance.md`](01-problema-y-alcance.md) §1.9; [`02-requisitos.md`](02-requisitos.md) §2.8.

### 48. ¿Existe alguna dependencia técnica, información, permiso, acceso, herramienta o recurso que todavía no tengan y que sea necesario para comenzar?

No para comenzar. Ya están: SDK .NET, Windows, Visual Studio/CLI, paquetes NuGet, datos sintéticos y
documentación. La primera restauración necesita internet; después funciona offline. **Pendiente no
bloqueante:** la UI ya corrió en un Windows limpio (capturas en `evidencias/validacion-ui-windows.md`); falta probar en el equipo del docente y confirmar si acepta escritorio. No se necesitan permisos
especiales porque no se usan datos reales ni sistemas oficiales. Si el equipo del docente no tiene Windows, la
consola de evidencias y el núcleo corren en otro SO; solo la UI WinForms requiere Windows.

**Evidencia:** [`03-arquitectura.md`](03-arquitectura.md) §3.8; README, “Cómo ejecutar”.

### 49. Si mañana tuvieran que comenzar a programar, ¿cada integrante sabría exactamente qué debe construir primero?

Sí. El orden está en el backlog y en el Gantt:

- **Alex:** T9/T10 ya hechos; si empezara de cero, T5 (esquema) → T7 (repositorio/validaciones) → T9 (hash).
- **Nelson:** T2 (spike generador/z-score) → T6 (RF1) → T11 (RF5) → T16 (RF7).
- **Cristian:** T8 (esqueleto UI) → T12 (pantallas) → T13 (reporte) → T18 (cerrada como no comprometida).

Hoy, con el MVP listo, las próximas tareas son T18 (opcional), T20 (docs, hecho) y T21 (ensayo). Cada una
tiene primer paso concreto en [`07-backlog.md`](07-backlog.md) §7.8.

**Evidencia:** [`04-plan-gantt.md`](04-plan-gantt.md) §4.3; [`07-backlog.md`](07-backlog.md) §7.8.

### 50. Si un nuevo integrante entrara mañana y solo revisara el repositorio, ¿podría comprender qué problema están resolviendo, qué van a construir, cuál es el MVP y qué trabajo debe realizarse a continuación?

Sí, en este orden de lectura (~45–60 min):

1. `README.md` (problema, estado, cómo ejecutar, estructura).
2. `docs/01-problema-y-alcance.md` (por qué y para quién).
3. `docs/02-requisitos.md` (qué, con criterios de aceptación y MVP).
4. `docs/03-arquitectura.md` (dónde va cada cosa).
5. `docs/04-plan-gantt.md` y `docs/07-backlog.md` (qué sigue y quién lo hace).
6. `docs/06-pruebas.md` y `docs/evidencias/reporte-verificacion.md` (cómo se comprueba).
7. `CONTRIBUTING.md` (cómo se trabaja) y el código/pruebas.

El repositorio incluye un proyecto de evidencia que se ejecuta con un comando, así que el nuevo integrante
puede verificar el estado sin preguntar.

**Evidencia:** README (índice de documentación); docs 01–11; `src/AuditaActas.Evidencia`.

---

# 11. Prueba de fracaso

### 51. ¿Qué tendría que ser cierto para que este proyecto fracase?

| Condición | ¿Es cierta hoy? | Comentario |
|---|---|---|
| El problema no es real o no le importa a nadie | No | Hay contexto documentado y usuarios definidos |
| El simulador es tan simple que el detector “funciona” sin demostrar nada | Parcial | Es un simulador; se mitiga con dos tipos de fraude, ruido y límites declarados |
| No se alcanzan las métricas y no hay análisis | No | 95,3 % / 2,8 % y barrido de umbral |
| El MVP no está a tiempo | No | MVP implementado y probado |
| El trabajo no se integra | No | Build/test/CI en verde |
| Alguien no puede ejecutar el proyecto | Riesgo | README, CI y evidencia en consola |
| Se interpreta como detector de fraude real | Riesgo de comunicación | Aviso visible y lenguaje de indicio |
| Se usan datos reales sin permiso | No | Solo datos sintéticos |
| El equipo desaparece | Riesgo de gestión | Suplentes, docs y pruebas |
| El docente exige algo incompatible con el alcance | Incierto | Supuestos y adaptación de entrega |

**Evidencia:** [`05-riesgos.md`](05-riesgos.md) §5.5.

### 52. ¿Ya tienen una estrategia para evitar o controlar cada una de esas condiciones?

Sí, una por una:

1. **Problema irreal** → evidencia de contexto y usuarios; no se inventa urgencia.
2. **Simulador trivial** → dos tipos de fraude, ruido, métricas honestas y limitaciones.
3. **Métricas insuficientes** → barrido de umbral y análisis; se puede cambiar el umbral sin tocar código.
4. **Retraso del MVP** → tareas pequeñas, integración temprana y recorte del 40 %.
5. **Falta de integración** → PR, CI, `main` verde y revisión cruzada.
6. **No se puede ejecutar** → README probado, CI Windows, consola de evidencias y binario de respaldo.
7. **Se malinterpreta como detector de fraude** → aviso en UI/README, lenguaje de “indicio”, sección de límites.
8. **Datos reales sin permiso** → prohibido por alcance; solo sintéticos.
9. **Equipo ausente** → suplentes, documentación y pruebas.
10. **Exigencia incompatible del docente** → supuestos explícitos, diálogo temprano y adaptación de la entrega.

**Evidencia:** [`05-riesgos.md`](05-riesgos.md) §5.6; [`10-guion-demo.md`](10-guion-demo.md) (preguntas
difíciles y respuestas).

---

# Apéndice: chuleta de 52 respuestas en una línea

Para repasar antes de la defensa. Si el docente pide detalle, abra la respuesta completa de la sección
correspondiente.

| # | Respuesta corta |
|---:|---|
| 1 | Miles de actas manuales; alteración, duplicados e inventos son lentos y subjetivos de detectar. |
| 2 | Observadores, delegados y tribunales; roles Operador, Auditor y Administrador. |
| 3 | OEA 2019, 35.253 actas en 2025 y prensa: contexto de trazabilidad, no prueba de fraude. |
| 4 | Revisión lenta/subjetiva, duplicados, desconfianza, impugnaciones y costo político. |
| 5 | Integridad verificable, priorización (95,3 % recall / 2,8 % FP) y reporte accionable. |
| 6 | Ataca las dos causas y no promete fraude: entrega indicios para revisión humana. |
| 7 | 2.000 mesas, recall ≥ 80 %, FP ≤ 10 %, 10.000 actas < 10 s; logrado 95,3/2,8/≈34 ms. |
| 8 | OE1–OE6, cada uno con indicador, prueba y estado. |
| 9 | Matriz problema → objetivo → RF → tarea → prueba → evidencia. |
| 10 | Aceptación 100 %, métricas, demo < 15 min y evidencia reproducible con un comando. |
| 11 | Tres roles, diez historias y criterios de aceptación por funcionalidad. |
| 12 | Cada RF responde a una necesidad concreta; no hay funciones de adorno. |
| 13 | Descripción + Dado/Cuando/Entonces + pruebas contrato + arquitectura por capas. |
| 14 | RF1–RF8 tienen al menos una prueba o artefacto verificable. |
| 15 | Diez RNF medibles (integridad, rendimiento, usabilidad, compatibilidad, privacidad, etc.) y límites. |
| 16 | RF1–RF8 + persistencia + UI + pruebas + documentación. |
| 17 | No votación, OCR, datos reales, web/móvil, blockchain ni conclusiones legales. |
| 18 | Mapa de calor, Benford, ML, PDF, datos reales, dispersión y chi-cuadrado. |
| 19 | 153 h, 3 personas, 6 semanas; MVP ya implementado; stack del curso. |
| 20 | Conserva RF1–RF6; elimina RF8, pantalla RF7, gráfico/chi² y, al extremo, SQLite. |
| 21 | MVP = RF1–RF6 + SQLite + UI + pruebas + docs; ejecutable hoy. |
| 22 | Verificación + priorización + reporte: valor real para el auditor. |
| 23 | Ya está terminado; T18/T21 son post-MVP y no bloquean. |
| 24 | MVP RF1–RF6; RF7/RF8 extensión temprana; gráfico/chi² futuro. |
| 25 | C#/.NET, WinForms, SQLite y xUnit conocidos; sin servidores; spikes y 47 pruebas. |
| 26 | Realismo del simulador, umbral, grupos pequeños, orden del hash, gráficos y UI sin lógica. |
| 27 | SQLite, hash, CSV, rendimiento y UI validados; falta probar el equipo del docente. |
| 28 | Diez decisiones con alternativa descartada y justificación. |
| 29 | .NET 8, Windows para UI, paquetes NuGet; sin APIs ni credenciales. |
| 30 | Seis proyectos, dependencias en una dirección, esquema y flujos documentados. |
| 31 | Actas sintéticas (generador/CSV/manual) en SQLite; sin datos personales. |
| 32 | Solo sintéticos, append-only, clave única, roles y bitácora; sin cifrado ni auth real. |
| 33 | 47 pruebas + reporte de evidencias + aceptación manual + CI. |
| 34 | Unitarias, integración, aceptación, rendimiento y manual/UI; límites declarados. |
| 35 | 21 tareas con entregable concreto, no tareas genéricas. |
| 36 | Responsable por tarea y suplentes definidos. |
| 37 | 153 h (109 del MVP), tareas de 3–10 h basadas en spikes. |
| 38 | Columna “Depende de” y camino crítico T5→T7→T9/T10→T14→T21. |
| 39 | Desarrollo, integración, pruebas, correcciones, documentación y ensayo. |
| 40 | T5, T7 y T14; ruta de emergencia y recorte. |
| 41 | Alex: arquitectura/persistencia/hash; Nelson: datos/estadística; Cristian: UI/reportes/pruebas. |
| 42 | `main` verde, ramas, PR, revisión cruzada, CI y reuniones. |
| 43 | R1 métricas, R2 tiempo/integración, R3 entorno. |
| 44 | R1 barrido/umbral; R2 recorte/PR; R3 versiones/CI/plan B. |
| 45 | Suplentes A↔B, B↔C, C↔A; docs y pruebas; recorte si hace falta. |
| 46 | No hay decisión bloqueante; solo fecha/formato del docente, que no frena el núcleo. |
| 47 | No crítico; supuestos claros; datos reales sería un cambio de alcance. |
| 48 | No; SDK/Windows/NuGet listos; UI validada en Windows limpio (21/11); falta el equipo del docente. |
| 49 | Alex T5→T7→T9; Nelson T2→T6→T11→T16; Cristian T8→T12→T13→T18. |
| 50 | Sí; README → 01 → 02 → 03 → 04/07 → 06 y evidencias → CONTRIBUTING. |
| 51 | Problema irreal, simulador trivial, métricas no alcanzadas, retraso, no integración, no ejecutable, mala interpretación, datos reales, equipo ausente, requisito incompatible. |
| 52 | Cada condición tiene control: evidencia, dos fraudes, umbral, recorte, CI, README/consola, aviso, sintéticos, suplentes y diálogo. |

---

## Versiones para entregar

El cuestionario también está en
[`08-cuestionario-aprobacion.docx`](08-cuestionario-aprobacion.docx) y
[`08-cuestionario-aprobacion.pdf`](08-cuestionario-aprobacion.pdf), generados desde este Markdown.
Para regenerar el DOCX (requiere `pip install python-docx`):

```bash
python scripts/exportar-cuestionario.py
```

