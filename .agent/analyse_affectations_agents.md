# Analyse des Affectations des Agents

## Date: 28 Décembre 2025

## Problème Identifié

Le CRUD agent actuel ne prend **PAS** en compte toutes les affectations nécessaires lors de la création ou modification d'un agent.

### Structure de la Base de Données

#### Tables d'Affectation pour Agents Administratifs
1. **t_service_agent** : Affectation d'un agent à un service
   - `num_affect` (PK, AUTO_INCREMENT)
   - `fk_service` (FK → t_services.id_service)
   - `fk_agent` (FK → t_agents.matricule)
   - `date_affect` (DATE)
   - Contrainte unique: `uk_tsa` sur (fk_service, fk_agent, date_affect)

2. **t_grade_agent** : Affectation d'un grade à un agent
   - `num_affect` (PK, AUTO_INCREMENT)
   - `fk_grade` (FK → t_grade.id_grade)
   - `fk_agent` (FK → t_agents.matricule)
   - `date_affect` (DATE)
   - Contrainte unique: `uk_tga` sur (fk_grade, fk_agent, date_affect)

3. **t_roles_agents** : Affectation d'un rôle à un agent
   - `num_affect` (PK, AUTO_INCREMENT)
   - `fk_role` (FK → t_roles.id_role)
   - `fk_agent` (FK → t_agents.matricule)
   - `date_affect` (DATE)
   - Contrainte unique: `uk_tra` sur (fk_role, fk_agent, date_affect)

#### Table d'Affectation pour Agents Professeurs
4. **t_affect_prof** : Affectation d'un professeur à une promotion
   - `num` (PK, AUTO_INCREMENT)
   - `id_prof` (FK → t_agents.matricule)
   - `cod_promo` (FK → t_promotions.cod_promo)
   - `annee_scol` (VARCHAR(10))

### Tables de Référence
- **t_services** : Liste des services (Administration, Enseignement, etc.)
- **t_grade** : Liste des grades (Licencié, Gradué, etc.)
- **t_roles** : Liste des rôles système (Enseignant, Directeur, etc.)
- **t_promotions** : Liste des promotions/classes

## État Actuel du Code

### ✅ Ce qui fonctionne
1. **FormAffectAgent.cs** : Gère correctement l'enregistrement des affectations
   - Désactive les affectations précédentes (UPDATE actif = 0)
   - Insère les nouvelles affectations dans t_service_agent, t_grade_agent, t_roles_agents
   - Utilise des transactions pour garantir la cohérence

2. **Agents.cs** : Contient les méthodes CRUD pour t_affect_prof
   - `CreateAffectProf()` : Crée une affectation professeur
   - `GetAffectProfByPromotion()` : Récupère les affectations par promotion
   - `GetAffectProfByAgent()` : Récupère les affectations d'un agent
   - `DeleteAffectProf()` : Supprime une affectation

3. **Administrations.cs** : Contient les méthodes CRUD pour les affectations administratives
   - Méthodes pour t_service_agent
   - Méthodes pour t_grade_agent
   - Méthodes pour t_roles_agents

### ❌ Ce qui manque

1. **FormMain.cs - BtnSaveAgent_Click()**
   - Vérifie que `_selectedServiceAgent` n'est pas vide
   - **MAIS** n'enregistre PAS les affectations dans les tables
   - Les variables `_selectedServiceAgent`, `_selectedGradeAgent`, `_selectedRoleAgent` sont stockées mais jamais utilisées

2. **FormMain.cs - BtnUpdateAgent_Click()**
   - Même problème que pour la création
   - Les affectations ne sont pas mises à jour

3. **Workflow incomplet**
   - L'utilisateur doit cliquer sur "Affecter" pour ouvrir FormAffectAgent
   - FormAffectAgent enregistre les affectations
   - **MAIS** si l'utilisateur clique sur "Enregistrer" sans avoir cliqué sur "Affecter", les affectations ne sont pas enregistrées

4. **Pas de gestion des affectations professeurs**
   - Aucun formulaire pour affecter un professeur à une promotion
   - Les méthodes existent dans Agents.cs mais ne sont pas utilisées

## Solutions Proposées

### Solution 1 : Enregistrement Automatique des Affectations (RECOMMANDÉE)

Modifier `BtnSaveAgent_Click()` et `BtnUpdateAgent_Click()` pour enregistrer automatiquement les affectations après la création/modification de l'agent.

**Avantages:**
- Workflow simplifié
- Garantit que les affectations sont toujours enregistrées
- Cohérence des données

**Inconvénients:**
- Nécessite de modifier le code existant

### Solution 2 : Rendre l'Affectation Obligatoire

Forcer l'utilisateur à passer par FormAffectAgent avant de pouvoir enregistrer l'agent.

**Avantages:**
- Séparation claire des responsabilités
- Validation explicite des affectations

**Inconvénients:**
- Workflow plus long
- Expérience utilisateur moins fluide

### Solution 3 : Affectation Intégrée dans FormMain

Intégrer les contrôles d'affectation directement dans FormMain au