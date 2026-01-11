# Analyse du projet EduKin

## 1) Type d’application & dépendances

- **Type**: application **Windows Forms** (WinExe) en **.NET 8** (`net8.0-windows10.0.22000`).
- **Libs clés** (NuGet):
  - **Dapper** (accès DB)
  - **MySql.Data** (MySQL)
  - **System.Data.SQLite.Core** (SQLite offline)
  - **BCrypt.Net-Next** (hash/verify mot de passe)
  - **OpenCvSharp** (caméra / capture)
  - **GemBox.Spreadsheet**, **itext** (exports Excel/PDF)
  - **Siticone.Desktop.UI** (UI)

## 2) Points d’entrée & flux de démarrage UI

- **Entry point**: `Inits/Program.cs` → `Application.Run(new FormStart())`.

### FormStart (bootstrap)

- Charge `school_config.json` via `SchoolConfigManager`.
- Si pas de config → navigation vers **FormConfig**.
- Si config présente:
  - charge les infos de l’école via `Administrations.GetEcoleForInitialization(idEcole)`
  - initialise le contexte global:
    - `EduKinContext.InitializeSchool(idEcole, denomination)`
  - navigation vers **FormLogin**.
- S’abonne à `Connexion.ConnectionChanged` pour détecter la bascule online/offline.

### FormConfig (configuration école)

- Test connexion MySQL via `Connexion.TestConnection()`.
- Si MySQL KO: propose de continuer en **SQLite** (offline) et initialise les vues SQLite via `SQLiteInitializer.InitializeViews()`.
- Recherche d’avenues/écoles via des **vues**:
  - `vue_avenue_hierarchie`
  - `vue_ecole`
- Sélection école existante:
  - sauvegarde `school_config.json`
  - `EduKinContext.Initialize(idEcole, denomination)`
  - initialise l’année scolaire active via `SchoolYearManager.InitializeContextWithActiveYear(...)`.

### FormLogin (authentification)

- Valide `EduKinContext.IsConfigured`.
- Authentifie via requête SQL sur `t_users_infos` + `t_roles`.
- Vérifie le mot de passe:
  - **BCrypt** en premier
  - fallback **SHA256** en cas d’échec.
- Initialise le contexte utilisateur:
  - `EduKinContext.InitializeUser(userId, username, roleName, userIndex)`
- Ouvre **FormMain**.

### FormMain (écran principal)

- Vérifie `EduKinContext.IsAuthenticated` et `EduKinContext.IsConfigured`.
- Utilise des services métier (`Eleves`, `Agents`, `Administrations`, etc.) pour charger les données et exécuter les actions.

## 3) Couche données & persistance (MySQL + SQLite)

### Connexion & bascule online/offline

- `DataSets/Connexion.cs` est un **singleton**:
  - `GetConnection()` retourne **MySqlConnection** si `_isOnline`, sinon **SQLiteConnection**.
  - monitoring automatique (Timer toutes les 10s) + event `ConnectionChanged`.
  - fallback SQLite après plusieurs échecs MySQL + initialisation tables:
    - `_sync_metadata`
    - `_connection_logs`

> Important: `Connexion.cs` contient une chaîne MySQL **hardcodée** (port `3309`, mot de passe en clair), alors que `appsettings.json` contient une autre config (port `3306`). `appsettings.json` n’est donc pas la source de vérité actuellement.

### SQLiteInitializer

- Crée les vues SQLite `vue_avenue_hierarchie` et `vue_ecole`.
- Crée tables d’auth offline minimales:
  - `t_roles`
  - `t_users_infos`

### Sync & offline

- `SyncManager` + `OfflineModeManager`:
  - auto-sync périodique (5 min) quand MySQL revient.
  - synchronisation bidirectionnelle par tables (`t_eleves`, `t_agents`, `t_affectation`, etc.).
  - détection conflits via timestamps / checksum.
  - génération d’IDs uniques offline (préfixe + userIndex + séquence + année).

### Couche “service” (CRUD) & isolation multi-écoles

- Les services héritent de `BaseService`.
- `BaseService` fournit:
  - `QueryWithIsolation`, `InsertWithIsolation`, `UpdateWithIsolation`, `DeleteWithIsolation`
  - **Isolation par école** via `EduKinContext.AddIsolationClause(query)` qui ajoute `... AND id_ecole = @IdEcole`.
  - retry automatique en cas d’erreurs DB (lock/timeouts).
- Adaptation SQL MySQL/SQLite via `SqlCompatibilityAdapter`.

## 4) Contexte global (état application)

- `Inits/EduKinContext.cs` centralise:
  - **École** (id, dénomination)
  - **Année scolaire** (active/clôturée, dates)
  - **Utilisateur** (id, username, role, userIndex)
  - Méthodes d’isolation SQL (`AddIsolationClause`, `GetIsolationParameters`)
  - Helpers de rôles (ex `IsAdmin()`, `HasRole()`).

## 5) Modules fonctionnels (par dossiers)

- **Layouts/**: WinForms (Start, Config, Login, Main, dialogues, affectations…)
- **Csharp/Admins/**:
  - `Administrations`: génération d’IDs via SP `sp_generate_id` en MySQL ou algo local en SQLite; gestion promotions/options/sections/écoles, etc.
  - `Eleves`, `Agents`: CRUD + listes + stats + affectations (avec isolation + compat offline).
- **Csharp/Securites/**:
  - `SessionManager`: sessions + permissions (tables `t_roles_permissions`, `t_permissions`, `t_sessions`, etc.) + hash SHA256 interne.
  - `UsersEvents`: login + audit log + CRUD users (SHA256 + SP d’ID).
  - `GestionRolesPermissions`: CRUD rôles/permissions + assignations.
- **Csharp/Finances/**:
  - `Paiements.cs` (module finances).
- **DataSets/**:
  - connexion, offline, sync, compat SQL.
- **Inits/**:
  - bootstrap, contexte global, config école.

## 6) Risques techniques / incohérences

- **Secrets en clair**:
  - `appsettings.json` contient un mot de passe MySQL en clair.
  - `Connexion.cs` contient aussi un mot de passe en clair.
- **Configuration MySQL incohérente**:
  - `Connexion.cs` utilise `Port=3309`.
  - `appsettings.json` et les messages d’erreur parlent de `3306`.
- **Deux systèmes d’authentification/hachage**:
  - `FormLogin` utilise BCrypt (+ fallback SHA256).
  - `SessionManager` / `UsersEvents` utilisent SHA256.
  - Risque: utilisateurs créés via un module ne se connectent pas via l’autre.
- **SQL non portable (NOW())**:
  - certaines requêtes dans l’UI utilisent `NOW()` directement (risque en SQLite si non adapté).
- **Isolation `id_ecole` pas uniforme**:
  - certaines tables n’ont pas `id_ecole` et nécessitent une isolation par jointure.
- **Couplage UI ↔ DB**:
  - SQL directement dans les Forms (`FormLogin`, `FormConfig`), ce qui complique tests et maintenance.

## 7) Améliorations recommandées (priorité)

- **Haute**: centraliser la configuration DB (utiliser `appsettings.json` ou variables d’environnement) et supprimer le hardcode.
- **Haute**: unifier le hash des mots de passe (idéalement **BCrypt** partout) et prévoir migration.
- **Haute**: sécuriser les secrets (ne pas committer le mot de passe DB).
- **Moyenne**: adapter toutes les requêtes UI au mode SQLite (via `SqlCompatibilityAdapter` ou services).
- **Moyenne**: clarifier et documenter la stratégie d’isolation multi-écoles.
- **Moyenne**: réduire le couplage UI/DB (déplacer SQL dans les services).
