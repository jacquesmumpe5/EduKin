# Correction: Erreur "Unknown column 'created_at' in 'field list'" lors de l'affectation

**Date:** 28 décembre 2025  
**Problème:** Erreur lors de la création d'une affectation d'élève à une promotion

## Message d'erreur complet

```
System.InvalidOperationException: 
'Erreur lors de l'opération 'CreateAffectation' pour l'école COLLEGE OASIS: 
Erreur non récupérable lors de l'opération 'InsertWithIsolation: t_affectation' pour l'école COLLEGE OASIS: 
Unknown column 'created_at' in 'field list''
```

## Analyse du problème

### Cause racine
La méthode `CreateAffectation` dans `Administrations.cs` utilisait `InsertWithIsolation` qui ajoute automatiquement les colonnes `created_at` et `updated_at` à toutes les insertions.

**Problème:** La table `t_affectation` ne possède pas ces colonnes de timestamps.

### Structure de la table t_affectation

```sql
CREATE TABLE IF NOT EXISTS `t_affectation` (
  `id_affect` int NOT NULL AUTO_INCREMENT,
  `matricule` varchar(50) NOT NULL,
  `cod_promo` varchar(50) NOT NULL,
  `annee_scol` varchar(10) NOT NULL,
  `indice_promo` varchar(10) NOT NULL,
  PRIMARY KEY (`id_affect`),
  -- ❌ Pas de colonnes created_at/updated_at
  ...
);
```

### Code problématique (AVANT)

```csharp
public bool CreateAffectation(string matricule, string codPromo, string anneeScol, string indicePromo)
{
    return ExecuteWithErrorHandling(() =>
    {
        var affectationData = new
        {
            matricule = matricule,
            cod_promo = codPromo,
            annee_scol = anneeScol,
            indice_promo = indicePromo
        };

        // ❌ InsertWithIsolation ajoute automatiquement created_at et updated_at
        var result = InsertWithIsolation("t_affectation", affectationData);
        return result > 0;
    }, "CreateAffectation");
}
```

**Requête SQL générée (qui échoue):**
```sql
INSERT INTO t_affectation 
(matricule, cod_promo, annee_scol, indice_promo, created_at, updated_at)
VALUES (@matricule, @cod_promo, @annee_scol, @indice_promo, @created_at, @updated_at)
-- ❌ Erreur: Unknown column 'created_at' in 'field list'
```

## Solution implémentée

### Code corrigé (APRÈS)

**Fichier modifié:** `Csharp/Admins/Administrations.cs`

```csharp
public bool CreateAffectation(string matricule, string codPromo, string anneeScol, string indicePromo)
{
    return ExecuteWithErrorHandling(() =>
    {
        using (var conn = GetSecureConnection())
        {
            // Note: t_affectation n'a pas de colonnes created_at/updated_at
            // donc on fait une insertion directe sans utiliser InsertWithIsolation
            var query = @"INSERT INTO t_affectation (matricule, cod_promo, annee_scol, indice_promo) 
                          VALUES (@matricule, @cod_promo, @annee_scol, @indice_promo)";
            
            var parameters = new
            {
                matricule = matricule,
                cod_promo = codPromo,
                annee_scol = anneeScol,
                indice_promo = indicePromo
            };
            
            var result = conn.Execute(query, parameters);
            return result > 0;
        }
    }, "CreateAffectation");
}
```

**Requête SQL générée (qui fonctionne):**
```sql
INSERT INTO t_affectation 
(matricule, cod_promo, annee_scol, indice_promo)
VALUES (@matricule, @cod_promo, @annee_scol, @indice_promo)
-- ✅ Succès: Colonnes correspondant exactement à la structure de la table
```

## Différences entre les approches

| Aspect | InsertWithIsolation | Insertion directe |
|--------|---------------------|-------------------|
| Timestamps | ✅ Ajoute automatiquement | ❌ N'ajoute pas |
| Isolation école | ✅ Ajoute id_ecole | ❌ N'ajoute pas |
| Flexibilité | ❌ Impose une structure | ✅ Contrôle total |
| Usage | Tables avec timestamps | Tables sans timestamps |

## Pourquoi ne pas utiliser InsertWithIsolation ?

### Raisons techniques

1. **Structure de table incompatible:** `t_affectation` n'a pas de colonnes `created_at` et `updated_at`
2. **Pas besoin d'isolation par école:** L'isolation se fait via la relation `t_affectation` → `t_eleves` → `t_ecoles`
3. **Table de liaison simple:** C'est une table de relation many-to-many entre élèves et promotions

### Isolation par école

L'isolation par école pour les affectations est garantie par :
1. La clé étrangère `matricule` qui référence `t_eleves`
2. Les élèves sont déjà isolés par école via leur création
3. Donc les affectations héritent automatiquement de cette isolation

## Tables avec et sans timestamps

### Tables AVEC timestamps (utilisent InsertWithIsolation)
- `t_eleves` : ✅ created_at, updated_at
- `t_agents` : ✅ created_at, updated_at
- `t_ecoles` : ✅ created_at, updated_at (si ajoutées)

### Tables SANS timestamps (insertion directe)
- `t_affectation` : ❌ Pas de timestamps
- `t_affect_sect` : ❌ Pas de timestamps
- `t_promotions` : ❌ Pas de timestamps
- `t_options` : ❌ Pas de timestamps
- `t_sections` : ❌ Pas de timestamps

## Flux de création d'élève corrigé

1. L'utilisateur remplit les informations de l'élève
2. L'utilisateur affecte l'élève à une promotion
3. L'utilisateur clique sur "Enregistrer"
4. **Création de l'élève** dans `t_eleves` (avec timestamps)
5. **Création de l'affectation** dans `t_affectation` (sans timestamps) ✅
6. **Succès:** "Élève créé avec succès!"

## Tests recommandés

1. ✅ Créer un nouvel élève avec tous les champs remplis
2. ✅ Affecter l'élève à une promotion
3. ✅ Enregistrer l'élève
4. ✅ Vérifier que l'élève est créé dans `t_eleves`
5. ✅ Vérifier que l'affectation est créée dans `t_affectation`
6. ✅ Vérifier qu'aucune erreur de colonne manquante n'apparaît
7. ✅ Vérifier que l'élève apparaît dans la liste avec sa promotion

## Impact

- **Correction critique:** L'enregistrement des affectations fonctionne maintenant
- **Compatibilité:** Respecte la structure existante de la base de données
- **Performance:** Aucun impact (même nombre de requêtes)
- **Maintenabilité:** Code plus clair avec commentaire explicatif
- **Isolation:** Maintenue via la relation avec t_eleves
