# -*- coding: utf-8 -*-
"""
Genera las tres texturas de suelo de Assets/Arte/Suelo.
Se dejan versionadas junto al proyecto para que el resultado sea reproducible
y quede claro que las texturas son propias y no descargadas.

Uso:  python generar_texturas_suelo.py
Requiere: numpy y pillow.
"""
import numpy as np
from PIL import Image

TAM = 512
DESTINO = "../Assets/Arte/Suelo/"

def ruido_periodico(tam, celdas, rng):
    """Value noise que se repite exacto en los bordes, para que la textura calce consigo misma."""
    g = rng.random((celdas, celdas))
    t = np.linspace(0, celdas, tam, endpoint=False)
    i = np.floor(t).astype(int)
    f = t - i
    u = f * f * (3 - 2 * f)
    i0, i1 = i % celdas, (i + 1) % celdas
    c00 = g[np.ix_(i0, i0)]; c10 = g[np.ix_(i0, i1)]
    c01 = g[np.ix_(i1, i0)]; c11 = g[np.ix_(i1, i1)]
    arriba = c00 * (1 - u)[None, :] + c10 * u[None, :]
    abajo  = c01 * (1 - u)[None, :] + c11 * u[None, :]
    return arriba * (1 - u)[:, None] + abajo * u[:, None]

def fbm(tam, semilla, celdas_base, octavas):
    """Suma varias octavas de ruido, cada una mas fina y con menos peso."""
    rng = np.random.default_rng(semilla)
    total = np.zeros((tam, tam)); amp = 1.0; amp_total = 0.0; celdas = celdas_base
    for _ in range(octavas):
        total += ruido_periodico(tam, celdas, rng) * amp
        amp_total += amp; amp *= 0.5; celdas *= 2
    v = total / amp_total
    return (v - v.min()) / (v.max() - v.min())

def posterizar(v, niveles):
    """Reduce el degradado a unos pocos escalones: de aqui sale el aspecto plano."""
    return np.clip(np.floor(v * niveles) / (niveles - 1), 0, 1)

def pintar(v, paleta):
    n = len(paleta)
    idx = np.clip((v * n).astype(int), 0, n - 1)
    salida = np.zeros(v.shape + (3,), dtype=np.uint8)
    for k, color in enumerate(paleta):
        salida[idx == k] = color
    return salida

CAPAS = {
    "Suelo_Hierba": (11, 6, 5, 6,
        [(74,110,48), (88,126,54), (101,141,60), (114,155,67), (128,168,75), (142,180,84)]),
    "Suelo_Tierra": (27, 5, 5, 5,
        [(92,68,45), (106,79,52), (120,91,60), (134,104,70), (147,118,82)]),
    "Suelo_Roca": (43, 4, 6, 5,
        [(96,97,102), (110,111,116), (124,126,131), (138,140,145), (152,155,160)]),
}

if __name__ == "__main__":
    for nombre, (semilla, celdas, octavas, niveles, paleta) in CAPAS.items():
        v = posterizar(fbm(TAM, semilla, celdas, octavas), niveles)
        Image.fromarray(pintar(v, paleta), "RGB").save(DESTINO + nombre + ".png", optimize=True)
        print(nombre, "listo")
