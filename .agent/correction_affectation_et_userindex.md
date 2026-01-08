# Correction: Problème d'affectation et UserIndex manquant dans le matricule

**Date:** 28 décembre 2025  
**Problèmes identifiés:**
1. L'affectation n'était pas enregistrée correctement
2. Le matricule généré ne contenait pas le UserIndex de l'utilisateur

## Problème 1: Affectation non enregistrée

### Message d'erreur
```
L'élève a été créé mais l'affectation n'a pas pu être enregistrée. 
Vous pouvez l'affecter manuellement plus tard.
```

### Cause racine
La méthode `CreateAffectationRecord` dans `FormMain.cs` insérait directement dans la table `t_affectation` sans utiliser le service `Administrations` qui gère l'isolation par école.

**Code problématique (AVANT):**
```csharp
private async Task<bool> CreateAffectationRecord(string matricule, string codePromotion,
    string anneeScolaire, string indicePromotion)
{
    try
    {
        using (var conn = Connexion.Instance.GetConnection())
        {
            var insertQuery = @"
                INSERT INTO t_affectation (matricule, cod_promo, annee_scol, indice_promo)
                VALUES (@Matricule, @CodPromo, @AnneeScolaire, @IndicePromo)";
            
            var result = await conn.ExecuteAsync(insertQuery, parameters);
            return result > 0;
        }
    }
    // ...
}
```

### Solution implémentée

**Code corrigé (APRÈS):**
```csharp
private async Task<bool> CreateAffectationRecord(string matricule, string codePromotion,
    string anneeScolaire, string indicePromotion)
{
    try
    {
        // Use the Administrations service which handles school isolation
        var success = await Task.Run(() => _administrations.CreateAffectation(
            matricule: matricule,
            codPromo: codePromotion,
            anneeScol: anneeScolaire,
            indicePromo: indicePromotion
        ));

        return success;
    }
    // ...
}
```

**Avantages:**
- Utilise le service `Administrations.CreateAffectation()` qui gère l'isolation par école
- Respecte l'architecture du système
- Garantit que l'affectation est liée à la bonne école

## Problème 2: UserIndex manquant dans le matricule

### Format attendu vs Format généré

| Attendu | Généré (AVANT) | Problème |
|---------|----------------|----------|
| `ELV{userIndex}{number}{year}` | `ELV{year}{number}` | UserIndex manquant |
| Exemple: `ELV00100000000012025` | Exemple: `ELV2025000001` | Pas de traçabilité utilisateur |

### Cause racine 1: UserIndex codé en dur

Dans `SaveEleveToDatabase`, le userIndex était codé en dur à "001":

**Code problématique (AVANT):**
```csharp
var studentCreated = _elevesService.CreateEleve(
    // ...
    userIndex: "001", // Default user index
    // ...
);
```

**Code corrigé (APRÈS):**
```csharp
// Get the current user index for matricule generation
var userIndex = UserContext.CurrentUserIndex;

var studentCreated = _elevesService.CreateEleve(
    // ...
    userIndex: userIndex,
    // ...
);
```

### Cause racine 2: GenerateUniqueMatricule n'utilisait pas le userIndex

La méthode `GenerateUniqueMatricule` dans `Eleves.cs` recevait le `userIndex` en paramètre mais ne l'utilisait pas dans la génération.

**Code problématique (AVANT):**
```csharp
private string GenerateUniqueMatricule(IDbConnection conn, string userIndex)
{
    var year = DateTime.Now.Year.ToString();
    var query = "SELECT MAX(...) FROM t_eleves WHERE matricule LIKE @Pattern";
    var maxNum = conn.QueryFirstOrDefault<int?>(query, new { Pattern = $"ELV{year}%" }) ?? 0;
    var newNum = maxNum + 1;
    var matricule = $"ELV{year}{newNum:D6}";  // ← userIndex non utilisé
    return matricule;
}
```

**Code corrigé (APRÈS):**
```csharp
private string GenerateUniqueMatricule(IDbConnection conn, string userIndex)
{
    var year = DateTime.Now.Year.ToString();
    
    // Rechercher le dernier numéro pour ce userIndex et cette année
    var pattern = $"ELV{userIndex}%{year}";
    var query = "SELECT MAX(CAST(SUBSTRING(matricule, LENGTH(@Prefix) + 1, 10) AS UNSIGNED)) 
                 FROM t_eleves WHERE matricule LIKE @Pattern";
    
    var maxNum = conn.QueryFirstOrDefault<int?>(query, new 
    { 
        Pattern = pattern,
        Prefix = $"ELV{userIndex}"
    }) ?? 0;
    
    var newNum = maxNum + 1;
    var radical = newNum.ToString().PadLeft(10, '0');
    var matricule = $"ELV{userIndex}{radical}{year}";  // ← Format correct
    return matricule;
}
```

## Format du matricule corrigé

### Structure
```
ELV + {userIndex} + {radical sur 10 chiffres} + {année}
```

### Exemples
- Utilisateur 001, 1er élève de 2025: `ELV00100000000012025`
- Utilisateur 002, 5e élève de 2025: `ELV00200000000052025`
- Utilisateur 001, 100e élève de 2025: `ELV00100000001002025`

### Avantages
- **Traçabilité:** Chaque matricule identifie l'utilisateur qui l'a créé
- **Unicité:** Combinaison userIndex + numéro séquentiel + année
- **Isolation:** Chaque utilisateur a sa propre séquence de numéros
- **Audit:** Possibilité de retrouver qui a créé un élève

## Modifications effectuées

### Fichier: Layouts/FormMain.cs

1. **SaveEleveToDatabase:**
   - Utilisation de `UserContext.CurrentUserIndex` au lieu de "001"

2. **CreateAffectationRecord:**
   - Utilisation de `_administrations.CreateAffectation()` au lieu d'insertion directe

### Fichier: Csharp/Admins/Eleves.cs

1. **GenerateUniqueMatricule:**
   - Intégration du `userIndex` dans le format du matricule
   - Recherche du dernier numéro par userIndex et année
   - Format: `ELV{userIndex}{radical}{year}`

## Tests recommandés

1. ✅ Se connecter avec un utilisateur ayant un user_index spécifique
2. ✅ Créer un nouvel élève
3. ✅ Vérifier que le matricule contient le user_index (ex: ELV00100000000012025)
4. ✅ Affecter l'élève à une promotion
5. ✅ Enregistrer l'élève
6. ✅ Vérifier que l'élève ET l'affectation sont enregistrés
7. ✅ Vérifier dans la base que l'affectation existe dans t_affectation
8. ✅ Créer un deuxième élève et vérifier l'incrémentation du numéro

## Impact

- **Traçabilité:** Amélioration significative avec l'identification de l'utilisateur créateur
- **Affectation:** Fonctionne maintenant correctement avec isolation par école
- **Compatibilité:** Le nouveau format de matricule est compatible avec la base de données
- **Sécurité:** Respect de l'architecture d'isolation par école
