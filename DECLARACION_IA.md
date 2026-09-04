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

### 4 de septiembre de 2026 · Terreno y materiales

| Elemento | Grado de asistencia |
|---|---|
| Generación del terreno en Unity | Ejecutada por el autor. Los parámetros (500 m de lado, 3000 árboles, especies usadas) los eligió él. |
| Depuración de `GeneradorSelva.cs` | **Corregido con IA.** Tres fallas: el terreno salía plano al regenerar porque el asset se creaba vacío y se rellenaba después, un error de consola por cambiar la selección durante `OnGUI`, y un mensaje de log que informaba árboles pedidos en vez de plantados. El diagnóstico se hizo con evidencia, midiendo el contenido real del archivo generado. |
| Distancia de billboard de los árboles | **Decidida con IA.** Se igualó a la distancia de dibujado porque los billboards de terreno requieren el shader `Nature/Soft Occlusion`, que no existe en URP. Las advertencias de consola que quedan son una limitación conocida del motor, no un error del proyecto. |
| Materiales de los árboles (`Assets/Arte/Naturaleza/Materiales/`) | **Asignados con IA** editando los archivos de material: mapa base, normal map y recorte alfa en las hojas. Las texturas venían en el pack de Quaternius, no se generó ninguna. El autor hizo la prueba que identificó el problema. |

---

*Este registro se actualiza en cada jornada de trabajo.*
