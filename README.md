# AttentionWorld

AttentionWorld es un videojuego educativo desarrollado en Unity orientado al fortalecimiento de habilidades cognitivas como la atención, la memoria, la lógica y el cálculo.

El proyecto está pensado principalmente para niños con Trastorno por Déficit de Atención e Hiperactividad (TDAH), e incluye diferentes tipos de usuario para facilitar el seguimiento del progreso, la asignación de actividades y la consulta de resultados.

## Objetivo

El objetivo principal de AttentionWorld es ofrecer un entorno interactivo y gamificado que permita trabajar diferentes habilidades cognitivas mediante minijuegos.

El sistema también busca facilitar el seguimiento del rendimiento mediante el almacenamiento de resultados, reportes de progreso y asignaciones diarias realizadas por profesores.

## Minijuegos

Actualmente el proyecto incluye cuatro minijuegos principales:

| Juego | Área cognitiva | Descripción |
| --- | --- | --- |
| Pelotas Saltarinas | Atención | El jugador debe identificar cuántas pelotas están rebotando. |
| Parejas | Memoria | Consiste en encontrar pares de cartas iguales en el menor tiempo posible. |
| Cálculo Divertido | Cálculo | El jugador debe resolver operaciones matemáticas básicas. |
| Rompecabezas | Lógica | El objetivo es completar un rompecabezas antes de que finalice el tiempo. |

Cada minijuego contiene cinco rondas progresivas. Durante la ejecución se registran datos como puntaje, aciertos y errores.

## Roles del sistema

AttentionWorld maneja diferentes roles de usuario:

### Niño

Puede acceder a los minijuegos disponibles, completar actividades asignadas y generar resultados que se almacenan automáticamente.

### Padre

Puede consultar el historial y el progreso asociado a su hijo.

### Profesor

Puede administrar salones, asignar juegos diarios y revisar el rendimiento de los estudiantes.

### Administrador

Tiene acceso a un panel general con estadísticas del sistema, incluyendo cantidad de usuarios, juegos registrados y promedios generales.

## Tecnologías utilizadas

El proyecto utiliza las siguientes tecnologías:

- Unity 2021.3.x
- C#
- AWS DynamoDB
- AWS Cognito
- Unity XCharts

Unity se utiliza como motor principal del videojuego, mientras que C# contiene la lógica de negocio y comportamiento de las escenas.

AWS Cognito gestiona la autenticación de usuarios y DynamoDB almacena información de perfiles, resultados y asignaciones.

## Base de datos

La persistencia del sistema se organiza principalmente en tres tablas de DynamoDB.

### PlayerData

Almacena la información de registro de los usuarios.

| Campo | Tipo | Descripción |
| --- | --- | --- |
| `PlayerID` | String | Identificador único del usuario. |
| `Name` | String | Nombre completo. |
| `Role` | String | Rol del usuario: `Child`, `Parents` o `Teacher`. |
| `Classroom` | String | Salón asignado para niños y profesores. |
| `Email` | String | Correo electrónico. |
| `ParentID` | String | Identificador utilizado para relacionar un padre con el niño correspondiente. |
| `YearOfBirth` | String | Año de nacimiento del niño. |

### GameResults

Almacena los resultados obtenidos durante las partidas.

| Campo | Tipo | Descripción |
| --- | --- | --- |
| `PlayerID` | String | Identificador del jugador. |
| `GameStamp` | String | Identificador con formato `YYYY-MM-DD#IDX` o `YYYY-MM-DD#SUMMARY`. |
| `PlayDate` | String | Fecha de la partida en formato `YYYY-MM-DD`. |
| `GameName` | String | Nombre del minijuego. |
| `CognitiveArea` | String | Área cognitiva evaluada. |
| `Score` | Number | Puntaje obtenido. |
| `CorrectCount` | Number | Cantidad de respuestas correctas. |
| `IncorrectCount` | Number | Cantidad de respuestas incorrectas. |
| `ItemType` | String | Tipo de registro: `SingleGame` o `DailySummary`. |

### DailyAssignments

Almacena las actividades asignadas por los profesores.

| Campo | Tipo | Descripción |
| --- | --- | --- |
| `PlayerID` | String | Identificador del estudiante. |
| `Date` | String | Fecha de la asignación. |
| `Classroom` | String | Salón del estudiante. |
| `Games` | String Set | Conjunto de juegos asignados. |
| `TeacherID` | String | Identificador del profesor que realizó la asignación. |

Un ejemplo del campo `Games` puede ser:

```text
GameSceneMath
PuzzleScene
```

## Funcionalidades principales

Entre las principales funcionalidades implementadas se encuentran:

- Inicio y cierre de sesión.
- Autenticación por rol mediante AWS Cognito.
- Redirección del usuario según su tipo de cuenta.
- Registro automático de resultados en DynamoDB.
- Consulta de historial de juego.
- Seguimiento de puntajes, aciertos y errores.
- Asignación diaria de minijuegos.
- Gestión de estudiantes por salón.
- Visualización de progreso por jugador.
- Dashboard administrativo con estadísticas generales.
- Perfil de usuario editable.
- Registro de métricas utilizadas en pruebas de usabilidad.

## Autenticación y sesión

La autenticación se realiza mediante AWS Cognito.

Una vez que el usuario inicia sesión, la información necesaria se mantiene durante la ejecución mediante `UserSession.cs`.

Dependiendo del rol detectado, el sistema carga la escena correspondiente:

- `Child` → Home del niño.
- `Parents` → Home del padre.
- `Teacher` → Home del profesor.
- `Admin` → Panel administrativo.

## Registro de resultados

Los resultados de cada minijuego son procesados y almacenados en DynamoDB.

El sistema puede registrar información individual de cada juego y también generar resúmenes diarios mediante los tipos:

```text
SingleGame
DailySummary
```

Esto permite consultar tanto resultados específicos como información agregada del rendimiento diario.

## Métricas y pruebas

Durante las pruebas de usabilidad se registran tiempos de respuesta y progreso de determinadas tareas.

Para la medición de tiempo se utiliza `Stopwatch` de C#.

Algunos resultados de prueba pueden almacenarse localmente en archivos como:

```text
LoginTestResults.txt
GameProgressTestResults.txt
```

Estos registros se utilizaron para analizar el comportamiento del sistema durante las pruebas.

## Estructura general del proyecto

```text
Assets/
├── Scenes/
│   ├── LoginScene
│   ├── HomeChildScene
│   ├── HomeParentsScene
│   ├── HomeTeacherScene
│   ├── AdminScene
│   └── MiniGameScenes/
│
├── Scripts/
│   ├── LoginManager.cs
│   ├── UserSession.cs
│   ├── GameSessionData.cs
│   ├── ResultSceneManager.cs
│   ├── SummarySceneManager.cs
│   └── AdminDashboardManager.cs
│
└── Resources/
    ├── Sprites/
    ├── Icons/
    └── Backgrounds/
```

## Estado del proyecto

Actualmente se encuentran implementadas las siguientes funcionalidades:

- Sistema de autenticación.
- Gestión de sesión por usuario.
- Minijuegos funcionales.
- Evaluación y registro de resultados.
- Historial de rendimiento.
- Resumen diario.
- Dashboard administrativo.
- Asignación de juegos por salón.
- Gestión básica de perfiles.

## Autor

**Kevin Andres Castro**

Correo de contacto: `kacastro15@ucatolica.edu.co`
Repositorio Universidad Católica de Colombia: https://repository.ucatolica.edu.co/entities/publication/689bf9ad-341a-438e-bd0a-0fbe36018964

## Licencia

Este proyecto fue desarrollado con fines académicos y educativos.

No está autorizado su uso comercial sin autorización previa del autor.
