# 10. Guion de demo y defensa

**Duración objetivo:** 8–10 minutos de demo + 5–8 minutos de preguntas.
**Regla:** no se explica código línea por línea; se muestra **problema → decisión → evidencia**.

## 10.1 Antes de entrar (checklist de 5 minutos)

- [ ] `dotnet build` sin errores ni advertencias.
- [ ] `dotnet test` con 47 pruebas en verde.
- [ ] `dotnet run --project src/AuditaActas.Evidencia` genera el reporte (hacerlo antes de la defensa).
- [ ] Abrir `docs/evidencias/reporte-verificacion.md` en una pestaña.
- [ ] Abrir la app con `dotnet run --project src/AuditaActas.UI` y dejarla en la pantalla inicial.
- [ ] Cerrar notificaciones; aumentar el zoom de la terminal y de la app.
- [ ] Tener a mano `datos/ejemplo_con_errores.csv` para la demo de importación.
- [ ] Plan B: si la UI falla, mostrar el reporte de evidencias y el CSV; la consola corre en cualquier SO.

## 10.2 Guion paso a paso

### 0:00–0:45 — Problema (sin tecnología)

> “El resultado de una elección se arma sumando miles de actas llenadas a mano. Si un acta se altera, se
> duplica o se llena con números inventados, hoy detectarlo depende de que alguien lo note revisando papeles.
> El problema es doble: no hay forma de demostrar que lo contado sigue siendo lo que decía el acta, y no hay
> un criterio objetivo para decidir cuáles de miles revisar primero. Nosotros hacemos dos cosas: una cadena de
> huellas que revela alteraciones y una prueba estadística que prioriza qué actas revisar. No probamos fraude:
> damos indicios para revisión humana.”

**No** diga “mi aplicación en WinForms con SQLite” en los primeros 30 segundos. Primero el problema.

### 0:45–1:30 — Solución y alcance

> “El sistema tiene tres roles: Operador carga, Auditor analiza, Administrador controla. El flujo es:
> generar o importar actas → verificar integridad → computar → auditar → exportar. El MVP incluye datos
> reproducibles, validaciones, hash, cómputo, z-score y reporte. Después agregamos evaluación y roles.
> Lo que **no** hacemos: votación, OCR, datos reales, web, conclusiones legales.”

Muestre el README o el índice de docs si quiere respaldo.

### 1:30–3:00 — Generar y verificar integridad

1. En la app: **Mesas 2000**, **% fraude 5**, **Semilla 42** → **Generar datos**.
2. Diga: “Genera 2.000 actas, 106 con fraude etiquetado, y las encadena con SHA-256. La misma semilla produce
   exactamente los mismos datos.”
3. **Verificar integridad** → “Integridad OK: 2.000 actas verificadas.”
4. **Alterar 1 acta (demo)** → “Cambié un voto a propósito.”
5. **Verificar integridad** → “Cadena rota en la posición 1001: el acta D03/M021/R0101/1 no coincide con su
   hash.” La fila se pinta en rojo.

**Frase clave:** “No guardamos solo el dato: guardamos la huella del dato y la huella del acta anterior. Si
alguien cambia una acta, se rompe el eslabón y sabemos exactamente dónde.”

### 3:00–4:00 — Cómputo

1. Elija **Nivel: Departamento** → **Resultados**.
2. Muestre las columnas: mesas, habilitados, emitidos, participación, votos.
3. Diga: “Los totales por nivel suman exactamente el total nacional; hay una prueba que lo verifica.”

### 4:00–5:30 — Auditoría y métricas

1. **Auditar** → lista de sospechosas con puntaje y motivo.
2. Lea un motivo: “Participación atípica (z=…)” o “Apoyo atípico a Candidato A (z=…)”.
3. Mire la barra inferior: “154 sospechosas de 2.000 | Recall 95,3 % | Falsos positivos 2,8 %”.
4. **Evaluación del detector** → matriz VP/FP/FN/VN y métricas.

**Frase clave:** “Sobre 2.000 mesas con 5 % de fraude inyectado, encontramos el 95,3 % de las actas
manipuladas con 2,8 % de falsos positivos. La meta era 80 % y 10 %. El umbral es configurable: 3,5 da 81,1 %
y 1,2 % de falsos positivos.”

### 5:30–6:30 — Importación y validaciones

1. **Importar CSV** → `datos/ejemplo_con_errores.csv`.
2. Debe importar 1 acta y rechazar 2: “Fila 3: emitidos 120 superan habilitados 100”; “Fila 4: mesa duplicada”.
3. Diga: “El sistema no acepta datos inconsistentes ni sobrescribe actas. Las actas son de solo inserción.”

### 6:30–7:15 — Reporte y persistencia

1. **Exportar sospechosas** → guarde en el escritorio y abra el CSV.
2. Muestre las columnas: acta, participación, puntaje, z, motivo.
3. Si hay tiempo: **Guardar base** → `.db`; **Cargar base** → verifica la cadena al cargar.

### 7:15–8:00 — Roles y cierre

1. Cambie **Rol: Operador** → “Auditar”, “Evaluación” y “Exportar” se deshabilitan.
2. Cambie **Rol: Administrador** → **Bitácora** muestra usuario, rol, acción y hora.
3. Cierre: “Cada afirmación tiene una prueba o un reporte reproducible: 47 pruebas, build sin advertencias y
   un comando que regenera las métricas. El repositorio incluye las respuestas a las 52 preguntas.”

## 10.3 Números que debe memorizar

| Dato | Valor |
|---|---|
| Dataset estándar | 2.000 mesas, semilla 42, 5 % fraude |
| Actas con fraude etiquetado | 106 (5,3 %) |
| Recall | **95,3 %** (meta ≥ 80 %) |
| Falsos positivos | **2,8 %** (meta ≤ 10 %) |
| Precisión | 65,6 % |
| Alertas | 154 de 2.000 |
| Rendimiento | 10.000 actas en **≈32 ms** (meta < 10 s) |
| Pruebas | **47** |
| Tareas | 21, 153 h, 3 personas |
| Umbral | 3,0 (barrido 2,0–4,0) |

## 10.4 Preguntas difíciles y respuestas cortas

**“¿Por qué z-score robusto y no machine learning?”**
Porque no hay datos reales etiquetados para entrenar; un modelo sería una caja negra no verificable. Mediana y
MAD son explicables, resistentes a outliers y se pueden probar con Excel/Python. El z-score es una elección
deliberada, no una limitación.

**“¿Por qué no Benford?”**
Requiere muchos datos por candidato y supuestos que no se cumplen con 2.000 mesas; además es difícil de
explicar y de defender. Se evaluó y se descartó.

**“¿La cadena de hashes no se puede reescribir?”**
Sí, si un atacante controla todo el archivo puede recalcular la cadena. Por eso la declaramos como detección
de alteraciones parciales, no como seguridad total. Para producción haría falta firma digital y publicar el
hash raíz en un medio independiente.

**“¿Esto prueba fraude?”**
No. Marca indicios para revisión humana. El aviso está en la UI y en el README. Decir lo contrario sería
irresponsable.

**“¿Los datos son reales?”**
No, son sintéticos y reproducibles. No usamos datos personales ni del OEP; el alcance lo prohíbe.

**“¿Por qué WinForms y no web?”**
Es escritorio local, sin servidor ni despliegue, con las tecnologías del curso. La arquitectura está separada:
si mañana se quiere web, se reutiliza Dominio/Aplicacion y se cambia la UI.

**“¿Qué pasa si el acta tiene pocas actas en su municipio?”**
El detector usa respaldo: municipio → departamento → nacional si hay menos de 8 actas. Está implementado y
probado con datasets pequeños.

**“¿Cómo sé que el reporte no está inventado?”**
El comando `dotnet run --project src/AuditaActas.Evidencia` regenera el reporte y el CSV desde el código; las
47 pruebas se ejecutan con `dotnet test`. Todo es reproducible con la semilla 42.

**“¿Qué falta?”**
El gráfico de dispersión y chi-cuadrado son post-MVP; no bloquean el objetivo. La fecha de defensa está
pendiente de confirmación. Lo demás está implementado y verificado.

**“¿Cuál fue el mayor riesgo?”**
Que el detector no alcanzara las métricas. Se mitigó con dos tipos de fraude, barrido de umbral y pruebas; el
resultado (95,3 % / 2,8 %) deja margen.

**“¿Qué harían con más tiempo?”**
Gráfico de dispersión, chi-cuadrado, PDF y, sobre todo, validación con datos reales anonimizados y permisos.
No agregaríamos ML sin datos.

## 10.5 Plan B si algo falla

| Falla | Plan B |
|---|---|
| La UI no abre | Mostrar `docs/evidencias/reporte-verificacion.md` y el CSV; explicar el flujo con las tablas. |
| `dotnet run` falla | `dotnet test` + `dotnet run --project src/AuditaActas.Evidencia`; el núcleo no depende de la UI. |
| No hay internet | Todo está restaurado; la app funciona offline. |
| Se acaba el tiempo | Saltar importación/persistencia y cerrar con métricas + roles. |
| Pregunta técnica inesperada | “Está documentado en `docs/03-arquitectura.md`; se lo muestro.” No improvisar. |

## 10.6 Cierre en una frase

> “AuditaActas no decide si hubo fraude: hace que la revisión sea **verificable**, **objetiva** y **más
> barata**, y deja la decisión final en manos de una persona.”
