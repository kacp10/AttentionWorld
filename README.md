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

## 📊 Estructura de Base de Datos

### Tabla: `PlayerData`
- `PlayerID`: ID del jugador
- `Name`, `Role`, `Classroom`, `Email`, `ParentID`, `YearOfBirth`

### Tabla: `GameResults`
- `PlayerID`
- `GameStamp`, `PlayDate`, `GameName`, `CognitiveArea`
- `Score`, `CorrectCount`, `IncorrectCount`
- `ItemType`: `SingleGame` o `DailySummary`

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
