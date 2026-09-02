using UnityEngine;
using UnityEditor;

/// <summary>
/// Herramienta de editor que construye el terreno de la selva: relieve, mezcla de
/// texturas, arboles y sotobosque. No forma parte del juego, solo del editor.
/// Se abre desde el menu Herramientas > Generador de Selva.
/// </summary>
public class GeneradorSelva : EditorWindow
{
    // ---------------------------------------------------------------- relieve
    private int semilla = 2026;
    private float ladoTerreno = 500f;
    private float alturaMaxima = 55f;
    private float escalaRuido = 160f;
    private int octavas = 4;
    private float persistencia = 0.5f;
    private float lacunaridad = 2f;
    private float radioValle = 0.30f;
    private float bordeValle = 0.20f;

    // --------------------------------------------------------------- texturas
    private Texture2D texturaHierba;
    private Texture2D texturaTierra;
    private Texture2D texturaRoca;
    private float tamanoMosaico = 12f;
    private float pendienteRoca = 32f;

    // ------------------------------------------------------------- vegetacion
    private GameObject arbolA;
    private GameObject arbolB;
    private GameObject arbolC;
    private int cantidadArboles = 700;
    private float pendienteMaxArboles = 28f;
    private float radioClaroCentral = 25f;

    private GameObject matorral;
    private int densidadMatorral = 4;

    private Vector2 desplazamientoScroll;

    [MenuItem("Herramientas/Generador de Selva")]
    private static void Abrir()
    {
        GeneradorSelva ventana = GetWindow<GeneradorSelva>("Generador de Selva");
        ventana.minSize = new Vector2(360f, 460f);
    }

    private void OnGUI()
    {
        desplazamientoScroll = EditorGUILayout.BeginScrollView(desplazamientoScroll);

        EditorGUILayout.LabelField("Relieve", EditorStyles.boldLabel);
        semilla = EditorGUILayout.IntField("Semilla", semilla);
        ladoTerreno = EditorGUILayout.Slider("Lado del terreno (m)", ladoTerreno, 200f, 1000f);
        alturaMaxima = EditorGUILayout.Slider("Altura maxima (m)", alturaMaxima, 10f, 150f);
        escalaRuido = EditorGUILayout.Slider("Escala del ruido", escalaRuido, 40f, 400f);
        octavas = EditorGUILayout.IntSlider("Octavas", octavas, 1, 6);
        persistencia = EditorGUILayout.Slider("Persistencia", persistencia, 0.1f, 0.9f);
        lacunaridad = EditorGUILayout.Slider("Lacunaridad", lacunaridad, 1.5f, 3f);
        radioValle = EditorGUILayout.Slider("Radio del valle", radioValle, 0.05f, 0.6f);
        bordeValle = EditorGUILayout.Slider("Suavidad del borde", bordeValle, 0.05f, 0.5f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Texturas del suelo", EditorStyles.boldLabel);
        texturaHierba = (Texture2D)EditorGUILayout.ObjectField("Hierba (zonas bajas)", texturaHierba, typeof(Texture2D), false);
        texturaTierra = (Texture2D)EditorGUILayout.ObjectField("Tierra (zonas medias)", texturaTierra, typeof(Texture2D), false);
        texturaRoca = (Texture2D)EditorGUILayout.ObjectField("Roca (pendientes)", texturaRoca, typeof(Texture2D), false);
        tamanoMosaico = EditorGUILayout.Slider("Tamano del mosaico (m)", tamanoMosaico, 2f, 40f);
        pendienteRoca = EditorGUILayout.Slider("Pendiente que vuelve roca", pendienteRoca, 10f, 60f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Vegetacion", EditorStyles.boldLabel);
        arbolA = (GameObject)EditorGUILayout.ObjectField("Arbol A", arbolA, typeof(GameObject), false);
        arbolB = (GameObject)EditorGUILayout.ObjectField("Arbol B", arbolB, typeof(GameObject), false);
        arbolC = (GameObject)EditorGUILayout.ObjectField("Arbol C", arbolC, typeof(GameObject), false);
        cantidadArboles = EditorGUILayout.IntSlider("Cantidad de arboles", cantidadArboles, 0, 3000);
        pendienteMaxArboles = EditorGUILayout.Slider("Pendiente maxima", pendienteMaxArboles, 5f, 60f);
        radioClaroCentral = EditorGUILayout.Slider("Claro central libre (m)", radioClaroCentral, 0f, 120f);
        matorral = (GameObject)EditorGUILayout.ObjectField("Matorral (detalle)", matorral, typeof(GameObject), false);
        densidadMatorral = EditorGUILayout.IntSlider("Densidad del matorral", densidadMatorral, 0, 16);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Genera un terreno nuevo en la escena abierta. Si ya existe uno generado antes, se reemplaza.",
            MessageType.Info);

        GUI.enabled = texturaHierba != null && texturaTierra != null && texturaRoca != null;
        if (GUILayout.Button("Generar selva", GUILayout.Height(34f)))
        {
            Generar();
        }
        GUI.enabled = true;

        if (texturaHierba == null || texturaTierra == null || texturaRoca == null)
        {
            EditorGUILayout.HelpBox("Faltan texturas de suelo por asignar.", MessageType.Warning);
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>Orquesta la generacion completa, paso por paso.</summary>
    private void Generar()
    {
        TerrainData datos = CrearDatosDeTerreno();

        AplicarRelieve(datos);
        PintarSuelo(datos);
        PlantarArboles(datos);
        SembrarMatorral(datos);

        ColocarEnEscena(datos);

        AssetDatabase.SaveAssets();
        Debug.Log("Selva generada: " + ladoTerreno + " m de lado, " + cantidadArboles + " arboles.");
    }

    /// <summary>Crea el asset de TerrainData con las resoluciones y el tamano elegidos.</summary>
    private TerrainData CrearDatosDeTerreno()
    {
        TerrainData datos = new TerrainData();
        datos.heightmapResolution = 513;
        datos.alphamapResolution = 512;
        datos.SetDetailResolution(512, 16);
        datos.size = new Vector3(ladoTerreno, alturaMaxima, ladoTerreno);

        const string ruta = "Assets/Terreno/TerrenoSelva.asset";
        AssetDatabase.DeleteAsset(ruta);
        AssetDatabase.CreateAsset(datos, ruta);
        return datos;
    }

    // ------------------------------------------------------------------ paso 1
    /// <summary>Escribe el mapa de alturas usando ruido Perlin de varias octavas.</summary>
    private void AplicarRelieve(TerrainData datos)
    {
        int resolucion = datos.heightmapResolution;
        float[,] alturas = new float[resolucion, resolucion];

        Random.InitState(semilla);
        Vector2 corrimiento = new Vector2(Random.Range(-9999f, 9999f), Random.Range(-9999f, 9999f));

        for (int fila = 0; fila < resolucion; fila++)
        {
            for (int columna = 0; columna < resolucion; columna++)
            {
                float x = (float)columna / (resolucion - 1);
                float y = (float)fila / (resolucion - 1);
                alturas[fila, columna] = RuidoFractal(x, y, corrimiento) * FactorValle(x, y);
            }
        }

        datos.SetHeights(0, 0, alturas);
    }

    /// <summary>Suma varias capas de Perlin, cada una mas fina y con menos peso que la anterior.</summary>
    private float RuidoFractal(float x, float y, Vector2 corrimiento)
    {
        float total = 0f;
        float amplitud = 1f;
        float frecuencia = 1f;
        float amplitudTotal = 0f;

        for (int octava = 0; octava < octavas; octava++)
        {
            float muestraX = (x * ladoTerreno / escalaRuido) * frecuencia + corrimiento.x;
            float muestraY = (y * ladoTerreno / escalaRuido) * frecuencia + corrimiento.y;

            total += Mathf.PerlinNoise(muestraX, muestraY) * amplitud;
            amplitudTotal += amplitud;

            amplitud *= persistencia;
            frecuencia *= lacunaridad;
        }

        return total / amplitudTotal;
    }

    /// <summary>Aplana el centro del mapa para dejar un valle recorrible y deja los cerros en el borde.</summary>
    private float FactorValle(float x, float y)
    {
        float distancia = Vector2.Distance(new Vector2(x, y), new Vector2(0.5f, 0.5f)) * 2f;
        float mezcla = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(radioValle, radioValle + bordeValle, distancia));
        return Mathf.Lerp(0.12f, 1f, mezcla);
    }

    // ------------------------------------------------------------------ paso 2
    /// <summary>Arma tres capas de textura y las mezcla segun altura y pendiente.</summary>
    private void PintarSuelo(TerrainData datos)
    {
        datos.terrainLayers = new TerrainLayer[]
        {
            CrearCapa(texturaHierba, "CapaHierba"),
            CrearCapa(texturaTierra, "CapaTierra"),
            CrearCapa(texturaRoca, "CapaRoca")
        };

        int resolucion = datos.alphamapResolution;
        float[,,] mezcla = new float[resolucion, resolucion, 3];

        for (int fila = 0; fila < resolucion; fila++)
        {
            for (int columna = 0; columna < resolucion; columna++)
            {
                float x = (float)columna / (resolucion - 1);
                float y = (float)fila / (resolucion - 1);

                float altura = datos.GetInterpolatedHeight(x, y) / datos.size.y;
                float pendiente = datos.GetSteepness(x, y);

                float pesoHierba = 1f - Mathf.InverseLerp(0.15f, 0.45f, altura);
                float pesoTierra = 1f - Mathf.Abs(altura - 0.45f) * 2.5f;
                float pesoRoca = Mathf.InverseLerp(pendienteRoca - 8f, pendienteRoca + 8f, pendiente);

                pesoHierba = Mathf.Max(pesoHierba, 0.05f) * (1f - pesoRoca);
                pesoTierra = Mathf.Max(pesoTierra, 0.05f) * (1f - pesoRoca);

                float suma = pesoHierba + pesoTierra + pesoRoca;
                mezcla[fila, columna, 0] = pesoHierba / suma;
                mezcla[fila, columna, 1] = pesoTierra / suma;
                mezcla[fila, columna, 2] = pesoRoca / suma;
            }
        }

        datos.SetAlphamaps(0, 0, mezcla);
    }

    /// <summary>Crea un TerrainLayer a partir de una textura suelta, sin pasar por el inspector.</summary>
    private TerrainLayer CrearCapa(Texture2D textura, string nombre)
    {
        TerrainLayer capa = new TerrainLayer();
        capa.diffuseTexture = textura;
        capa.tileSize = new Vector2(tamanoMosaico, tamanoMosaico);
        capa.name = nombre;

        string ruta = "Assets/Terreno/" + nombre + ".terrainlayer";
        AssetDatabase.DeleteAsset(ruta);
        AssetDatabase.CreateAsset(capa, ruta);
        return capa;
    }

    // ------------------------------------------------------------------ paso 3
    /// <summary>Reparte arboles al azar, evitando pendientes fuertes y el claro del centro.</summary>
    private void PlantarArboles(TerrainData datos)
    {
        GameObject[] modelos = FiltrarNulos(new GameObject[] { arbolA, arbolB, arbolC });
        if (modelos.Length == 0 || cantidadArboles == 0)
        {
            return;
        }

        TreePrototype[] prototipos = new TreePrototype[modelos.Length];
        for (int i = 0; i < modelos.Length; i++)
        {
            prototipos[i] = new TreePrototype();
            prototipos[i].prefab = modelos[i];
        }
        datos.treePrototypes = prototipos;

        Random.InitState(semilla + 1);
        System.Collections.Generic.List<TreeInstance> arboles = new System.Collections.Generic.List<TreeInstance>();
        float radioNormalizado = radioClaroCentral / ladoTerreno;

        int intentos = cantidadArboles * 4;
        for (int i = 0; i < intentos && arboles.Count < cantidadArboles; i++)
        {
            float x = Random.value;
            float y = Random.value;

            if (datos.GetSteepness(x, y) > pendienteMaxArboles)
            {
                continue;
            }
            if (Vector2.Distance(new Vector2(x, y), new Vector2(0.5f, 0.5f)) < radioNormalizado)
            {
                continue;
            }

            TreeInstance arbol = new TreeInstance();
            arbol.position = new Vector3(x, datos.GetInterpolatedHeight(x, y) / datos.size.y, y);
            arbol.prototypeIndex = Random.Range(0, prototipos.Length);
            arbol.widthScale = Random.Range(0.8f, 1.3f);
            arbol.heightScale = Random.Range(0.8f, 1.4f);
            arbol.rotation = Random.Range(0f, Mathf.PI * 2f);
            arbol.color = Color.white;
            arbol.lightmapColor = Color.white;
            arboles.Add(arbol);
        }

        datos.SetTreeInstances(arboles.ToArray(), true);
    }

    // ------------------------------------------------------------------ paso 4
    /// <summary>Siembra matorral como capa de detalle, mas densa en las zonas planas.</summary>
    private void SembrarMatorral(TerrainData datos)
    {
        if (matorral == null || densidadMatorral == 0)
        {
            return;
        }

        DetailPrototype prototipo = new DetailPrototype();
        prototipo.prototype = matorral;
        prototipo.usePrototypeMesh = true;
        prototipo.useInstancing = true;
        prototipo.renderMode = DetailRenderMode.VertexLit;
        prototipo.minWidth = 0.7f;
        prototipo.maxWidth = 1.4f;
        prototipo.minHeight = 0.7f;
        prototipo.maxHeight = 1.4f;
        datos.detailPrototypes = new DetailPrototype[] { prototipo };

        int resolucion = datos.detailResolution;
        int[,] capa = new int[resolucion, resolucion];

        for (int fila = 0; fila < resolucion; fila++)
        {
            for (int columna = 0; columna < resolucion; columna++)
            {
                float x = (float)columna / (resolucion - 1);
                float y = (float)fila / (resolucion - 1);
                bool planoSuficiente = datos.GetSteepness(x, y) < 20f;
                capa[fila, columna] = planoSuficiente ? densidadMatorral : 0;
            }
        }

        datos.SetDetailLayer(0, 0, 0, capa);
    }

    // ------------------------------------------------------------------ paso 5
    /// <summary>Reemplaza el terreno anterior por el nuevo y ajusta las distancias de dibujado.</summary>
    private void ColocarEnEscena(TerrainData datos)
    {
        GameObject anterior = GameObject.Find("TerrenoSelva");
        if (anterior != null)
        {
            DestroyImmediate(anterior);
        }

        GameObject objeto = Terrain.CreateTerrainGameObject(datos);
        objeto.name = "TerrenoSelva";
        objeto.transform.position = new Vector3(-ladoTerreno * 0.5f, 0f, -ladoTerreno * 0.5f);

        Terrain terreno = objeto.GetComponent<Terrain>();
        terreno.detailObjectDistance = 90f;
        terreno.treeBillboardDistance = 70f;
        terreno.treeDistance = 350f;
        terreno.heightmapPixelError = 8f;

        Undo.RegisterCreatedObjectUndo(objeto, "Generar selva");
        Selection.activeGameObject = objeto;
    }

    /// <summary>Devuelve solo los modelos realmente asignados en la ventana.</summary>
    private GameObject[] FiltrarNulos(GameObject[] posibles)
    {
        System.Collections.Generic.List<GameObject> validos = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject candidato in posibles)
        {
            if (candidato != null)
            {
                validos.Add(candidato);
            }
        }
        return validos.ToArray();
    }
}
