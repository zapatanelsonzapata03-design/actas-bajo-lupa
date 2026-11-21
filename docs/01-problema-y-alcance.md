# 1. Problema y alcance

## 1.1 El problema en 30 segundos (sin mencionar tecnología)

El resultado de una elección se construye sumando miles de actas de mesa llenadas a mano. Si un acta se
altera después de cerrarse, se duplica o se llena con números inventados, hoy detectarlo depende de que
alguien lo note revisando papeles. El problema no es contar votos: es **demostrar que lo contado es lo que
realmente decían las actas** y **saber cuáles revisar primero** cuando hay miles. La revisión manual es
lenta, cara y subjetiva; el resultado es desconfianza y conflictos que no se resuelven con más discursos,
sino con evidencia verificable.

## 1.2 Formulación del problema

> Los observadores, delegados y tribunales electorales no tienen una forma objetiva, rápida y verificable de
> (a) comprobar que un acta registrada no cambió después de su cierre y (b) priorizar qué actas revisar
> entre miles, con un criterio estadístico reproducible y sin afirmar fraude sin pruebas.

## 1.3 Quién tiene el problema y quién usa la solución

| Interesado | Necesidad | Relación con el prototipo |
|---|---|---|
| Observadores electorales | Detectar actas que merecen revisión y sustentar la solicitud | Usuario **Auditor/Analista** |
| Delegados de partido | Verificar integridad de las actas de su circunscripción | Usuario **Auditor/Analista** |
| Tribunal electoral | Priorizar recursos de revisión y auditar el conteo | Usuario **Auditor/Analista** |
| Personal de carga | Registrar/importar actas sin alterar las existentes | Usuario **Operador de carga** |
| Responsable del sistema | Controlar accesos y revisar qué se hizo | Usuario **Administrador** |
| Ciudadanía | Entender por qué se revisa un acta y no otra | Beneficiario indirecto (reporte) |

**Usuarios del prototipo:** tres roles con permisos distintos (RF8). El prototipo **no** es una herramienta
de votación ni un sistema oficial; es un apoyo a la revisión humana.

## 1.4 Evidencia de que el problema es real o relevante

No se afirma que haya habido fraude. La evidencia es de **contexto y de necesidad de trazabilidad**:

1. **Auditoría de la OEA a las elecciones de 2019.** El informe final documenta la interrupción del TREP al
   83,85 % del conteo, problemas de cadena de custodia y que más del 5 % de las imágenes de actas del TREP
   pasaron directo al cómputo oficial. Es un antecedente sobre integridad y trazabilidad, no una prueba de
   fraude en procesos posteriores.
   Fuente: [OAS, Final Report of the Audit of the Elections in Bolivia](https://www.oas.org/fpdb/press/Audit-Report-EN-vFINAL.pdf).
2. **Complejidad operativa del cómputo oficial.** El TSE informó que SCORC procesó el **100 % de 35.253
   actas** en la segunda vuelta de 2025; el volumen es real y cualquier revisión manual compite con plazos
   cortos. Fuente: [OEP, proclamación de resultados](https://www.oep.org.bo/institucional-institucional/tse-proclama-resultados-oficiales-de-la-segunda-vuelta-electoral/).
3. **Cobertura informativa del avance del cómputo.** La prensa reportó avances parciales y cierres por
   departamento (por ejemplo, 68,73 % y el cierre en Oruro, Pando y Tarija), lo que muestra la necesidad de
   leer resultados parciales con cuidado. Fuente: [El Deber](https://eldeber.com.bo/pais/tres-departamentos-concluyeron-el-computo-oficial-de-votos_527004/).
4. **Necesidad académica y profesional.** El curso de Programación II exige construir un sistema completo con
   requisitos, arquitectura, pruebas y planificación; este proyecto ejercita esas competencias sobre un
   dominio con impacto social.

> **Advertencia metodológica:** las fuentes anteriores contextualizan el problema; **no** validan el detector
> de este prototipo ni demuestran irregularidades en 2025. El prototipo usa datos sintéticos y solo prioriza
> revisión humana.

## 1.5 Consecuencias actuales del problema

| Consecuencia | Efecto concreto |
|---|---|
| Revisión manual lenta | Miles de actas y pocos revisores; se revisa por muestreo informal o por denuncia. |
| Subjetividad | Dos revisores pueden priorizar actas distintas sin un criterio reproducible. |
| Alteración no detectable | Si nadie conserva una huella de los datos originales, un cambio posterior es invisible. |
| Duplicados | Una mesa cargada dos veces infla totales sin que el sistema lo note si no hay control. |
| Desconfianza | Los resultados se cuestionan aunque el conteo sea correcto, porque no hay evidencia verificable. |
| Costo político y social | Conflictos postelectorales, impugnaciones y pérdida de legitimidad. |

## 1.6 Valor concreto que aporta la solución

1. **Integridad verificable:** cada acta registrada queda encadenada con SHA-256; si un dato cambia, la
   verificación señala la primera acta alterada (RF3). Evidencia: `CadenaHashService.Verificar`, prueba
   `AlterarUnVoto_RompeLaCadenaEnEsaActa` y reporte de evidencias.
2. **Priorización objetiva:** un z-score robusto (mediana/MAD) compara cada acta con su municipio y produce un
   puntaje y un motivo (RF5). Sobre 2.000 mesas simuladas: **recall 95,3 %** y **falsos positivos 2,8 %**
   (meta: ≥ 80 % y ≤ 10 %).
3. **Menos trabajo manual:** el auditor revisa una lista ordenada por puntaje, no miles de actas al azar.
4. **Trazabilidad:** reporte CSV con acta, puntaje, z-scores y motivo (RF6); bitácora de acciones por usuario y
   rol (RF8).
5. **Reproducibilidad:** misma semilla = mismos datos y mismas métricas; el docente puede regenerar la
   evidencia con un comando.

## 1.7 Por qué esta solución es adecuada y no solo una aplicación interesante

Porque ataca las **dos causas** del problema, no una sola:

| Causa | Respuesta del sistema | Cómo se comprueba |
|---|---|---|
| Alteración posterior al cierre | Cadena de hashes SHA-256 con `hash_anterior` | Alterar un voto rompe la verificación y señala el acta |
| Datos anómalos o inventados | z-score robusto sobre participación y apoyo por candidato | Métricas de recall/falsos positivos sobre 2.000 mesas |
| Duplicados o actas inconsistentes | Validación en alta/importación y clave única | Pruebas de filas inválidas y duplicadas |
| Falta de priorización | Reporte ordenado por puntaje con motivo | CSV exportado y pantalla de auditoría |

Y porque **no promete lo que no puede**: no prueba fraude, no reemplaza al órgano electoral, no usa datos
personales y no decide por el auditor. Su salida es una **lista de indicios para revisión humana**. Esa
honestidad es parte del valor: un sistema que dijera “fraude confirmado” sería peligroso y no sería defendible.

## 1.8 Alcance

### Sí se desarrolla

- Simulador de actas sintéticas reproducibles con fraude etiquetado (RF1).
- Registro manual e importación CSV con validaciones (RF2).
- Cadena de hashes SHA-256 y verificación de integridad (RF3).
- Cómputo por recinto, municipio, departamento y nacional (RF4).
- Auditoría estadística con z-score robusto y umbral configurable (RF5).
- Reporte CSV de actas sospechosas con puntaje y motivo (RF6).
- Evaluación del detector: precisión, recall, falsos positivos, F1 (RF7).
- Roles (Operador, Auditor, Administrador) y bitácora (RF8).
- Persistencia SQLite de solo inserción.
- Pruebas automatizadas, evidencias y documentación.

### No se desarrolla

- Sistema de votación, escrutinio oficial o reemplazo de Sirepre/SCORC.
- OCR de imágenes de actas o lectura de PDF escaneados.
- Datos personales, padrón electoral o datos reales del OEP.
- Blockchain distribuida, aplicación web o móvil, servicios en la nube.
- Conclusiones de “fraude probado” o dictámenes legales.
- Integración con sistemas oficiales.

### Funcionalidades tentadoras descartadas

| Tentación | Por qué se descarta |
|---|---|
| Mapa de calor geográfico | Vistoso, pero no cambia la decisión de qué acta revisar; alto costo en WinForms. |
| Ley de Benford | Requiere muchos datos por candidato y supuestos que no se cumplen con 2.000 mesas; difícil de explicar. |
| Machine learning | Sin datos reales etiquetados no hay entrenamiento honesto; el z-score es explicable y verificable. |
| PDF con diseño | El CSV es suficiente para el MVP y se abre en Excel; el PDF es presentación, no valor. |
| Datos reales del OEP | Privacidad, permisos y riesgo de conclusiones incorrectas; fuera del alcance académico. |
| Gráfico de dispersión / chi-cuadrado | Se posterga (T18 y chi-cuadrado); no es necesario para cumplir el objetivo general. |

## 1.9 Supuestos y restricciones

- El docente acepta **datos simulados** y una aplicación de escritorio; está pendiente la confirmación formal,
  pero no bloquea el desarrollo (el producto es autocontenido y exportable).
- El equipo tiene 3 integrantes y ~51 horas por persona en 6 semanas (153 h planificadas).
- El entorno de ejecución es **Windows 10/11 con .NET 8**; el núcleo (Dominio/Aplicación) es multiplataforma.
- No se manejan datos personales; las actas son sintéticas.
- El prototipo es local (sin servidor); la base SQLite vive en el equipo del usuario.
- La fecha de entrega/defensa no está confirmada; el plan de 6 semanas es una proyección desde el 5/11/2026.

## 1.10 Si hubiera que reducir el proyecto un 40 %

Se conserva el núcleo que resuelve el problema:

**Se conserva:** RF1 (datos), RF2 (carga/validación), RF3 (integridad), RF4 (cómputo), RF5 (z-score) y
RF6 (reporte). Es decir, el flujo completo “generar → registrar → verificar → computar → auditar → exportar”.

**Se elimina en este orden:**

1. RF8 (roles y bitácora), conservando la base de datos y un usuario único.
2. Pantalla de evaluación RF7, dejando las métricas en el reporte de evidencias.
3. Gráfico de dispersión (T18) y chi-cuadrado.
4. Persistencia SQLite, trabajando solo en memoria y CSV (si el tiempo fuera extremo).

El recorte no toca la cadena de hashes ni el cómputo: sin ellos el proyecto pierde su razón de ser.

## 1.11 Enlaces

- Requisitos y criterios de aceptación: [`02-requisitos.md`](02-requisitos.md)
- Arquitectura: [`03-arquitectura.md`](03-arquitectura.md)
- Plan: [`04-plan-gantt.md`](04-plan-gantt.md)
- Riesgos: [`05-riesgos.md`](05-riesgos.md)
- Pruebas: [`06-pruebas.md`](06-pruebas.md)
- Respuestas al cuestionario: [`08-cuestionario-aprobacion.md`](08-cuestionario-aprobacion.md)
