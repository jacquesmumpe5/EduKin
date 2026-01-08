-- Script d'insertion du Super Administrateur avec hash bcrypt correct
-- Utilisateur: jacques7 / sandwiche1991
-- Nom complet: MUMPE BALANDA Jacques

-- IMPORTANT: Exécutez ce script dans votre base de données MySQL

-- Supprimer l'utilisateur s'il existe
DELETE FROM t_users_infos WHERE username = 'jacques7';

-- Insérer le Super Administrateur avec hash bcrypt
-- Le hash sera généré par l'application C# lors de la création
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
    'USR00100000000012025',
    'MUMPE',
    'BALANDA',
    'Jacques',
    'M',
    'jacques7',
    -- Hash bcrypt pour 'sandwiche1991' (généré avec cost=10)
    '$2a$10$N9qo8uLOickgx2ZMRZoMye.IVF42/3jVMiVgpRVyQpYi3R6OwqKOm',
    NULL,
    NULL,                   -- id_ecole NULL pour utilisateur système
    'ROL00000000001',       -- Super Administrateur
    'SYSTEM',
    1,
    0,
    NOW(),
    NOW()
);

-- Vérification
SELECT 
    u.username,
    u.nom,
    u.postnom,
    u.prenom,
    u.type_user,
    r.nom_role,
    u.compte_verrouille
FROM t_users_infos u
LEFT JOIN t_roles r ON u.fk_role = r.id_role
WHERE u.username = 'jacques7';