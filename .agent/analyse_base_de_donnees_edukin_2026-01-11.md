# Analyse de la base de données EduKin

> Sources analysées:
>
>- `DataSets/ecole_db.sql` (schéma MySQL + procédures + données de référence)
>- `DataSets/SQLiteInitializer.cs` (vues SQLite + tables d’auth offline)
>- `DataSets/SyncManager.cs` / `DataSets/OfflineModeManager.cs` (sync + génération d’IDs offline + scripts de tables SQLite)
>- `DataSets/SchemaValidator.cs` (validation/migration de schéma)
>- Code applicatif (requêtes Dapper dans `FormLogin`, `Administrations`, `Eleves`, `Agents`, etc.)

## 1) Moteurs & stratégie de stockage

- **MySQL** (mode en ligne): schéma principal décrit dans `ecole_db.sql`.
- **SQLite** (mode hors-ligne): base locale `ecole_local.db`.
  - la structure n’est pas une copie complète 1:1 de MySQL.
  - l’application crée/assure certaines tables et vues minimales (via `Connexion.EnsureSQLiteDatabase`, `SQLiteInitializer`, `SyncManager.CreateSQLiteTables`, `SchemaValidator.MigrateSQLiteSchema`).

## 2) Stratégie d’identifiants (IDs)

### IDs “métier” (format texte)

Une grande partie des IDs fonctionnels sont des `VARCHAR`/`TEXT` avec un format type:

- `PREFIX + user_index(3) + radical(10) + year(4)`
- Exemple commenté dans le code SQL: `ELV00100000000012025`

### Génération MySQL: `sp_generate_id`

- Procédure stockée `sp_generate_id(p_table, p_column, p_prefix, p_user_index, OUT p_new_id)`.
- Assure l’unicité via un **verrou applicatif MySQL** (`GET_LOCK`) par `(table, colonne, user_index)`.
- Extrait le plus grand radical, l’incrémente, puis concatène.

### Génération SQLite (offline)

- Plusieurs implémentations:
  - `Administrations.GenerateLocalId(...)` / `GenerateLocalIdForNewSchool(...)`.
  - `SyncManager.GenerateUniqueId(...)` et `OfflineModeManager.GenerateUniqueId(...)`.
- Contrainte: sans verrou distribué, l’unicité dépend de la discipline + sync + user_index.

## 3) Tables principales (domaine scolaire)

### `t_ecoles`

- Clé primaire: `id_ecole` (varchar).
- Colonnes clés: `denomination`, `FkAvenue` (FK), `numero`, `logo`.
- FK:
  - `FkAvenue_Ecole` → `t_entite_administrative(IdEntite)`.

### `t_entite_administrative`

- Référentiel géographique hiérarchique.
- Clé primaire: `IdEntite`.
- Champs: `Fk_EntiteMere`, `Fk_TypeEntite`, `Etat`, `IsVisible`.
- Très volumineuse (dans `ecole_db.sql`: ~116k lignes).

### Affectation école → section

- `t_sections` (référentiel sections)
- `t_affect_sect`:
  - PK: `num_affect` (int auto increment)
  - FK `id_ecole` → `t_ecoles.id_ecole`
  - FK `cod_sect` → `t_sections.cod_sect`
  - Unique: `(id_ecole, cod_sect)`

### Options & promotions (structure pédagogique)

- `t_options`: options rattachées à `t_sections` (`cod_sect`).
- `t_promotions`: promotions rattachées à `t_options` (`cod_opt`).
- Tables d’affectation:
  - `t_affect_options` (association section-affectée → option)

### Élèves & affectations

- `t_eleves`:
  - PK: `matricule` (varchar)
  - champs: identité, adresse (`FkAvenue`, `numero`), tuteur, profil, timestamps.
  - indexes:
    - `FULLTEXT idx_eleves_nom (nom, postnom, prenom)`
    - `idx_eleves_sexe_date (sexe, date_naiss)`
    - `idx_eleves_tuteur (nom_tuteur)`

- `t_affectation`:
  - PK: `id_affect` (int auto increment)
  - FK `matricule` → `t_eleves.matricule`
  - FK `cod_promo` → `t_promotions.cod_promo`
  - champs: `annee_scol`, `indice_promo`

> Remarque “isolation par école”: `t_eleves` ne porte pas `id_ecole` dans le schéma MySQL. L’app déduit l’appartenance à une école via la chaîne:
>
>`t_eleves` → `t_affectation` → `t_promotions` → `t_options` → `t_sections` → `t_affect_sect(id_ecole)`

### Agents & affectations

- `t_agents`:
  - PK: `matricule` (varchar)
  - FK `id_ecole` → `t_ecoles.id_ecole`
  - champs: identité, adresse, salaires, profil, timestamps.
  - contraintes `CHECK` (email, sexe, salaires >= 0…)

- `t_affect_prof`:
  - PK: `num` (int auto increment)
  - FK `id_prof` → `t_agents.matricule`
  - FK `cod_promo` → `t_promotions.cod_promo`

- `t_affect_cours`:
  - PK: `num` (int auto increment)
  - FK `id_cours` → `t_cours.id_cours`
  - FK `cod_promo` → `t_promotions.cod_promo`

## 4) Années scolaires

- `t_annee_scolaire`:
  - PK: `id_annee` (int auto increment)
  - `id_ecole` (varchar) + `code_annee` (ex: `2025-2026`)
  - flags: `est_active`, `est_cloturee`.

Le code (`SchoolYearManager`, `EduKinContext`) impose la règle:

- **une seule année active par école**
- une année clôturée est considérée comme **non modifiable** côté application.

## 5) Sécurité / authentification / permissions

### `t_users_infos`

- PK: `id_user` (varchar).
- Champs attendus côté app: `username`, `pwd_hash`, `fk_role`, `type_user`, `id_ecole`, `compte_verrouille`, `account_locked_until`, `user_index`, timestamps.
- Particularité: certaines parties du code comparent `pwd_hash` comme SHA256, d’autres vérifient BCrypt (voir section risques).

### RBAC

- `t_roles`:
  - PK: `id_role` (varchar)
  - `nom_role`, `description_role`, etc.

- `t_permissions`:
  - PK: `id_permission` (varchar)
  - `nom_permission`, `description_permission`, etc.

- `t_roles_permissions`:
  - PK: `id_role_permission` (varchar)
  - FK `fk_role` → `t_roles.id_role`
  - FK `fk_permission` → `t_permissions.id_permission`
  - champ `accordee` (0/1)

### Sessions / audit

Selon le code, l’app utilise aussi (si présentes en base):

- `t_sessions`
- `t_session_events`
- `t_user_events`

## 6) Finances (aperçu)

Le script SQL montre des tables typiques:

- `t_paiement` (référence par `t_entree.num_recu`)
- `t_entree` (FK vers école + paiement)
- `t_frais`, `t_type_frais`
- `t_caisse`, etc.

## 7) Vues importantes

### En SQLite

Créées par `SQLiteInitializer`:

- `vue_avenue_hierarchie`: jointure hiérarchique sur `t_entite_administrative` (Avenue → Quartier → Commune → Ville → Province).
- `vue_ecole`: jointure `t_ecoles` + `t_entite_administrative` pour exposer les colonnes d’affichage.

### En MySQL

`ecole_db.sql` contient aussi des vues (ex: `vue_roles_permissions`, et d’autres vues selon le script complet).

## 8) Synchronisation MySQL ⇄ SQLite (offline)

- Le mécanisme de sync vise principalement des tables “opérationnelles”:
  - élèves/agents/affectations/cours/grilles/finances…
- Détection de changements:
  - timestamps (`created_at`, `updated_at`) si disponibles
  - sinon fallback checksum/chargement complet.
- Risque structurel: certaines tables du schéma MySQL n’existent pas forcément en SQLite au même niveau de détail.

## 9) Points de risque / incohérences (BD)

- **Configuration MySQL non centralisée**: paramètres DB (port/mot de passe) divergent entre `appsettings.json` et `Connexion.cs`.
- **Hash mots de passe non unifié**:
  - BCrypt utilisé dans `FormLogin`.
  - SHA256 utilisé dans `UsersEvents`/`SessionManager`.
  - Risque direct: `t_users_infos.pwd_hash` peut être incompatible selon le module qui a créé le compte.
- **Portabilité MySQL → SQLite**:
  - `NOW()`, `ENUM`, `FULLTEXT`, certaines fonctions SQL et contraintes `CHECK` ne se traduisent pas automatiquement.
  - l’adaptation est partielle (via `SqlCompatibilityAdapter`), mais l’UI exécute parfois du SQL non adapté.
- **Isolation multi-écoles**:
  - côté schéma, `t_eleves` n’a pas `id_ecole` → isolation par jointures, plus fragile.
- **Vérifications de schéma**:
  - `SchemaValidator` peut aider, mais nécessite que les schémas attendus soient bien définis et maintenus.

## 10) Recommandations BD (concrètes)

- **Centraliser la configuration DB** (source unique) et supprimer le hardcode.
- **Standardiser `pwd_hash`**:
  - recommandation: BCrypt partout.
  - définir une stratégie de migration (ex: détecter format, re-hasher à la prochaine connexion).
- **Portabilité offline**:
  - faire passer toutes les requêtes UI par `SqlCompatibilityAdapter` ou par les services `BaseService`.
- **Isolation**:
  - soit ajouter `id_ecole` à `t_eleves` (migration),
  - soit imposer des requêtes toujours “anchored” par `t_affect_sect`/`id_ecole` + tests.
- **Schéma de sync**:
  - formaliser la liste des tables réellement synchronisées,
  - s’assurer que les colonnes nécessaires (`created_at`, `updated_at`) existent là où la sync l’exige.
