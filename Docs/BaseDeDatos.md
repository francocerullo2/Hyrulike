# Base de datos de Hyrulike

## 1. Tecnología

El ranking online de Hyrulike usa **Firebase Cloud Firestore**, una base de datos NoSQL en la nube de Google, con el plan gratuito **Spark**.

El juego se conecta a Firestore con el **SDK de Firebase para Unity (versión 14.1.0)**, en concreto con el paquete de Firestore.

Elegí Firestore en lugar de MySQL o SQLite por estos motivos:

- El ranking tiene que ser **compartido** entre jugadores de distintos ordenadores. SQLite solo guardaría los datos en el PC de cada jugador.
- No hace falta montar ni mantener un servidor propio, como pasaría con MySQL.
- El plan gratuito es suficiente para el número de partidas de un proyecto como este.

El progreso del jugador (las mejoras permanentes: arco y guardia) **no** va en Firestore. Se guarda en local con `PlayerPrefs` (ver el apartado 8).

## 2. Estructura

La base de datos tiene una sola colección, **`scores`**. Cada documento es una partida enviada al ranking y su ID lo genera Firestore automáticamente.

| Campo   | Tipo   | Descripción                                    | Ejemplo  |
|---------|--------|------------------------------------------------|----------|
| `name`  | string | Alias del jugador (de 3 a 10 letras o números) | `Franco` |
| `level` | int    | Nivel alcanzado (de 1 a 10)                    | `7`      |
| `time`  | int    | Duración de la partida en segundos             | `845`    |
| `won`   | bool   | `true` si ha derrotado al jefe final           | `false`  |
| `score` | int    | Puntuación usada para ordenar el ranking       | `699155` |

Ejemplo de documento:

```json
{
  "name": "Franco",
  "level": 7,
  "time": 845,
  "won": false,
  "score": 699155
}
```

## 3. Cálculo de la puntuación

La puntuación se calcula en `RankingMenu.CalculateScore()`:

```
score = level * 100000 - time
si won es true: score = score + 100000
```

- **El nivel es lo que más cuenta.** Cada nivel vale 100000 puntos, así que quien llega más lejos siempre queda por encima.
- **El tiempo solo desempata.** Cada segundo resta 1 punto. Entre dos jugadores que llegan al mismo nivel, queda por encima el más rápido.
- **Ganar cuenta como un nivel más.** Así quien derrota al jefe queda por encima de quien muere contra él, aunque los dos hayan llegado al nivel 10.

Ejemplos:

| Partida                                   | Cálculo                          | Score   |
|-------------------------------------------|----------------------------------|---------|
| Muere en el nivel 7 a los 845 s           | 7 × 100000 − 845                 | 699155  |
| Gana al jefe (nivel 10) a los 1500 s      | 10 × 100000 − 1500 + 100000      | 1098500 |

Al guardar un único número ya calculado, Firestore puede ordenar el ranking por un solo campo, sin tener que crear índices compuestos.

## 4. Operaciones que hace el juego

Todas están en `Assets/Scripts/UIScripts/RankingMenu.cs`.

| Operación | Cuándo                                                       | Código                                                       |
|-----------|--------------------------------------------------------------|--------------------------------------------------------------|
| Insertar  | El jugador escribe su nombre y pulsa **Enviar** en la pantalla de Game Over o de Victoria | `db.Collection("scores").AddAsync(entry)` |
| Consultar | Al abrir la pantalla y justo después de enviar una partida   | `db.Collection("scores").OrderByDescending("score").Limit(10).GetSnapshotAsync()` |

El juego **no modifica ni borra** partidas. Además, las reglas de seguridad tampoco lo permiten (ver el apartado 5).

Si no hay conexión a Internet, el ranking muestra "Sin conexion" y, si falla el envío, "No se ha podido enviar". En ese caso el botón **Enviar** se vuelve a activar para intentarlo otra vez.

## 5. Validación de los datos

Los datos se validan dos veces.

### 5.1. En el juego (cliente)

Antes de enviar nada, `RankingMenu.IsValidName()` comprueba el nombre escrito en el campo de texto:

1. Quita los espacios del principio y del final (`Trim`).
2. Comprueba que tiene entre **3 y 10 caracteres**.
3. Comprueba que todos los caracteres son **letras o números** (`char.IsLetterOrDigit`).

Si el nombre no es válido, no se envía y aparece el mensaje *"El nombre debe tener de 3 a 10 letras o numeros"*.

Además, el botón **Enviar** se desactiva después de enviar, para que la misma partida no se suba dos veces.

### 5.2. En Firestore (servidor)

Las reglas de seguridad de Firestore también validan los datos. Así, el ranking queda protegido aunque alguien intente escribir en la base de datos sin pasar por el juego:

```
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /scores/{score} {
      allow read: if true;
      allow create: if request.resource.data.name is string
                    && request.resource.data.name.size() >= 3
                    && request.resource.data.name.size() <= 10
                    && request.resource.data.score is int;
    }
  }
}
```

- `allow read: if true` → cualquiera puede consultar el ranking.
- `allow create` → solo se pueden crear partidas nuevas, con un nombre de 3 a 10 caracteres y una puntuación entera.
- Como `update` y `delete` no aparecen, **están prohibidos**: nadie puede cambiar ni borrar partidas desde fuera de la consola.
- Cualquier otra colección distinta de `scores` queda cerrada.

## 6. Creación y datos iniciales

### 6.1. Crear la base de datos

En Firestore la colección no hace falta crearla a mano: **se crea sola** cuando el juego envía la primera partida. Por eso no hay un script SQL de creación.

Pasos para montar la base de datos desde cero en un proyecto nuevo de Firebase:

1. En la [consola de Firebase](https://console.firebase.google.com), crear un proyecto.
2. Ir a **Firestore Database → Crear base de datos** y elegir una ubicación.
3. En la pestaña **Reglas**, pegar las reglas del apartado 5.2 y pulsar **Publicar**.
4. Registrar la aplicación (Android o iOS) para descargar `google-services.json` y colocarlo en `Assets/`. El SDK genera a partir de él `Assets/StreamingAssets/google-services-desktop.json`, que es el que usa la versión de Windows.

### 6.2. Dejar el ranking a cero

1. Consola de Firebase → **Firestore Database → Datos**.
2. Pulsar los tres puntos junto a la colección `scores` → **Borrar colección**.
3. Escribir `scores` para confirmar y pulsar **Borrar**.

Las reglas no se borran. La colección vuelve a aparecer con la siguiente partida enviada.

Para borrar una sola partida (por ejemplo, una de prueba con un nombre erróneo), se pulsa sobre el documento y luego **Eliminar documento**.

### 6.3. Datos de ejemplo

El ranking que se entrega contiene:

- **Partidas de prueba** jugadas por mí con la versión de Windows, con derrotas en distintos niveles y al menos una victoria.
- **Partidas de jugadores de prueba** (amigos que probaron el juego), que sirven también como prueba con usuarios reales.

Todas se han subido desde el propio juego, así que pasan por la misma validación que cualquier partida normal.

## 7. Acceso

- **El juego no tiene usuarios ni contraseñas.** El ranking es anónimo: cada jugador escribe un alias al terminar la partida.
- Para consultar la base de datos, el profesorado tendrá acceso de **lector (Viewer)** al proyecto de Firebase, que se dará con su correo en la entrega.

## 8. Datos guardados en local (PlayerPrefs)

Las mejoras permanentes se guardan en el PC del jugador con `PlayerPrefs`. En Windows, Unity las guarda en el registro:

- Versión de Windows (.exe): `HKEY_CURRENT_USER\Software\FrancGermanCerullo\Hyrulike`
- Editor de Unity: `HKEY_CURRENT_USER\Software\Unity\UnityEditor\FrancGermanCerullo\Hyrulike`

Para reiniciar el progreso basta con borrar esa clave, por ejemplo desde PowerShell:

```powershell
Remove-Item -Path "HKCU:\Software\FrancGermanCerullo\Hyrulike" -Recurse
```

## 9. Privacidad

Solo se guarda un **alias** elegido por el jugador, junto con datos de la partida. No se recogen correos, contraseñas ni ningún dato personal, así que el ranking no trata datos personales en el sentido del RGPD.
