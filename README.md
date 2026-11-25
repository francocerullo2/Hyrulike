# Hyrulike

Hyrulike es un videojuego de acción 2D con vista cenital y elementos roguelike para Windows (64 bits), desarrollado en Unity con C#.

El jugador controla a Radnak, que debe atravesar 3 zonas de 3 niveles cada una y un nivel final con el jefe. En cada nivel hay que derrotar a todos los enemigos para abrir la salida. Al terminar un nivel se elige una mejora temporal, y al acabar cada zona una permanente (arco o guardia), que se conserva entre partidas. Al morir o ganar, la partida se puede enviar a un ranking online.

Proyecto final del ciclo de Desarrollo de Aplicaciones Multiplataforma (DAM). Autor: Franco Germán Cerullo.

## Índice

1. [Requisitos](#requisitos)
2. [Instalación del juego](#instalación-del-juego)
3. [Configuración](#configuración)
4. [Controles](#controles)
5. [Ranking online (Firebase)](#ranking-online-firebase)
6. [Abrir el proyecto en Unity](#abrir-el-proyecto-en-unity)
7. [Pruebas](#pruebas)
8. [Preguntas frecuentes](#preguntas-frecuentes)
9. [Créditos](#créditos)

## Requisitos

- Windows 10 o superior (64 bits).
- Procesador Intel Core i3 / AMD Ryzen 3.
- 4 GB de RAM.
- Gráfica compatible con DirectX 11.
- Unos 500 MB libres.
- Resolución mínima de 1280 × 720.
- Teclado.
- Conexión a Internet para el ranking.

## Instalación del juego

1. Descargar `Hyrulike.zip` desde la sección [Releases](https://github.com/francocerullo2/Hyrulike/releases/latest) de este repositorio.
2. Descomprimirlo (clic derecho → Extraer todo).
3. Abrir la carpeta `Hyrulike` y ejecutar `Hyrulike.exe`. No hace falta instalar nada más: todas las librerías necesarias están incluidas en la carpeta.
4. Si Windows muestra el aviso "Windows protegió su PC", pulsar **Más información** y después **Ejecutar de todas formas**. El aviso aparece porque el ejecutable no está firmado digitalmente.

El juego se abre en el menú principal.

> **Importante:** no mover `Hyrulike.exe` fuera de su carpeta. Necesita los archivos que tiene al lado (`Hyrulike_Data`, `UnityPlayer.dll`...) para funcionar.

## Configuración

El juego no necesita ninguna configuración previa. Desde el menú principal (**Opciones**) o durante la partida (**Esc → Ajustes**) se puede:

- **Volumen:** subir o bajar el volumen general de 10 en 10 %.
- **Pantalla completa:** activarla o desactivarla.

## Controles

| Acción | Tecla |
|---|---|
| Moverse | WASD |
| Atacar | J |
| Dash | Espacio |
| Arco (una vez desbloqueado) | K |
| Guardia (una vez desbloqueada, mantener pulsado) | L |
| Pausa | Esc |

Los controles también se muestran en el juego antes de empezar cada partida.

## Ranking online (Firebase)

Para enviar y ver puntuaciones hace falta conexión a Internet. No hay que crear ninguna cuenta: al acabar la partida basta con escribir un nombre de 3 a 10 letras o números.

El ranking usa Firebase Cloud Firestore. No hay que instalar ni configurar nada: el juego ya incluye la configuración del proyecto de Firebase (`google-services-desktop.json`, en `Assets/StreamingAssets` y, en la build, en `Hyrulike_Data/StreamingAssets`) y se conecta solo al abrir la pantalla de Game Over o de Victoria.

La estructura de la base de datos, las reglas de seguridad y los pasos para dejarla a cero están en [Docs/BaseDeDatos.md](Docs/BaseDeDatos.md).

El juego no tiene cuentas de usuario ni contraseñas.

## Abrir el proyecto en Unity

1. Instalar [Git LFS](https://git-lfs.com/). El repositorio guarda los sprites, sonidos y librerías con LFS, así que **"Download ZIP" no los descarga**: hay que clonarlo.
2. Clonar el repositorio:

   ```
   git lfs install
   git clone https://github.com/francocerullo2/Hyrulike.git
   ```

3. Instalar **Unity 6000.4.5f1** desde Unity Hub (con el módulo *Windows Build Support*).
4. En Unity Hub, pulsar **Add → Add project from disk** y elegir la carpeta clonada.
5. Abrir la escena `Assets/Scenes/MainMenu.unity` y pulsar Play.
6. Para generar el ejecutable: **File → Build Profiles → Windows → Build**.

El editor de código usado es Visual Studio Community. La documentación del código, generada con Doxygen, está en [Docs/Doxygen/html](Docs/Doxygen/html) (abrir `index.html`).

## Pruebas

Las 12 pruebas de unidad están en `Assets/Editor/Tests`. Se ejecutan en Unity desde **Window → General → Test Runner → EditMode → Run All**.

## Preguntas frecuentes

**El juego no arranca o falta algún archivo.**
Comprueba que has descomprimido el ZIP entero y que `Hyrulike.exe` sigue dentro de su carpeta, junto a `Hyrulike_Data` y `UnityPlayer.dll`. No funciona si se ejecuta desde dentro del ZIP sin descomprimir.

**Windows muestra "Windows protegió su PC".**
Pulsa Más información y después Ejecutar de todas formas. Aparece porque el ejecutable no está firmado digitalmente.

**El ranking muestra "Sin conexion" o "No se ha podido enviar".**
El ranking necesita Internet. Sin conexión se puede jugar igual, pero no se pueden enviar ni ver puntuaciones. Si falla el envío, el botón Enviar se vuelve a activar para reintentarlo.

**No me deja enviar mi nombre.**
El nombre debe tener de 3 a 10 caracteres y solo letras o números, sin espacios ni símbolos.

**¿Cómo cambio el volumen o la pantalla completa?**
Desde Opciones en el menú principal o desde Esc → Ajustes durante la partida.

**¿Se guarda la partida a medias?**
No. Es un roguelike: cada partida empieza desde el primer nivel. Solo se conservan las mejoras permanentes (arco y guardia).

**Al abrir el proyecto en Unity faltan sprites o sonidos.**
El repositorio se descargó sin Git LFS. Instala Git LFS y ejecuta `git lfs pull` dentro de la carpeta del proyecto.

## Créditos

- [Tiny Swords](https://pixelfrog-assets.itch.io/tiny-swords), de Pixel Frog.
- [Ninja Adventure Asset Pack](https://pixel-boy.itch.io/ninja-adventure-asset-pack), de Pixel-boy y AAA (CC0).
