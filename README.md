# 🧠 AttentionWorld

**AttentionWorld** es un videojuego educativo desarrollado en **Unity** que busca fortalecer la atención, memoria, lógica y cálculo en niños con Trastorno por Déficit de Atención (TDAH). El sistema se adapta dinámicamente al rol del usuario (Niño, Padre, Profesor o Administrador), ofreciendo experiencias y reportes personalizados.

---

## 🎯 Objetivos del Proyecto

- Estimular habilidades cognitivas clave en niños con TDAH.
- Ofrecer reportes automáticos para padres y docentes.
- Gamificar el aprendizaje en un entorno accesible y tecnológico.
- Evaluar rendimiento diario y registrar historial de juego.

---

## 🧩 Minijuegos Incluidos

| Juego                | Área Cognitiva | Descripción                                      |
|---------------------|----------------|--------------------------------------------------|
| Pelotas Saltarinas  | Atención       | Identificar cuántas pelotas rebotan.            |
| Parejas             | Memoria        | Encontrar cartas iguales en el menor tiempo.     |
| Cálculo Divertido   | Cálculo        | Resolver operaciones matemáticas básicas.        |
| Rompecabezas        | Lógica         | Completar el puzzle antes de que acabe el tiempo.|

Cada minijuego tiene 5 rondas progresivas. El sistema registra puntajes, aciertos y errores.

---

## 🧑‍🏫 Roles del Sistema

- 👶 **Niños**: Juegan minijuegos asignados o libres, y su rendimiento se guarda automáticamente.
- 👨‍👩‍👧 **Padres**: Visualizan el historial y progreso de sus hijos.
- 👩‍🏫 **Profesores**: Asignan juegos diarios, gestionan salones y revisan progreso de estudiantes.
- 🧑‍💼 **Administrador**: Accede a un panel con estadísticas generales (usuarios, juegos, promedios, etc).

---

## 💾 Tecnologías Usadas

- 🎮 Unity 2021.3.x
- ☁️ AWS DynamoDB (NoSQL)
- 🔐 AWS Cognito (autenticación)
- 📊 Unity XCharts (gráficos)
- 💻 C# (backend y lógica de negocio)

---

## 📊 Estructura de Base de Datos

El sistema utiliza **3 tablas principales** en AWS DynamoDB:

### 🧍‍♂️ Tabla: `PlayerData`

Contiene los datos de registro de todos los usuarios.

| Campo        | Tipo     | Descripción                                                    |
|--------------|----------|----------------------------------------------------------------|
| `PlayerID`   | Cadena   | ID único del jugador (clave primaria)                          |
| `Name`       | Cadena   | Nombre completo del usuario                                    |
| `Role`       | Cadena   | Rol: `Child`, `Parents`, `Teacher`                             |
| `Classroom`  | Cadena   | Salón asignado (solo niños y profesores)                       |
| `Email`      | Cadena   | Correo electrónico del usuario                                 |
| `ParentID`   | Cadena   | ID del hijo (solo si el usuario es `Parent`)                   |
| `YearOfBirth`| Cadena   | Año de nacimiento (solo niños)                                 |

---

### 🎮 Tabla: `GameResults`

Registra resultados por minijuego y día.

| Campo           | Tipo     | Descripción                                                      |
|-----------------|----------|------------------------------------------------------------------|
| `PlayerID`      | Cadena   | ID del jugador                                                   |
| `GameStamp`     | Cadena   | Formato `YYYY-MM-DD#IDX` o `YYYY-MM-DD#SUMMARY`                 |
| `PlayDate`      | Cadena   | Fecha del juego (`YYYY-MM-DD`)                                  |
| `GameName`      | Cadena   | Nombre del minijuego                                             |
| `CognitiveArea` | Cadena   | Área evaluada: atención, memoria, cálculo o lógica              |
| `Score`         | Número   | Puntaje obtenido                                                 |
| `CorrectCount`  | Número   | Aciertos (si aplica)                                             |
| `IncorrectCount`| Número   | Errores (si aplica)                                              |
| `ItemType`      | Cadena   | `SingleGame` o `DailySummary`                                   |

---

### 📅 Tabla: `DailyAssignments`

Define qué juegos debe completar cada niño por día.

| Campo        | Tipo                  | Descripción                                                  |
|--------------|------------------------|--------------------------------------------------------------|
| `PlayerID`   | Cadena                | ID del niño asignado                                         |
| `Date`       | Cadena                | Fecha de la asignación (`YYYY-MM-DD`)                        |
| `Classroom`  | Cadena                | Salón del estudiante                                         |
| `Games`      | Conjunto de cadenas   | Juegos asignados (ej: `"GameSceneMath", "PuzzleScene"`)     |
| `TeacherID`  | Cadena                | ID del profesor que hizo la asignación                       |

---

## 🧠 Funcionalidades Clave

- Autenticación por rol (`Child`, `Parents`, `Teacher`, `Admin`)
- Evaluación automática y persistencia en DynamoDB
- Dashboard de administración con conteo total de:
  - Usuarios registrados
  - Usuarios recientes
  - Juegos jugados
  - Promedio diario
- Visualización de progreso por jugador
- Asignación de juegos diarios por salón
- Vista de perfil editable (correo/teléfono)

---

## 🧪 Métricas y Evaluación

- ⏱️ Medición de tiempos de respuesta con `Stopwatch`.
- 📋 Seguimiento de tareas en pruebas de usabilidad.
- 🧾 Exportación local de resultados (`LoginTestResults.txt`, `GameProgressTestResults.txt`).

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
│ ├── UserSession.cs
│ ├── GameSessionData.cs
│ ├── ResultSceneManager.cs
│ ├── SummarySceneManager.cs
│ ├── AdminDashboardManager.cs
├── Resources/
│ ├── Sprites/
│ ├── Icons/
│ └── Backgrounds/

yaml
Copiar
Editar

---

## 🔐 Autenticación

Se gestiona a través de **AWS Cognito**, con persistencia en el objeto `UserSession.cs`. Cada rol es redirigido a su escena correspondiente al iniciar sesión.

---

## ✅ Estado del Proyecto

- [x] Sistema de login/logout
- [x] Minijuegos funcionales y evaluables
- [x] Registro de resultados por día
- [x] Dashboard de estadísticas para admin
- [x] Asignación dinámica de juegos por salón
- [x] Perfil de usuario editable (simulado)

---

## 📩 Contacto

**Desarrollador:** Kevin Andres Castro  
**Correo:** kacastro15@ucatolica.edu.co

---

## ⚠️ Licencia

Este proyecto fue desarrollado con fines **educativos**. No está autorizado para uso comercial sin previa autorización escrita.
