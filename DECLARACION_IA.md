# Declaración de uso de inteligencia artificial

**Proyecto:** Explorador en la Selva
**Ramo:** Taller de Programación de Videojuegos (800TV06), Módulo II
**Autor:** Gonzalo Tapia Vergara
**Herramienta usada:** Claude (Anthropic)

Este documento registra dónde se usó asistencia de IA en el desarrollo del proyecto. Se escribe
a medida que el trabajo avanza, no reconstruido al final, para que refleje lo que realmente pasó.

---

## Criterio general

El **código de juego** de `Assets/Scripts/` lo escribe el autor. La IA se usa para explicar
enfoques, resolver dudas puntuales y revisar el resultado, no para generar esos archivos.

La **herramienta de editor** y las **utilidades de apoyo**, que son código mecánico y no forman
parte del juego entregado, sí se generaron con asistencia de IA. Están identificadas abajo.

---

## Registro

### 2 de septiembre de 2026 · Preparación

| Elemento | Grado de asistencia |
|---|---|
| Planificación del proyecto: alcance, cronograma y lectura de la rúbrica | Conversación con IA. Las decisiones de alcance, plazo, estilo visual y herramientas las tomó el autor. |
| `Assets/Editor/GeneradorSelva.cs` | **Generado con IA** a partir de una especificación acordada. Es una herramienta de editor: no se compila dentro del juego. Revisado y ejecutado por el autor. |
| `Herramientas/generar_texturas_suelo.py` | **Generado con IA.** Utilidad fuera de `Assets/`, no forma parte del juego. |
| Las tres texturas de `Assets/Arte/Suelo/` | **Generadas** por el script anterior. No se descargaron de ningún banco de texturas. |
| Selección de los packs de assets | Búsqueda y comparación de licencias asistida por IA. La decisión de estilo, estilizado y liviano, fue del autor. |
| Extracción y organización de los assets en `Assets/Arte/` | Ejecutada con asistencia de IA, incluyendo la exclusión deliberada de las carpetas `OBJ/`, `glTF/` y `Blends/`. |
| Configuración de `.gitattributes` | Asistida por IA. Se desactivó Git LFS porque estaba declarado pero no funcionaba: los binarios del primer commit se guardaron crudos en vez de como punteros. |
| `README.md` y este archivo | Redactados con asistencia de IA a partir del contenido real del proyecto. |

---

*Este registro se actualiza en cada jornada de trabajo.*
