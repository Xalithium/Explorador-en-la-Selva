# Explorador en la Selva

Escena jugable en Unity 3D desarrollada para la Actividad Evaluativa EPE2 del ramo
**Taller de Programación de Videojuegos (800TV06)**, Módulo II, Instituto Profesional de Chile.

El jugador controla a un explorador en tercera persona que recorre una selva, interactúa con
cofres, antorchas y rocas movibles, y esquiva trampas, precipicios y criaturas hostiles.

- **Editor:** Unity 6000.5.0f1
- **Pipeline:** Universal Render Pipeline (URP)
- **Entrada:** Input System (paquete nuevo)

---

## Criterio de nombres

El proyecto mezcla inglés y español de forma deliberada, según tres reglas:

1. **Lo que trae el motor o la plantilla conserva su nombre original.** Por ejemplo
   `InputSystem_Actions.inputactions` o los assets de `Assets/Settings/`.
2. **Las carpetas y APIs de Unity van en inglés**, por convención del motor y del material del
   curso: `Assets/`, `Editor/`, `Scripts/`, `Scenes/`, `Start()`, `OnCollisionEnter()`.
3. **Todo el código propio va en español**: clases, métodos, variables y tags.

---

## Estructura

| Ruta | Qué contiene |
|---|---|
| `Assets/Editor/` | Herramientas que corren solo dentro del editor y no se compilan en el juego |
| `Assets/Scripts/` | Los componentes del juego, una carpeta plana, un script por responsabilidad |
| `Assets/Arte/` | Modelos y texturas, separados por origen |
| `Assets/Terreno/` | TerrainData y capas de terreno, generados por herramienta |
| `Herramientas/` | Utilidades fuera de `Assets/`, para que Unity no las importe |

### Scripts

| Archivo | Qué hace |
|---|---|
| `Assets/Editor/GeneradorSelva.cs` | Ventana de editor que construye el terreno completo: relieve por ruido Perlin de varias octavas con un valle central navegable, mezcla de tres capas de textura según altura y pendiente, distribución de árboles evitando pendientes fuertes y el claro central, y siembra de matorral como capa de detalle. Se abre desde el menú `Herramientas > Generador de Selva`. |

Esta tabla se completa a medida que se escriben los scripts de juego.

> **Nota sobre `Assets/InputSystem_Actions.cs`.** Ese archivo tiene unas 1800 líneas y no
> está escrito a mano: lo genera Unity automáticamente a partir de
> `InputSystem_Actions.inputactions` cuando se activa la opción *Generate C# Class*. Se
> versiona porque el proyecto no compila sin él, pero no es código de autoría propia ni
> debe leerse como tal.

### Herramientas fuera de Assets

| Archivo | Qué hace |
|---|---|
| `Herramientas/generar_texturas_suelo.py` | Genera las tres texturas de `Assets/Arte/Suelo/` con ruido periódico posterizado. Se versiona para que el resultado sea reproducible y quede claro que las texturas son propias. Requiere numpy y pillow. |

---

## Créditos y licencias de assets

| Origen | Qué se usa | Licencia |
|---|---|---|
| [Quaternius](https://quaternius.com/) · Stylized Nature MegaKit | Árboles, plantas, rocas y matorral | Quaternius Asset License (QAL) v1.0: uso personal, educativo y comercial permitido, sin atribución obligatoria, sin reventa de los assets sueltos |
| [Quaternius](https://quaternius.com/) · Ultimate Modular Ruins Pack | Ruinas modulares, cofre, antorcha, trampa y props | QAL v1.0 |
| [Quaternius](https://quaternius.com/) · Ultimate Animated Character Pack | Personaje jugable y sus animaciones | QAL v1.0 |
| Propio | Las tres texturas de suelo de `Assets/Arte/Suelo/` | Generadas con el script de `Herramientas/` |

De los packs de Quaternius se importaron únicamente los archivos FBX y sus texturas. Las carpetas
`OBJ/`, `glTF/` y `Blends/` se dejaron fuera a propósito, tanto por peso como porque los `.blend`
provocan errores de importación en Unity si no hay Blender instalado.

---

## Uso de IA

El detalle de dónde se usó asistencia de inteligencia artificial está en
[`DECLARACION_IA.md`](DECLARACION_IA.md), en la raíz de este repositorio.
