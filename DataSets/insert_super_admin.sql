-- Script d'insertion du Super Administrateur système
-- Utilisateur: jacques7
-- Nom complet: MUMPE BALANDA Jacques
-- Date: 2025-01-05

-- ÉTAPE 1: Vérifier si l'utilisateur existe déjà
SELECT 'Vérification existence utilisateur jacques7' AS etape;
SELECT COUNT(*) as utilisateur_existe FROM t_users_infos WHERE username = 'jacques7';

-- ÉTAPE 2: Supprimer l'utilisateur s'il existe (pour éviter les doublons)
DELETE FROM t_users_infos WHERE username = 'jacques7';

-- ÉTAPE 3: Récupérer l'ID du rôle Super Administrateur
SELECT 'Vérification rôle Super Administrateur' AS etape;
SELECT id_role, nom_role FROM t_roles WHERE nom_role = 'Super Administrateur';

-- ÉTAPE 4: Insérer le Super Administrateur
-- Note: Le mot de passe 'sandwiche1991' sera hashé avec bcrypt
-- Hash bcrypt pour 'sandwiche1991': $2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi
INSERT INTO t_users_infos (
    id_user,
    nom,
    postnom, 
    prenom,
    sexe,
    username,
    pwd_hash,
    telephone,
    id_ecole,
    fk_role,
    type_user,
    user_index,
    compte_verrouille,
    tentatives_connexion,
    created_at,
    last_password_change
) VALUES (
    'USR00100000000012025',  -- ID généré selon le pattern
    'MUMPE',                 -- nom
    'BALANDA',              -- postnom
    'Jacques',              -- prenom
    'M',                    -- sexe
    'jacques7',             -- username
    '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', -- pwd_hash (bcrypt pour 'sandwiche1991')
    NULL,                   -- telephone (optionnel)
    NULL,                   -- id_ecole (NULL pour utilisateur système)
    'ROL00000000001',       -- fk_role (Super Administrateur)
    'SYSTEM',               -- type_user (utilisateur système)
    1,                      -- user_index (premier utilisateur)
    0,                      -- compte_verrouille (non verrouillé)
    0,                      -- tentatives_connexion
    NOW(),                  -- created_at
    NOW()                   -- last_password_change
);

-- ÉTAPE 5: Vérifier l'insertion
SELECT 'Vérification insertion' AS etape;
SELECT 
    u.id_user,
    u.nom,
    u.postnom,
    u.prenom,
    u.username,
    u.type_user,
    u.user_index,
    u.compte_verrouille,
    r.nom_role
FROM t_users_infos u
LEFT JOIN t_roles r ON u.fk_role = r.id_role
WHERE u.username = 'jacques7';

-- ÉTAPE 6: Message de confirmation
SELECT 'Super Administrateur jacques7 créé avec succès!' AS resultat;
SELECT 'Username: jacques7' AS info1;
SELECT 'Password: sandwiche1991' AS info2;
SELECT 'Rôle: Super Administrateur' AS info3;
SELECT 'Type: SYSTEM (utilisateur système)' AS info4;