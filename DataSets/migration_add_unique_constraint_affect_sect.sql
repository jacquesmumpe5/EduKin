-- Migration: Ajout de contrainte UNIQUE sur t_affect_sect
-- Date: 2025-12-28
-- Description: Empêche qu'une même section soit affectée deux fois à la même école

-- Vérifier et supprimer les doublons existants avant d'ajouter la contrainte
-- Garder uniquement la première affectation pour chaque combinaison (id_ecole, cod_sect)
DELETE t1 FROM t_affect_sect t1
INNER JOIN t_affect_sect t2 
WHERE t1.num_affect > t2.num_affect 
  AND t1.id_ecole = t2.id_ecole 
  AND t1.cod_sect = t2.cod_sect;

-- Ajouter la contrainte UNIQUE
ALTER TABLE `t_affect_sect` 
ADD UNIQUE KEY `uk_affect_sect_ecole_section` (`id_ecole`, `cod_sect`);

-- Vérification
SELECT 'Migration terminée avec succès' AS status;
