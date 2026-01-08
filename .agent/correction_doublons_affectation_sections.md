# Correction: Prévention des doublons d'affectation de sections

**Date:** 28 décembre 2025  
**Objectif:** Empêcher qu'une même section soit affectée deux fois à la même école

## Problème identifié

Le système permettait d'affecter plusieurs fois la même section à une école, ce qui créait des doublons dans la table `t_affect_sect`.

## Solutions implémentées

### 1. Validation au niveau applicatif (C#)

**Fichier modifié:** `Csharp/Admins/Administrations.cs`

Ajout d'une vérification dans la méthode `CreateAffectSection` :
- Vérification si la combinaison (id_ecole, cod_sect) existe déjà
- Lancement d'une exception `InvalidOperationException` si un doublon est détecté
- Message d'erreur clair: "Cette section est déjà affectée à cette école."

```csharp
// Vérifier si cette section est déjà affectée à cette école
var checkQuery = "SELECT COUNT(*) FROM t_affect_sect WHERE id_ecole = @IdEcole AND cod_sect = @CodSect";
var exists = conn.ExecuteScalar<int>(checkQuery, new { IdEcole = idEcole, CodSect = codSect });

if (exists > 0)
{
    throw new InvalidOperationException("Cette section est déjà affectée à cette école.");
}
```

### 2. Contrainte au niveau base de données (SQL)

**Fichier modifié:** `DataSets/ecole_db.sql`

Ajout d'une contrainte UNIQUE sur la table `t_affect_sect` :
```sql
UNIQUE KEY `uk_affect_sect_ecole_section` (`id_ecole`, `cod_sect`)
```

Cette contrainte garantit l'unicité au niveau de la base de données, même en cas d'accès direct.

### 3. Script de migration

**Fichier créé:** `DataSets/migration_add_unique_constraint_affect_sect.sql`

Script pour mettre à jour les bases de données existantes :
1. Supprime les doublons existants (garde la première affectation)
2. Ajoute la contrainte UNIQUE
3. Vérifie le succès de la migration

## Gestion des erreurs

Les formulaires existants gèrent déjà correctement les exceptions :
- `FormAffectSection.cs` : Affiche le message d'erreur dans un MessageBox
- `FormAdmins.cs` : Affiche le message d'erreur dans un MessageBox

## Instructions d'application

### Pour les nouvelles installations
Le schéma mis à jour sera automatiquement appliqué.

### Pour les bases de données existantes
Exécuter le script de migration :
```bash
mysql -u [user] -p [database] < DataSets/migration_add_unique_constraint_affect_sect.sql
```

## Tests recommandés

1. Tenter d'affecter une section déjà affectée à une école
2. Vérifier que le message d'erreur s'affiche correctement
3. Vérifier que la liste des affectations ne contient pas de doublons
4. Tester avec plusieurs écoles différentes

## Impact

- **Sécurité:** Protection contre les doublons au niveau applicatif ET base de données
- **Performance:** Impact minimal (une requête SELECT supplémentaire avant l'insertion)
- **Compatibilité:** Aucun changement d'interface utilisateur requis
- **Données existantes:** Le script de migration nettoie automatiquement les doublons
