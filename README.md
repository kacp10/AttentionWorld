# 🧠 AttentionWorld

**AttentionWorld** es un juego interactivo educativo desarrollado en **Unity** que busca fortalecer la atención, la memoria, la lógica y el cálculo en niños con Trastorno por Déficit de Atención (TDAH). El sistema se adapta dinámicamente al rol del usuario (Niño, Padre o Profesor), ofreciendo diferentes experiencias y reportes personalizados.

---

## 🎯 Objetivos del Proyecto

- Mejorar las habilidades cognitivas en niños con TDAH.
- Ofrecer herramientas de monitoreo para padres y profesores.
- Crear una experiencia divertida, inclusiva y tecnológica para el aprendizaje.
- Facilitar el seguimiento del progreso a través de reportes automáticos.

---

## 🧩 Minijuegos Incluidos

| Juego                | Área Cognitiva | Descripción                                |
|---------------------|----------------|--------------------------------------------|
| Pelotas Saltarinas  | Atención       | Detectar la cantidad de pelotas que rebotan. |
| Parejas             | Memoria        | Encontrar parejas de cartas.                |
| Cálculo Divertido   | Cálculo        | Resolver operaciones matemáticas.           |
| Rompecabezas        | Lógica         | Encajar piezas antes de que termine el tiempo. |

Cada minijuego tiene 5 rondas progresivas y mide resultados que luego se almacenan en la base de datos para seguimiento.

---

## 🧑‍🏫 Roles del Sistema

- 👶 **Niños**: Juegan los minijuegos asignados o en modo libre. Los resultados se registran automáticamente.
- 👨‍👩‍👧 **Padres**: Pueden ver el progreso de sus hijos, actualizar su perfil y consultar estadísticas.
- 👩‍🏫 **Profesores**: Asignan juegos, consultan resultados por salón y visualizan tableros de rendimiento.
- 🧑‍💼 **Administrador**: Tiene acceso a un *Dashboard* donde puede ver estadísticas globales como:
  - Total de usuarios registrados
  - Nuevos usuarios recientes
  - Juegos jugados
  - Promedio diario de juego

---

## 💾 Tecnologías Usadas

- 🎮 Unity 2021.3.x (motor del juego)
- ☁️ AWS DynamoDB (base de datos NoSQL)
- 🔐 AWS Cognito (gestión de usuarios y autenticación)
- 📈 Unity XCharts (para visualización de progreso)
- 🧠 C# (lógica de negocio)

---

📊 Estructura de Base de Datos
Este sistema utiliza AWS DynamoDB como base de datos NoSQL. A continuación se describen las tres tablas principales que lo conforman:

🧍‍♂️ Tabla: PlayerData
Contiene la información de cada usuario registrado en el sistema.

Campo	Tipo	Descripción
PlayerID	Cadena	ID único del jugador (clave primaria)
Name	Cadena	Nombre del usuario
Role	Cadena	Rol del usuario: Child, Parents, Teacher
Classroom	Cadena	Salón asignado (niños y profesores)
Email	Cadena	Correo del usuario
ParentID	Cadena	ID del hijo (solo si el usuario es padre)
YearOfBirth	Cadena	Año de nacimiento (solo si el usuario es niño)

🎮 Tabla: GameResults
Registra los resultados obtenidos en los minijuegos.

Campo	Tipo	Descripción
PlayerID	Cadena	ID del jugador que realizó el juego
GameStamp	Cadena	Formato: YYYY-MM-DD#IDX o YYYY-MM-DD#SUMMARY
PlayDate	Cadena	Fecha del juego (YYYY-MM-DD)
GameName	Cadena	Nombre del minijuego jugado
CognitiveArea	Cadena	Área cognitiva evaluada: atención, memoria, lógica o cálculo
Score	Número	Puntaje total obtenido en el juego
CorrectCount	Número	Número de respuestas correctas (si aplica)
IncorrectCount	Número	Número de respuestas incorrectas (si aplica)
ItemType	Cadena	SingleGame o DailySummary, según el tipo de registro

📅 Tabla: DailyAssignments
Define qué juegos debe realizar cada niño, asignados por el profesor para una fecha determinada.

Campo	Tipo	Descripción
PlayerID	Cadena	ID del niño que recibió los juegos
Date	Cadena	Fecha de la asignación (YYYY-MM-DD)
Classroom	Cadena	Salón del jugador asignado
Games	Conjunto de cadenas	Lista de escenas/juegos asignados (ej: "GameSceneMath")
TeacherID	Cadena	ID del profesor que hizo la asignación

---

## 🧠 Funcionalidades Clave

- Sistema de **login seguro** con distintos flujos según el rol.
- Carga dinámica de **juegos asignados** desde base de datos.
- **Evaluación automática** de resultados y guardado en tiempo real.
- Pantallas de **resultados personalizados** con feedback visual (cerebro llenándose).
- Dashboard de **admin con estadísticas globales**.
- Escena de perfil para cada rol, con opciones de edición simulada.

---

## 🧪 Métricas de Usabilidad y Rendimiento

- ⏱️ Tiempos de carga registrados con `Stopwatch`.
- 📋 Seguimiento de tareas (`tracker?.StartTask("T2")`) para análisis de experiencia.
- 🧾 Resultados guardados localmente para validaciones posteriores (`LoginTestResults.txt` / `GameProgressTestResults.txt`).

---

## 📂 Estructura del Proyecto

Assets/
├── Scenes/
│ ├── LoginScene
│ ├── HomeChildScene
│ ├── HomeParentsScene
│ ├── HomeTeacherScene
│ ├── AdminScene
│ └── MiniGameScenes/
├── Scripts/
│ ├── LoginManager.cs
│ ├── GameSessionData.cs
│ ├── ResultSceneManager.cs
│ ├── SummarySceneManager.cs
│ ├── AdminDashboardManager.cs
│ └── UserSession.cs
└── Resources/
└── Sprites, Icons, Backgrounds

yaml
Copiar
Editar

---

## 🔐 Autenticación

La autenticación se realiza usando **AWS Cognito**, con sesiones persistentes administradas desde el script `UserSession.cs`. Cada rol navega a su respectiva escena de inicio tras el login.

---

## ✅ Estado del Proyecto

- [x] Flujo completo de login y logout
- [x] Implementación de 4 minijuegos funcionales
- [x] Registro y visualización de resultados por juego y por día
- [x] Dashboard de administración
- [x] Visualización de progreso en perfiles
- [x] Integración con AWS DynamoDB

---

## 📩 Contacto

Desarrollado por: **Kevin Andres Castro**  
Correo: kacastro15@ucatolica.edu.co

---

## ⚠️ Licencia

Este proyecto es de uso académico y educativo. No está destinado para uso comercial sin autorización previa.
