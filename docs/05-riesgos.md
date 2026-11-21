# 5. Riesgos y prueba de fracaso

## 5.1 Los tres riesgos que más podrían hacer fracasar el proyecto

| # | Riesgo | Prob. | Impacto | ¿Por qué es grave? | Mitigación | Plan de contingencia | Disparador (cuándo actuar) | Responsable |
|---|---|---|---|---|---|---|---|---|
| **R1** | El detector da demasiados falsos positivos o no alcanza las metas | Media | Alto | Sin métricas creíbles, el proyecto no demuestra valor; una lista llena de falsos positivos no se usa | Dataset con dos tipos de fraude, z-score robusto, barrido de umbral, pruebas de aceptación | Bajar el umbral a 3,5 (recall 81,1 %, FP 1,2 %) o documentar honestamente el resultado y el análisis; nunca ocultar métricas | Recall < 80 % o FP > 10 % en la semilla 42 | Nelson |
| **R2** | Retrasos por poca disponibilidad o conflictos de integración | Alta | Alto | El equipo es pequeño y el calendario es corto; una ausencia frena el camino crítico | Tareas de 3–10 h, PR frecuentes, `main` verde, suplentes, recorte del 40 % | Congelar alcance, priorizar MVP, eliminar T18, chi-cuadrado y roles si es necesario | Una tarea del camino crítico se atrasa > 3 días | Alex |
| **R3** | Problemas de entorno o librerías (.NET, paquetes, Windows) | Baja | Medio | Si el docente no puede ejecutar, la mejor demo no sirve | Versiones fijadas, `dotnet restore/build/test` documentado, CI en Windows, evidencia en consola | Publicar binario o ejecutar la consola de evidencias; llevar una máquina propia a la defensa | Falla `dotnet restore` o la UI no abre en otra máquina | Cristian |

## 5.2 Registro completo de riesgos

| ID | Riesgo | Categoría | Prob. | Impacto | Mitigación / control | Contingencia |
|---|---|---|---|---|---|---|
| R1 | Métricas de detección insuficientes | Técnico | Media | Alto | Barrido de umbral, dos tipos de fraude, pruebas | Ajustar umbral y documentar; meta revisable con análisis |
| R2 | Falta de tiempo / integrantes | Gestión | Alta | Alto | Plan por tareas, suplentes, PR, recorte | Entregar MVP recortado y evidencia |
| R3 | Entorno/librerías | Técnico | Baja | Medio | Versiones fijas, README, CI | Binario/consola, máquina propia |
| R4 | El simulador es demasiado simple y el detector “funciona” sin demostrar nada | Calidad | Media | Alto | Dos tipos de fraude, ruido, etiquetas, limitaciones declaradas | Agregar escenarios y documentar que es un prototipo |
| R5 | El proyecto se interpreta como “detector de fraude real” | Ético/comunicación | Media | Alto | Aviso visible en UI/README; lenguaje de “indicio”; alcance explícito | Aclarar en la defensa y en el reporte; no usar datos reales |
| R6 | Se pierde la base o se altera el archivo | Datos | Baja | Medio | Actas de solo inserción, hash, bitácora; el usuario guarda una copia | Regenerar datos (semilla) y comparar hashes |
| R7 | La UI se vuelve un monolito con lógica | Mantenibilidad | Media | Medio | Servicios en Aplicacion, revisión de PR, pruebas sin UI | Refactorizar antes de agregar funciones |
| R8 | Scope creep (mapa de calor, ML, PDF, web) | Gestión | Alta | Medio | Lista de “tentaciones descartadas”; recorte del 40 % | Decir no y registrar la idea en el backlog futuro |
| R9 | Dependencia de una persona clave | Equipo | Media | Alto | Suplentes A↔B, B↔C, C↔A; código y docs | El suplente continúa con el módulo y la suite de pruebas |
| R10 | El docente exige un formato no previsto (web, base específica, datos reales) | Externo | Media | Medio | Producto autocontenido y exportable; supuestos documentados | Adaptar la entrega, no el núcleo; acordar alcance |
| R11 | Falsos negativos en actas con fraude sutil | Técnico | Media | Medio | Barrido de umbral y reporte de limitaciones | Bajar umbral (más alertas) o agregar otra prueba estadística |
| R12 | La cadena no protege contra un atacante con control total del archivo | Seguridad | Baja | Medio | Se declara como límite; hash + bitácora | Para producción: firma digital y anclaje externo |

## 5.3 Riesgo R1 en detalle (el más importante)

**Qué puede fallar:** que el z-score no separe el fraude inyectado del ruido normal, o que el umbral genere
demasiadas alertas.

**Evidencia actual (semilla 42, 2.000 mesas, 5 % fraude):**

| Métrica | Valor | Meta |
|---|---:|---:|
| Recall | 95,3 % | ≥ 80 % |
| Falsos positivos | 2,8 % | ≤ 10 % |
| Precisión | 65,6 % | Informativa |
| Alertas | 154 de 2.000 | — |

**Barrido de umbral:**

| Umbral | Recall | FP | Precisión |
|---:|---:|---:|---:|
| 2,0 | 100,0 % | 21,1 % | 21,0 % |
| 2,5 | 99,1 % | 7,9 % | 41,3 % |
| **3,0** | **95,3 %** | **2,8 %** | **65,6 %** |
| 3,5 | 81,1 % | 1,2 % | 79,6 % |
| 4,0 | 78,3 % | 0,4 % | 91,2 % |

**Decisión:** umbral 3,0 por equilibrio recall/FP. Si el docente prioriza precisión, se puede cambiar a 3,5
sin tocar código (parámetro `umbral`). El análisis está en el reporte de evidencias.

**Limitación honesta:** el detector se evalúa con el mismo simulador que genera el fraude; eso mide
consistencia interna, no validez contra datos reales. Se declara en la defensa.

## 5.4 Si una persona clave deja de estar disponible

| Si falta… | Suplente | Qué debe saber | Dónde está escrito |
|---|---|---|---|
| Alex (arquitectura, persistencia, hash) | Nelson | Esquema, cadena, integración, comandos | Docs 03, 06, 09; `ActaRepositorio`, `CadenaHashService` |
| Nelson (datos, estadística) | Alex | Generador, z-score, calibración, métricas | Docs 02 §RF1/RF5, 06; `GeneradorService`, `AuditoriaService` |
| Cristian (UI, reportes, pruebas) | Nelson | Formularios, CSV, suite, CI | Docs 06, 09, 10; `FormPrincipal`, `ReporteCsv` |

**Mecanismo:** PR revisado por otro integrante, `main` siempre verde, documentación en el repositorio,
reunión de 15 minutos dos veces por semana y backlog actualizado. El suplente no empieza de cero: hereda
código probado y documentación.

## 5.5 Prueba de fracaso: ¿qué tendría que ser cierto para que el proyecto fracase?

| # | Condición de fracaso | ¿Es cierto hoy? | Control / estrategia |
|---|---|---|---|
| 1 | El problema no es real o no le importa a nadie | No | Evidencia de contexto (OEA, TSE, prensa) y usuarios definidos |
| 2 | El simulador es tan simple que el detector “funciona” sin demostrar nada | Parcial (es un simulador) | Dos tipos de fraude, ruido, métricas honestas y limitaciones declaradas |
| 3 | No se alcanzan las métricas y no hay análisis | No (95,3 % / 2,8 %) | Barrido de umbral y análisis documentado |
| 4 | El MVP no está a tiempo | No (MVP implementado) | Recorte del 40 %, tareas pequeñas, integración temprana |
| 5 | El trabajo no se integra | No (build/test en verde) | PR, CI, `main` verde, integración desde la semana 2 |
| 6 | Alguien no puede ejecutar el proyecto | Riesgo | README probado, CI Windows, evidencia en consola |
| 7 | Se interpreta como detector de fraude real | Riesgo de comunicación | Aviso visible, lenguaje de indicio, sección de límites |
| 8 | Se usan datos reales sin permiso | No | Datos sintéticos por diseño; prohibido en el alcance |
| 9 | El equipo desaparece | Riesgo de gestión | Suplentes, docs, pruebas, recorte de alcance |
| 10 | El docente exige algo incompatible con el alcance | Incierto | Supuestos explícitos; adaptar entrega, no núcleo |

## 5.6 Estrategia general anti-fracaso

1. **Reducir incertidumbre primero:** spikes de generador, hash, SQLite y UI antes de comprometer el resto.
2. **Medir, no opinar:** cada afirmación importante tiene una prueba o un reporte reproducible.
3. **Integrar temprano:** el flujo completo funciona desde la semana 2; no se deja todo para el final.
4. **Recortar sin drama:** si el tiempo aprieta, se elimina lo que no cambia la decisión del auditor.
5. **Comunicar límites:** el prototipo prioriza revisión humana; no prueba fraude. Eso protege al equipo y al
   usuario.
6. **Documentar para el suplente:** si alguien falta, el repositorio sigue siendo la fuente de verdad.
