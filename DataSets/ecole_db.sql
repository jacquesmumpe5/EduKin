-- --------------------------------------------------------
-- Hôte:                         127.0.0.1
-- Version du serveur:           8.4.7 - MySQL Community Server - GPL
-- SE du serveur:                Win64
-- HeidiSQL Version:             12.14.0.7165
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Listage de la structure de la base pour ecole_db
CREATE DATABASE IF NOT EXISTS `ecole_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `ecole_db`;

-- Listage de la structure de procédure ecole_db. sp_generate_id
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_generate_id`(
    IN  p_table       VARCHAR(64),   -- nom de la table
    IN  p_column      VARCHAR(64),   -- nom de la colonne ID
    IN  p_prefix      VARCHAR(16),   -- préfixe ex: 'ELV'
    IN  p_user_index  VARCHAR(3),    -- indice utilisateur ex: '001'
    OUT p_new_id      VARCHAR(128)   -- résultat final
)
BEGIN
    DECLARE v_lock_name   VARCHAR(200);
    DECLARE v_locked      INT DEFAULT 0;
    DECLARE v_have_lock   TINYINT DEFAULT 0;
    DECLARE v_year        CHAR(4);
    DECLARE v_last_num    BIGINT DEFAULT 0;
    DECLARE v_next_num    BIGINT;
    DECLARE v_radical     VARCHAR(32);
    DECLARE v_pad_len     INT DEFAULT 10;  -- radical fixe à 10 chiffres
    DECLARE v_start_pos   INT;

    -- Handler d’erreur : libération du verrou si besoin
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        IF v_have_lock = 1 THEN
            DO RELEASE_LOCK(v_lock_name);
        END IF;
        RESIGNAL;
    END;

    -- Validation des paramètres
    IF p_table IS NULL OR p_table = ''
       OR p_column IS NULL OR p_column = ''
       OR p_prefix IS NULL OR p_prefix = ''
       OR p_user_index IS NULL OR p_user_index = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Paramètres invalides.';
    END IF;

    SET v_year = DATE_FORMAT(CURRENT_DATE(), '%Y');
    SET v_lock_name = CONCAT('genid:', p_table, ':', p_column, ':', p_user_index);

    -- Verrou applicatif pour ce poste uniquement
    SELECT GET_LOCK(v_lock_name, 10) INTO v_locked;
    IF v_locked <> 1 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Verrou de génération non obtenu.';
    END IF;
    SET v_have_lock = 1;

    -- Extraction du dernier numéro
    -- Exemple de format recherché : ELV00100000000012025
    SET @sql := CONCAT(
        'SELECT COALESCE(MAX(CAST(SUBSTRING(`', REPLACE(p_column,'`','``'), '` , ',
        CHAR_LENGTH(p_prefix) + CHAR_LENGTH(p_user_index) + 1, ', ',
        v_pad_len, ') AS UNSIGNED)), 0) INTO @tmp_last_num ',
        'FROM `', REPLACE(p_table,'`','``'), '` ',
        'WHERE `', REPLACE(p_column,'`','``'), '` REGEXP ''^', p_prefix, p_user_index, '[0-9]{10}[0-9]{4}$'' '
    );

    PREPARE stmt FROM @sql;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;

    SET v_last_num = COALESCE(@tmp_last_num, 0);
    SET v_next_num = v_last_num + 1;
    SET v_radical  = LPAD(CAST(v_next_num AS CHAR), v_pad_len, '0');

    SET p_new_id = CONCAT(p_prefix, p_user_index, v_radical, v_year);

    -- Libération du verrou
    DO RELEASE_LOCK(v_lock_name);
    SET v_have_lock = 0;
END//
DELIMITER ;

-- Listage de la structure de table ecole_db. t_affect_cours
CREATE TABLE IF NOT EXISTS `t_affect_cours` (
  `num` int NOT NULL AUTO_INCREMENT,
  `id_cours` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_promo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `periode_max` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `annee_scol` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `titulaire` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `tel_titulaire` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `indice` varchar(2) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `statut_examen` varchar(5) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`num`),
  KEY `idx_affect_cours_cod_promo` (`cod_promo`),
  KEY `idx_affect_cours_id_cours` (`id_cours`),
  CONSTRAINT `fk_affect_cours_cours` FOREIGN KEY (`id_cours`) REFERENCES `t_cours` (`id_cours`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_affect_cours_promo` FOREIGN KEY (`cod_promo`) REFERENCES `t_promotions` (`cod_promo`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_affect_cours : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_affect_options
CREATE TABLE IF NOT EXISTS `t_affect_options` (
  `num_affect_opt` int NOT NULL AUTO_INCREMENT,
  `num_affect_sect` int NOT NULL,
  `cod_opt` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_affect` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`num_affect_opt`),
  UNIQUE KEY `uk_affect_sect_opt` (`num_affect_sect`,`cod_opt`),
  KEY `idx_affect_options_sect` (`num_affect_sect`),
  KEY `idx_affect_options_opt` (`cod_opt`),
  CONSTRAINT `fk_affect_options_opt` FOREIGN KEY (`cod_opt`) REFERENCES `t_options` (`cod_opt`) ON DELETE CASCADE,
  CONSTRAINT `fk_affect_options_sect` FOREIGN KEY (`num_affect_sect`) REFERENCES `t_affect_sect` (`num_affect`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_affect_options : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_affect_prof
CREATE TABLE IF NOT EXISTS `t_affect_prof` (
  `num` int NOT NULL AUTO_INCREMENT,
  `id_prof` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_promo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `annee_scol` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`num`),
  KEY `idx_affect_prof_id_prof` (`id_prof`),
  KEY `idx_affect_prof_cod_promo` (`cod_promo`),
  CONSTRAINT `fk_affect_prof_agents` FOREIGN KEY (`id_prof`) REFERENCES `t_agents` (`matricule`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_affect_prof_promo` FOREIGN KEY (`cod_promo`) REFERENCES `t_promotions` (`cod_promo`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_affect_prof : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_affect_sect
CREATE TABLE IF NOT EXISTS `t_affect_sect` (
  `num_affect` int NOT NULL AUTO_INCREMENT,
  `id_ecole` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_sect` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_affect` date DEFAULT (curdate()),
  PRIMARY KEY (`num_affect`),
  UNIQUE KEY `uk_affect_sect_ecole_section` (`id_ecole`,`cod_sect`),
  KEY `idx_affect_sect_cod_sect` (`cod_sect`),
  KEY `idx_affect_sect_id_ecole` (`id_ecole`),
  CONSTRAINT `fk_affect_sect_ecoles` FOREIGN KEY (`id_ecole`) REFERENCES `t_ecoles` (`id_ecole`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_affect_sect_sections` FOREIGN KEY (`cod_sect`) REFERENCES `t_sections` (`cod_sect`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_affect_sect : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_affectation
CREATE TABLE IF NOT EXISTS `t_affectation` (
  `id_affect` int NOT NULL AUTO_INCREMENT,
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_promo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `annee_scol` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `indice_promo` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id_affect`),
  KEY `idx_affectation_matricule` (`matricule`),
  KEY `idx_affectation_cod_promo` (`cod_promo`),
  KEY `idx_affect_matricule` (`matricule`),
  KEY `idx_affect_cod_promo` (`cod_promo`),
  CONSTRAINT `fk_affectation_eleves` FOREIGN KEY (`matricule`) REFERENCES `t_eleves` (`matricule`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_affectation_promo` FOREIGN KEY (`cod_promo`) REFERENCES `t_promotions` (`cod_promo`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_affectation : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_agents
CREATE TABLE IF NOT EXISTS `t_agents` (
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `postnom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `prenom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `sexe` enum('M','F') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `lieu_naiss` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_naiss` date NOT NULL,
  `email` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tel` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FkAvenue` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Numero` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `sal_base` decimal(10,2) DEFAULT NULL,
  `ipr` decimal(10,2) DEFAULT NULL,
  `prime` decimal(10,2) DEFAULT NULL,
  `cnss` decimal(10,2) DEFAULT NULL,
  `sal_net` decimal(10,2) DEFAULT NULL,
  `id_ecole` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `profil` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`matricule`),
  KEY `idx_agents_nom` (`nom`),
  KEY `idx_agents_id_ecole` (`id_ecole`),
  KEY `idx_agents_sal_net` (`sal_net`),
  KEY `idx_agents_email` (`email`),
  KEY `idx_agents_sexe_service` (`sexe`) USING BTREE,
  KEY `FK_t_agents_t_roles` (`prime`) USING BTREE,
  KEY `FkAvenue` (`FkAvenue`),
  CONSTRAINT `fk_agents_ecoles` FOREIGN KEY (`id_ecole`) REFERENCES `t_ecoles` (`id_ecole`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `chk_agents_date_naiss` CHECK ((`date_naiss` >= _utf8mb4'1900-01-01')),
  CONSTRAINT `chk_agents_email` CHECK (((`email` is null) or (`email` like _utf8mb4'%@%.%'))),
  CONSTRAINT `chk_agents_ipr` CHECK (((`ipr` is null) or (`ipr` >= 0))),
  CONSTRAINT `chk_agents_sal_base` CHECK (((`sal_base` is null) or (`sal_base` >= 0))),
  CONSTRAINT `chk_agents_sal_net` CHECK (((`sal_net` is null) or (`sal_net` >= 0))),
  CONSTRAINT `chk_agents_sexe` CHECK ((`sexe` in (_utf8mb4'M',_utf8mb4'F')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_agents : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_annee_scolaire
CREATE TABLE IF NOT EXISTS `t_annee_scolaire` (
  `id_annee` int NOT NULL AUTO_INCREMENT,
  `id_ecole` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `code_annee` varchar(9) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_debut` date NOT NULL,
  `date_fin` date NOT NULL,
  `est_active` tinyint(1) DEFAULT '0',
  `est_cloturee` tinyint(1) DEFAULT '0',
  `date_creation` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_annee`),
  UNIQUE KEY `id_ecole` (`id_ecole`,`code_annee`),
  CONSTRAINT `t_annee_scolaire_chk_1` CHECK ((`date_debut` < `date_fin`))
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_annee_scolaire : 0 rows
/*!40000 ALTER TABLE `t_annee_scolaire` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_annee_scolaire` ENABLE KEYS */;

-- Listage de la structure de table ecole_db. t_caisse
CREATE TABLE IF NOT EXISTS `t_caisse` (
  `id_caisse` int NOT NULL AUTO_INCREMENT,
  `date_stock` date DEFAULT NULL,
  `montant` decimal(10,2) DEFAULT NULL,
  `id_ecole` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id_caisse`),
  KEY `idx_caisse_id_ecole` (`id_ecole`),
  CONSTRAINT `fk_caisse_ecoles` FOREIGN KEY (`id_ecole`) REFERENCES `t_ecoles` (`id_ecole`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_caisse : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_coupons
CREATE TABLE IF NOT EXISTS `t_coupons` (
  `num_coupon` int NOT NULL AUTO_INCREMENT,
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `periode` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `max_gen` int NOT NULL,
  `totaux` decimal(10,2) NOT NULL,
  `pourc` decimal(5,2) NOT NULL,
  `cod_promo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `annee_scol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`num_coupon`),
  KEY `idx_coupons_matricule` (`matricule`),
  CONSTRAINT `fk_coupons_eleves` FOREIGN KEY (`matricule`) REFERENCES `t_eleves` (`matricule`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_coupons : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_cours
CREATE TABLE IF NOT EXISTS `t_cours` (
  `id_cours` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `intitule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id_cours`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_cours : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_dettes
CREATE TABLE IF NOT EXISTS `t_dettes` (
  `id_dettes` int NOT NULL AUTO_INCREMENT,
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `motif` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `montant` decimal(10,2) DEFAULT NULL,
  `date_dette` date DEFAULT NULL,
  `mois` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `annee_scol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_dettes`),
  KEY `idx_dettes_matricule` (`matricule`),
  CONSTRAINT `fk_dettes_agents` FOREIGN KEY (`matricule`) REFERENCES `t_agents` (`matricule`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_dettes : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_ecoles
CREATE TABLE IF NOT EXISTS `t_ecoles` (
  `id_ecole` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `denomination` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `FkAvenue` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `logo` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `numero` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id_ecole`),
  KEY `idx_ecoles_adresse` (`FkAvenue`),
  CONSTRAINT `FkAvenue_Ecole` FOREIGN KEY (`FkAvenue`) REFERENCES `t_entite_administrative` (`IdEntite`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_ecoles : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_eleves
CREATE TABLE IF NOT EXISTS `t_eleves` (
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `postnom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `prenom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `sexe` enum('M','F') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `lieu_naiss` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `date_naiss` date DEFAULT NULL,
  `nom_tuteur` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `tel_tuteur` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FkAvenue` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `numero` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ecole_prov` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `profil` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`matricule`),
  KEY `idx_eleves_sexe_date` (`sexe`,`date_naiss`),
  KEY `idx_eleves_tuteur` (`nom_tuteur`),
  FULLTEXT KEY `idx_eleves_nom` (`nom`,`postnom`,`prenom`),
  CONSTRAINT `chk_eleves_date_naiss` CHECK (((`date_naiss` is null) or (`date_naiss` >= _utf8mb4'1900-01-01'))),
  CONSTRAINT `chk_eleves_sexe` CHECK ((`sexe` in (_utf8mb4'M',_utf8mb4'F'))),
  CONSTRAINT `chk_eleves_tel_tuteur` CHECK (((`tel_tuteur` is null) or (length(`tel_tuteur`) >= 8)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_eleves : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_entite_administrative
CREATE TABLE IF NOT EXISTS `t_entite_administrative` (
  `IdEntite` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IntituleEntite` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DenominationHabitant` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Fk_EntiteMere` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Fk_TypeEntite` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Etat` tinyint(1) DEFAULT '1',
  `IsVisible` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`IdEntite`),
  KEY `idx_ea_entite_mere` (`Fk_EntiteMere`),
  KEY `idx_ea_type` (`Fk_TypeEntite`),
  KEY `idx_intitule_entite` (`IntituleEntite`),
  KEY `idx_fk_entitemere` (`Fk_EntiteMere`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_entite_administrative : ~116 609 rows (environ)
INSERT INTO `t_entite_administrative` (`IdEntite`, `IntituleEntite`, `DenominationHabitant`, `Fk_EntiteMere`, `Fk_TypeEntite`, `Etat`, `IsVisible`) VALUES
	('ENA0000012019', 'République démocratique du congo', 'Congolaise (Kinshasa)', 'ENA0922802019', 'TEA00000000012019', 1, 1),
	('ENA0000012024', 'Kukenda', NULL, 'ENA0927092019', 'TEA00000000092019', 1, 1),
	('ENA0000022024', 'Lukunga', NULL, 'ENA0927092019', 'TEA00000000092019', 1, 1),
	('ENA0000032024', 'Lubudi', NULL, 'ENA0927092019', 'TEA00000000092019', 1, 1),
	('ENA0922802019', 'Afrique', NULL, 'ENA0922802019', 'TEA00000000142019', 1, 1),
	('ENA0922812019', 'Kinshasa', '', 'ENA0000012019', 'TEA00000000022019', 1, 1),
	('ENA0922822019', 'Kinshasa', '', 'ENA0922812019', 'TEA00000000032019', 1, 1),
	('ENA3286962025', 'Kabinda', '', 'ENA0923632019', 'TEA00000000062019', 1, 1);

-- Listage de la structure de table ecole_db. t_entree
CREATE TABLE IF NOT EXISTS `t_entree` (
  `id_entree` int NOT NULL AUTO_INCREMENT,
  `date_entree` date DEFAULT NULL,
  `num_recu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `libelle` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `montant` decimal(10,2) DEFAULT NULL,
  `annee_scol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_ecole` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id_entree`),
  KEY `idx_entree_date_entree` (`date_entree`),
  KEY `idx_entree_id_ecole` (`id_ecole`),
  KEY `idx_entree_annee_scol` (`annee_scol`),
  KEY `fk_entree_paiement` (`num_recu`),
  CONSTRAINT `fk_entree_ecoles` FOREIGN KEY (`id_ecole`) REFERENCES `t_ecoles` (`id_ecole`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_entree_paiement` FOREIGN KEY (`num_recu`) REFERENCES `t_paiement` (`num_recu`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_entree : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_frais
CREATE TABLE IF NOT EXISTS `t_frais` (
  `cod_frais` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `montant` decimal(10,2) NOT NULL,
  `cod_type_frais` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `modalite` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `periode` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `annee_scol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_ecole` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`cod_frais`),
  KEY `idx_frais_cod_orientation` (`cod_type_frais`) USING BTREE,
  CONSTRAINT `fk_frais_type` FOREIGN KEY (`cod_type_frais`) REFERENCES `t_type_frais` (`cod_type_frais`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_frais : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_grade
CREATE TABLE IF NOT EXISTS `t_grade` (
  `id_grade` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `code_grade` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `sigle` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `libelle_grade` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_grade`),
  UNIQUE KEY `uk_t_grade_code` (`code_grade`),
  UNIQUE KEY `uk_t_grade_sigle` (`sigle`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Listage des données de la table ecole_db.t_grade : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_grade_agent
CREATE TABLE IF NOT EXISTS `t_grade_agent` (
  `num_affect` int NOT NULL AUTO_INCREMENT,
  `fk_grade` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `fk_agent` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_affect` date NOT NULL,
  PRIMARY KEY (`num_affect`),
  UNIQUE KEY `uk_tga` (`fk_grade`,`fk_agent`,`date_affect`),
  KEY `idx_tga_fk_agent` (`fk_agent`),
  KEY `idx_tga_fk_grade` (`fk_grade`),
  KEY `idx_tga_date` (`date_affect`),
  CONSTRAINT `FK_t_grade_agent_t_agents` FOREIGN KEY (`fk_agent`) REFERENCES `t_agents` (`matricule`),
  CONSTRAINT `FK_t_grade_agent_t_grade` FOREIGN KEY (`fk_grade`) REFERENCES `t_grade` (`id_grade`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Listage des données de la table ecole_db.t_grade_agent : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_grilles
CREATE TABLE IF NOT EXISTS `t_grilles` (
  `num` int NOT NULL AUTO_INCREMENT,
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `periode` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `annee_scol` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cours` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `intitule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cotes` decimal(5,2) DEFAULT NULL,
  `maxima` decimal(5,2) DEFAULT NULL,
  `statut` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cod_promo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `indice` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`num`),
  KEY `idx_grilles_matricule` (`matricule`),
  KEY `idx_grilles_cod_promo` (`cod_promo`),
  KEY `idx_grilles_annee_scol` (`annee_scol`),
  KEY `idx_grilles_eleve_periode` (`matricule`,`periode`,`annee_scol`),
  KEY `idx_grilles_cours_periode` (`id_cours`,`periode`),
  CONSTRAINT `fk_grilles_eleves` FOREIGN KEY (`matricule`) REFERENCES `t_eleves` (`matricule`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `chk_grilles_cotes` CHECK (((`cotes` is null) or ((`cotes` >= 0) and (`cotes` <= `maxima`)))),
  CONSTRAINT `chk_grilles_maxima` CHECK (((`maxima` is null) or (`maxima` > 0))),
  CONSTRAINT `chk_grilles_statut` CHECK (((`statut` is null) or (`statut` in (_utf8mb4'ADMIS',_utf8mb4'ECHEC',_utf8mb4'ABSENT',_utf8mb4'REPORTE'))))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_grilles : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_options
CREATE TABLE IF NOT EXISTS `t_options` (
  `cod_opt` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_sect` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `code_epst` varchar(5) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`cod_opt`),
  UNIQUE KEY `code_epst` (`code_epst`),
  KEY `idx_options_cod_sect` (`cod_sect`),
  KEY `idx_opt_cod_sect` (`cod_sect`),
  CONSTRAINT `fk_options_sections` FOREIGN KEY (`cod_sect`) REFERENCES `t_sections` (`cod_sect`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_options : ~50 rows (environ)
INSERT INTO `t_options` (`cod_opt`, `description`, `cod_sect`, `code_epst`) VALUES
	('OPT00200000000012025', 'Agriculture Générale', 'SEC00200000000082025', '801'),
	('OPT00200000000022025', 'Pêche et Navigation', 'SEC00200000000082025', '802'),
	('OPT00200000000032025', 'Vétérinaire', 'SEC00200000000082025', '803'),
	('OPT00200000000042025', 'Industries Agricoles', 'SEC00200000000082025', '804'),
	('OPT00200000000052025', 'Nutrition', 'SEC00200000000082025', '805'),
	('OPT00200000000062025', 'Foresterie', 'SEC00200000000082025', '806'),
	('OPT00200000000072025', 'Arts Plastiques', 'SEC00200000000062025', '501'),
	('OPT00200000000082025', 'Arts Dramatiques', 'SEC00200000000062025', '502'),
	('OPT00200000000092025', 'Musique', 'SEC00200000000062025', '503'),
	('OPT00200000000102025', 'Esthétique et Coiffure', 'SEC00200000000062025', '504'),
	('OPT00200000000112025', 'Coiffure', 'SEC00200000000062025', '505'),
	('OPT00200000000122025', 'Hôtesse d’Accueil', 'SEC00200000000072025', '701'),
	('OPT00200000000132025', 'Hôtellerie et Restauration', 'SEC00200000000072025', '702'),
	('OPT00200000000142025', 'Hébergement', 'SEC00200000000072025', '703'),
	('OPT00200000000152025', 'Tourisme', 'SEC00200000000072025', '704'),
	('OPT00200000000162025', 'Latin-Philosophie', 'SEC00200000000012025', '101'),
	('OPT00200000000172025', 'Latin-Grec', 'SEC00200000000012025', '104'),
	('OPT00200000000182025', 'Pédagogie Générale', 'SEC00200000000032025', '201'),
	('OPT00200000000192025', 'Éducation Physique', 'SEC00200000000032025', '202'),
	('OPT00200000000202025', 'Normale', 'SEC00200000000032025', '203'),
	('OPT00200000000212025', 'Pédagogie Maternelle', 'SEC00200000000032025', '204'),
	('OPT00200000000222025', 'Pédagogie Pré-scolaire', 'SEC00200000000032025', '205'),
	('OPT00200000000232025', 'Sciences', 'SEC00200000000022025', '102'),
	('OPT00200000000242025', 'Latin-Mathématique', 'SEC00200000000022025', '105'),
	('OPT00200000000252025', 'Sociale', 'SEC00200000000052025', '401'),
	('OPT00200000000262025', 'Commerciale et Gestion', 'SEC00200000000042025', '301'),
	('OPT00200000000272025', 'Secrétariat-Administration', 'SEC00200000000042025', '302'),
	('OPT00200000000282025', 'Coupe et Couture', 'SEC00200000000042025', '601'),
	('OPT00200000000292025', 'Mécanique Générale', 'SEC00200000000092025', '901'),
	('OPT00200000000302025', 'Mécanique Machines-Outils', 'SEC00200000000092025', '902'),
	('OPT00200000000312025', 'Électricité', 'SEC00200000000092025', '903'),
	('OPT00200000000322025', 'Construction', 'SEC00200000000092025', '904'),
	('OPT00200000000332025', 'Chimie Industrielle', 'SEC00200000000092025', '905'),
	('OPT00200000000342025', 'Électronique Industrielle', 'SEC00200000000092025', '906'),
	('OPT00200000000352025', 'Imprimerie', 'SEC00200000000092025', '907'),
	('OPT00200000000362025', 'Commutation', 'SEC00200000000092025', '908'),
	('OPT00200000000372025', 'Radio-Transmission', 'SEC00200000000092025', '909'),
	('OPT00200000000382025', 'Météorologie', 'SEC00200000000092025', '910'),
	('OPT00200000000392025', 'Aviation Civile', 'SEC00200000000092025', '911'),
	('OPT00200000000402025', 'Mécanique – Dessin', 'SEC00200000000092025', '912'),
	('OPT00200000000412025', 'Hydro-Pneumatique', 'SEC00200000000092025', '913'),
	('OPT00200000000422025', 'Petrochimie', 'SEC00200000000092025', '914'),
	('OPT00200000000432025', 'Mécanique – Automobile', 'SEC00200000000092025', '915'),
	('OPT00200000000442025', 'Construction Métallique', 'SEC00200000000092025', '916'),
	('OPT00200000000452025', 'Menuiserie', 'SEC00200000000092025', '917'),
	('OPT00200000000462025', 'Mines et Géologie', 'SEC00200000000092025', '918'),
	('OPT00200000000472025', 'Métallurgie', 'SEC00200000000092025', '919'),
	('OPT00200000000482025', 'Dessin de Bâtiment', 'SEC00200000000092025', '920'),
	('OPT00200000000492025', 'Plomberie - Installation Sanit', 'SEC00200000000092025', '921'),
	('OPT00200000000502025', 'Mines et Carrières', 'SEC00200000000092025', '922');

-- Listage de la structure de table ecole_db. t_paie
CREATE TABLE IF NOT EXISTS `t_paie` (
  `id_paie` int NOT NULL AUTO_INCREMENT,
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `date_paie` date DEFAULT NULL,
  `mois` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `annee_scol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `sal_base` decimal(10,2) DEFAULT NULL,
  `prime` decimal(10,2) DEFAULT NULL,
  `autre_gains` decimal(10,2) DEFAULT NULL,
  `tot_gain` decimal(10,2) DEFAULT NULL,
  `ipr` decimal(10,2) DEFAULT NULL,
  `avs` decimal(10,2) DEFAULT NULL,
  `prets` decimal(10,2) DEFAULT NULL,
  `autre_ret` decimal(10,2) DEFAULT NULL,
  `tot_ret` decimal(10,2) DEFAULT NULL,
  `sal_net` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`id_paie`),
  KEY `idx_paie_matricule` (`matricule`),
  CONSTRAINT `fk_paie_agents` FOREIGN KEY (`matricule`) REFERENCES `t_agents` (`matricule`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_paie : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_paiement
CREATE TABLE IF NOT EXISTS `t_paiement` (
  `num_recu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_frais` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_paie` date NOT NULL,
  `libelle` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `motif` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `montant` decimal(10,2) NOT NULL,
  `agent` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_promo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `annee_scol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`num_recu`),
  KEY `idx_paiement_cod_promo` (`cod_promo`),
  KEY `idx_paiement_annee_scol` (`annee_scol`),
  KEY `idx_paiement_matricule` (`matricule`),
  KEY `idx_paiement_cod_frais` (`cod_frais`),
  KEY `idx_paiement_agent` (`agent`),
  KEY `idx_paiement_date_agent` (`date_paie`,`agent`),
  KEY `idx_paiement_periode_annee` (`annee_scol`),
  CONSTRAINT `fk_paiement_eleves` FOREIGN KEY (`matricule`) REFERENCES `t_eleves` (`matricule`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_paiement_frais` FOREIGN KEY (`cod_frais`) REFERENCES `t_frais` (`cod_frais`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_paiement_users_infos` FOREIGN KEY (`agent`) REFERENCES `t_users_infos` (`id_user`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `chk_paiement_date` CHECK ((`date_paie` >= _utf8mb4'2000-01-01')),
  CONSTRAINT `chk_paiement_montant` CHECK ((`montant` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_paiement : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_palmares
CREATE TABLE IF NOT EXISTS `t_palmares` (
  `IdPalmares` int NOT NULL AUTO_INCREMENT,
  `Matricule` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CodPromo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Periode` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Place` int NOT NULL,
  `Moyenne` decimal(5,2) NOT NULL,
  `AnneeScol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Mention` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_ecole` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdPalmares`),
  KEY `idx_palmares_matricule` (`Matricule`),
  KEY `idx_palmares_promo` (`CodPromo`),
  KEY `idx_palmares_annee` (`AnneeScol`),
  KEY `idx_palmares_ecole` (`id_ecole`),
  CONSTRAINT `fk_palmares_ecoles` FOREIGN KEY (`id_ecole`) REFERENCES `t_ecoles` (`id_ecole`) ON DELETE CASCADE,
  CONSTRAINT `fk_palmares_eleves` FOREIGN KEY (`Matricule`) REFERENCES `t_eleves` (`matricule`) ON DELETE CASCADE,
  CONSTRAINT `fk_palmares_promotions` FOREIGN KEY (`CodPromo`) REFERENCES `t_promotions` (`cod_promo`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_palmares : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_permissions
CREATE TABLE IF NOT EXISTS `t_permissions` (
  `id_permission` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nom_permission` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description_permission` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `module_concerne` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `action_permission` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ressource_cible` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `etat` tinyint(1) DEFAULT '1',
  `date_creation` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_modification` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `cree_par` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `modifie_par` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_permission`),
  UNIQUE KEY `nom_permission` (`nom_permission`),
  KEY `idx_nom_permission` (`nom_permission`),
  KEY `idx_module` (`module_concerne`),
  KEY `idx_action` (`action_permission`),
  KEY `idx_etat_permission` (`etat`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_permissions : ~25 rows (environ)
INSERT INTO `t_permissions` (`id_permission`, `nom_permission`, `description_permission`, `module_concerne`, `action_permission`, `ressource_cible`, `etat`, `date_creation`, `date_modification`, `cree_par`, `modifie_par`) VALUES
	('PER00000000001', 'Créer Utilisateur', 'Créer de nouveaux utilisateurs', 'Utilisateurs', 'CREATE', 'utilisateurs', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000002', 'Modifier Utilisateur', 'Modifier les informations des utilisateurs', 'Utilisateurs', 'UPDATE', 'utilisateurs', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000003', 'Supprimer Utilisateur', 'Supprimer des utilisateurs', 'Utilisateurs', 'DELETE', 'utilisateurs', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000004', 'Voir Utilisateurs', 'Consulter la liste des utilisateurs', 'Utilisateurs', 'READ', 'utilisateurs', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000005', 'Gérer Rôles', 'Créer, modifier, supprimer des rôles', 'Sécurité', 'MANAGE', 'roles', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000006', 'Gérer Permissions', 'Attribuer/retirer des permissions', 'Sécurité', 'MANAGE', 'permissions', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000007', 'Créer École', 'Créer de nouvelles écoles', 'Écoles', 'CREATE', 'ecoles', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000008', 'Modifier École', 'Modifier les informations des écoles', 'Écoles', 'UPDATE', 'ecoles', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000009', 'Supprimer École', 'Supprimer des écoles', 'Écoles', 'DELETE', 'ecoles', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000010', 'Voir Écoles', 'Consulter les informations des écoles', 'Écoles', 'READ', 'ecoles', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000011', 'Inscrire Élève', 'Inscrire de nouveaux élèves', 'Élèves', 'CREATE', 'eleves', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000012', 'Modifier Élève', 'Modifier les informations des élèves', 'Élèves', 'UPDATE', 'eleves', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000013', 'Supprimer Élève', 'Supprimer des élèves', 'Élèves', 'DELETE', 'eleves', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000014', 'Voir Élèves', 'Consulter les informations des élèves', 'Élèves', 'READ', 'eleves', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000015', 'Créer Classe', 'Créer de nouvelles classes', 'Classes', 'CREATE', 'classes', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000016', 'Modifier Classe', 'Modifier les informations des classes', 'Classes', 'UPDATE', 'classes', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000017', 'Supprimer Classe', 'Supprimer des classes', 'Classes', 'DELETE', 'classes', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000018', 'Voir Classes', 'Consulter les informations des classes', 'Classes', 'READ', 'classes', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000019', 'Saisir Notes', 'Saisir et modifier les notes', 'Notes', 'CREATE', 'notes', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000020', 'Voir Notes', 'Consulter les notes', 'Notes', 'READ', 'notes', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000021', 'Valider Notes', 'Valider et publier les notes', 'Notes', 'VALIDATE', 'notes', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000022', 'Générer Rapports', 'Générer des rapports et statistiques', 'Rapports', 'GENERATE', 'rapports', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000023', 'Voir Statistiques', 'Consulter les statistiques', 'Rapports', 'READ', 'statistiques', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000024', 'Configuration Système', 'Modifier la configuration du système', 'Système', 'CONFIGURE', 'systeme', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('PER00000000025', 'Sauvegarde Système', 'Effectuer des sauvegardes', 'Système', 'BACKUP', 'systeme', 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL);

-- Listage de la structure de table ecole_db. t_promotions
CREATE TABLE IF NOT EXISTS `t_promotions` (
  `cod_promo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `cod_opt` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `indice_promo` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`cod_promo`),
  UNIQUE KEY `description` (`description`),
  KEY `idx_promotions_cod_opt` (`cod_opt`),
  KEY `idx_promo_cod_opt` (`cod_opt`),
  CONSTRAINT `fk_promotions_options` FOREIGN KEY (`cod_opt`) REFERENCES `t_options` (`cod_opt`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_promotions : ~2 rows (environ)
INSERT INTO `t_promotions` (`cod_promo`, `description`, `cod_opt`, `indice_promo`) VALUES
	('PRO00200000000012025', '3ème Coupe et Couture', 'OPT00200000000282025', '3ème CC'),
	('PRO00200000000022025', '3ème Commerciale et Gestion', 'OPT00200000000262025', '3ème CG');

-- Listage de la structure de table ecole_db. t_roles
CREATE TABLE IF NOT EXISTS `t_roles` (
  `id_role` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nom_role` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description_role` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `niveau_acces` int DEFAULT '1',
  `etat` tinyint(1) DEFAULT '1',
  `date_creation` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_modification` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `cree_par` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `modifie_par` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_role`),
  UNIQUE KEY `nom_role` (`nom_role`),
  KEY `idx_nom_role` (`nom_role`),
  KEY `idx_niveau_acces` (`niveau_acces`),
  KEY `idx_etat_role` (`etat`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_roles : ~8 rows (environ)
INSERT INTO `t_roles` (`id_role`, `nom_role`, `description_role`, `niveau_acces`, `etat`, `date_creation`, `date_modification`, `cree_par`, `modifie_par`) VALUES
	('ROL00000000001', 'Super Administrateur', 'Accès complet à toutes les fonctionnalités du système', 10, 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('ROL00000000002', 'Administrateur', 'Gestion complète de l\'école et des utilisateurs', 8, 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('ROL00000000003', 'Directeur', 'Gestion pédagogique et administrative de l\'école', 6, 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('ROL00000000004', 'Secrétaire', 'Gestion des inscriptions et de l\'administration courante', 4, 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('ROL00000000005', 'Enseignant', 'Gestion des classes et des notes', 3, 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('ROL00000000006', 'Surveillant', 'Surveillance et discipline', 2, 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('ROL00000000007', 'Invité', 'Accès en lecture seule limité', 1, 1, '2025-11-06 12:15:44', '2025-11-06 12:15:44', NULL, NULL),
	('ROL00000000008', 'Utilisateur Standard', 'Utilisateur standard avec permissions de base', 2, 1, '2025-11-06 12:16:03', '2025-11-06 12:16:03', NULL, NULL);

-- Listage de la structure de table ecole_db. t_roles_agents
CREATE TABLE IF NOT EXISTS `t_roles_agents` (
  `num_affect` int NOT NULL AUTO_INCREMENT,
  `fk_role` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `fk_agent` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_affect` date NOT NULL,
  PRIMARY KEY (`num_affect`),
  UNIQUE KEY `uk_tra` (`fk_role`,`fk_agent`,`date_affect`),
  KEY `idx_tra_fk_agent` (`fk_agent`),
  KEY `idx_tra_fk_role` (`fk_role`),
  KEY `idx_tra_date` (`date_affect`),
  CONSTRAINT `FK_t_roles_agents_t_agents` FOREIGN KEY (`fk_agent`) REFERENCES `t_agents` (`matricule`),
  CONSTRAINT `FK_t_roles_agents_t_roles` FOREIGN KEY (`fk_role`) REFERENCES `t_roles` (`id_role`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Listage des données de la table ecole_db.t_roles_agents : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_roles_permissions
CREATE TABLE IF NOT EXISTS `t_roles_permissions` (
  `id_role_permission` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `fk_role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `fk_permission` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `accordee` tinyint(1) DEFAULT '1',
  `date_attribution` datetime DEFAULT CURRENT_TIMESTAMP,
  `attribuee_par` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_role_permission`),
  UNIQUE KEY `unique_role_permission` (`fk_role`,`fk_permission`),
  KEY `idx_role` (`fk_role`),
  KEY `idx_permission` (`fk_permission`),
  KEY `idx_accordee` (`accordee`),
  CONSTRAINT `t_roles_permissions_ibfk_1` FOREIGN KEY (`fk_role`) REFERENCES `t_roles` (`id_role`) ON DELETE CASCADE,
  CONSTRAINT `t_roles_permissions_ibfk_2` FOREIGN KEY (`fk_permission`) REFERENCES `t_permissions` (`id_permission`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_roles_permissions : ~80 rows (environ)
INSERT INTO `t_roles_permissions` (`id_role_permission`, `fk_role`, `fk_permission`, `accordee`, `date_attribution`, `attribuee_par`) VALUES
	('RP0000000001', 'ROL00000000001', 'PER00000000001', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000002', 'ROL00000000001', 'PER00000000002', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000003', 'ROL00000000001', 'PER00000000003', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000004', 'ROL00000000001', 'PER00000000004', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000005', 'ROL00000000001', 'PER00000000005', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000006', 'ROL00000000001', 'PER00000000006', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000007', 'ROL00000000001', 'PER00000000007', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000008', 'ROL00000000001', 'PER00000000008', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000009', 'ROL00000000001', 'PER00000000009', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000010', 'ROL00000000001', 'PER00000000010', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000011', 'ROL00000000001', 'PER00000000011', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000012', 'ROL00000000001', 'PER00000000012', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000013', 'ROL00000000001', 'PER00000000013', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000014', 'ROL00000000001', 'PER00000000014', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000015', 'ROL00000000001', 'PER00000000015', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000016', 'ROL00000000001', 'PER00000000016', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000017', 'ROL00000000001', 'PER00000000017', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000018', 'ROL00000000001', 'PER00000000018', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000019', 'ROL00000000001', 'PER00000000019', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000020', 'ROL00000000001', 'PER00000000020', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000021', 'ROL00000000001', 'PER00000000021', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000022', 'ROL00000000001', 'PER00000000022', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000023', 'ROL00000000001', 'PER00000000023', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000024', 'ROL00000000001', 'PER00000000024', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000025', 'ROL00000000001', 'PER00000000025', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000101', 'ROL00000000002', 'PER00000000001', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000102', 'ROL00000000002', 'PER00000000002', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000103', 'ROL00000000002', 'PER00000000003', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000104', 'ROL00000000002', 'PER00000000004', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000105', 'ROL00000000002', 'PER00000000005', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000106', 'ROL00000000002', 'PER00000000006', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000107', 'ROL00000000002', 'PER00000000007', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000108', 'ROL00000000002', 'PER00000000008', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000109', 'ROL00000000002', 'PER00000000009', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000110', 'ROL00000000002', 'PER00000000010', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000111', 'ROL00000000002', 'PER00000000011', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000112', 'ROL00000000002', 'PER00000000012', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000113', 'ROL00000000002', 'PER00000000013', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000114', 'ROL00000000002', 'PER00000000014', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000115', 'ROL00000000002', 'PER00000000015', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000116', 'ROL00000000002', 'PER00000000016', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000117', 'ROL00000000002', 'PER00000000017', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000118', 'ROL00000000002', 'PER00000000018', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000119', 'ROL00000000002', 'PER00000000019', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000120', 'ROL00000000002', 'PER00000000020', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000121', 'ROL00000000002', 'PER00000000021', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000122', 'ROL00000000002', 'PER00000000022', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000123', 'ROL00000000002', 'PER00000000023', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000201', 'ROL00000000003', 'PER00000000004', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000202', 'ROL00000000003', 'PER00000000007', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000203', 'ROL00000000003', 'PER00000000008', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000204', 'ROL00000000003', 'PER00000000010', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000205', 'ROL00000000003', 'PER00000000011', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000206', 'ROL00000000003', 'PER00000000012', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000207', 'ROL00000000003', 'PER00000000014', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000208', 'ROL00000000003', 'PER00000000015', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000209', 'ROL00000000003', 'PER00000000016', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000210', 'ROL00000000003', 'PER00000000018', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000211', 'ROL00000000003', 'PER00000000020', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000212', 'ROL00000000003', 'PER00000000021', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000213', 'ROL00000000003', 'PER00000000022', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000214', 'ROL00000000003', 'PER00000000023', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000301', 'ROL00000000004', 'PER00000000004', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000302', 'ROL00000000004', 'PER00000000010', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000303', 'ROL00000000004', 'PER00000000011', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000304', 'ROL00000000004', 'PER00000000012', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000305', 'ROL00000000004', 'PER00000000014', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000306', 'ROL00000000004', 'PER00000000018', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000307', 'ROL00000000004', 'PER00000000020', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000401', 'ROL00000000005', 'PER00000000014', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000402', 'ROL00000000005', 'PER00000000018', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000403', 'ROL00000000005', 'PER00000000019', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000404', 'ROL00000000005', 'PER00000000020', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000501', 'ROL00000000006', 'PER00000000014', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000502', 'ROL00000000006', 'PER00000000018', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000601', 'ROL00000000007', 'PER00000000023', 1, '2025-11-06 12:15:44', NULL),
	('RP0000000801', 'ROL00000000008', 'PER00000000014', 1, '2025-11-06 12:16:03', NULL),
	('RP0000000802', 'ROL00000000008', 'PER00000000018', 1, '2025-11-06 12:16:03', NULL),
	('RP0000000803', 'ROL00000000008', 'PER00000000020', 1, '2025-11-06 12:16:03', NULL),
	('RP0000000804', 'ROL00000000008', 'PER00000000023', 1, '2025-11-06 12:16:03', NULL);

-- Listage de la structure de table ecole_db. t_sections
CREATE TABLE IF NOT EXISTS `t_sections` (
  `cod_sect` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`cod_sect`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_sections : ~9 rows (environ)
INSERT INTO `t_sections` (`cod_sect`, `description`) VALUES
	('SEC00200000000012025', 'Littéraire'),
	('SEC00200000000022025', 'Scientifique'),
	('SEC00200000000032025', 'Pédagogique'),
	('SEC00200000000042025', 'Technique'),
	('SEC00200000000052025', 'Sociale'),
	('SEC00200000000062025', 'Artistique'),
	('SEC00200000000072025', 'Hôtellerie et Tourisme'),
	('SEC00200000000082025', 'Agricole'),
	('SEC00200000000092025', 'Technique Industrielle');

-- Listage de la structure de table ecole_db. t_service_agent
CREATE TABLE IF NOT EXISTS `t_service_agent` (
  `num_affect` int NOT NULL AUTO_INCREMENT,
  `fk_service` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `fk_agent` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `date_affect` date NOT NULL,
  PRIMARY KEY (`num_affect`),
  UNIQUE KEY `uk_tsa` (`fk_service`,`fk_agent`,`date_affect`),
  KEY `idx_tsa_fk_agent` (`fk_agent`),
  KEY `idx_tsa_fk_service` (`fk_service`),
  KEY `idx_tsa_date` (`date_affect`),
  CONSTRAINT `FK_t_service_agent_t_agents` FOREIGN KEY (`fk_agent`) REFERENCES `t_agents` (`matricule`),
  CONSTRAINT `FK_t_service_agent_t_services` FOREIGN KEY (`fk_service`) REFERENCES `t_services` (`id_service`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Listage des données de la table ecole_db.t_service_agent : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_services
CREATE TABLE IF NOT EXISTS `t_services` (
  `id_service` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_service`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_services : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_sortie
CREATE TABLE IF NOT EXISTS `t_sortie` (
  `id_sortie` int NOT NULL AUTO_INCREMENT,
  `date_sortie` date DEFAULT NULL,
  `heure_sortie` time DEFAULT NULL,
  `motif` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `montant` decimal(10,2) DEFAULT NULL,
  `annee_scol` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `agent` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `commentaire` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `beneficiaire` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_sortie`),
  KEY `idx_sortie_date_sortie` (`date_sortie`),
  KEY `idx_sortie_annee_scol` (`annee_scol`),
  KEY `idx_sortie_type` (`type`),
  KEY `fk_sortie_users_infos` (`agent`),
  CONSTRAINT `fk_sortie_users_infos` FOREIGN KEY (`agent`) REFERENCES `t_users_infos` (`id_user`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_sortie : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_source_options
CREATE TABLE IF NOT EXISTS `t_source_options` (
  `Code_Epst` int NOT NULL,
  `OptionName` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Code_Epst`) USING BTREE,
  UNIQUE KEY `OptionName` (`OptionName`),
  UNIQUE KEY `Code` (`Code_Epst`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_source_options : ~51 rows (environ)
INSERT INTO `t_source_options` (`Code_Epst`, `OptionName`) VALUES
	(100, 'Education de Base'),
	(101, 'Latin-Philosophie'),
	(102, 'Sciences'),
	(104, 'Latin-Grec'),
	(105, 'Latin-Mathématique'),
	(201, 'Pédagogie Générale'),
	(202, 'Éducation Physique'),
	(203, 'Normale'),
	(204, 'Pédagogie Maternelle'),
	(205, 'Pédagogie Pré-scolaire'),
	(301, 'Commerciale et Gestion'),
	(302, 'Secrétariat-Administration'),
	(401, 'Sociale'),
	(501, 'Arts Plastiques'),
	(502, 'Arts Dramatiques'),
	(503, 'Musique'),
	(504, 'Esthétique et Coiffure'),
	(505, 'Coiffure'),
	(601, 'Coupe et Couture'),
	(701, 'Hôtesse d’Accueil'),
	(702, 'Hôtellerie et Restauration'),
	(703, 'Hébergement'),
	(704, 'Tourisme'),
	(801, 'Agriculture Générale'),
	(802, 'Pêche et Navigation'),
	(803, 'Vétérinaire'),
	(804, 'Industries Agricoles'),
	(805, 'Nutrition'),
	(806, 'Foresterie'),
	(901, 'Mécanique Générale'),
	(902, 'Mécanique Machines-Outils'),
	(903, 'Électricité'),
	(904, 'Construction'),
	(905, 'Chimie Industrielle'),
	(906, 'Électronique Industrielle'),
	(907, 'Imprimerie'),
	(908, 'Commutation'),
	(909, 'Radio-Transmission'),
	(910, 'Météorologie'),
	(911, 'Aviation Civile'),
	(912, 'Mécanique – Dessin'),
	(913, 'Hydro-Pneumatique'),
	(914, 'Petrochimie'),
	(915, 'Mécanique – Automobile'),
	(916, 'Construction Métallique'),
	(917, 'Menuiserie'),
	(918, 'Mines et Géologie'),
	(919, 'Métallurgie'),
	(920, 'Dessin de Bâtiment'),
	(921, 'Plomberie-Installation Sanitaire'),
	(922, 'Mines et Carrières');

-- Listage de la structure de table ecole_db. t_type_entite_administrative
CREATE TABLE IF NOT EXISTS `t_type_entite_administrative` (
  `IdTypeEntite` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IntituleTypeEntite` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Etat` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IdTypeEntite`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_type_entite_administrative : ~14 rows (environ)
INSERT INTO `t_type_entite_administrative` (`IdTypeEntite`, `IntituleTypeEntite`, `Etat`) VALUES
	('TEA00000000012019', 'Pays', '1'),
	('TEA00000000022019', 'Province', '1'),
	('TEA00000000032019', 'Ville', '1'),
	('TEA00000000042019', 'District', '1'),
	('TEA00000000052019', 'Territoire', '1'),
	('TEA00000000062019', 'Secteur', '1'),
	('TEA00000000072019', 'Commune', '1'),
	('TEA00000000092019', 'Quartier', '1'),
	('TEA00000000102019', 'Cité', '1'),
	('TEA00000000112019', 'Groupement', '1'),
	('TEA00000000122019', 'Village', '1'),
	('TEA00000000132019', 'Avenue/Rue', '1'),
	('TEA00000000142019', 'Continent', '1'),
	('TEA00000000152019', 'Cheffrerie', '1');

-- Listage de la structure de table ecole_db. t_type_frais
CREATE TABLE IF NOT EXISTS `t_type_frais` (
  `cod_type_frais` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`cod_type_frais`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_type_frais : ~0 rows (environ)

-- Listage de la structure de table ecole_db. t_users_infos
CREATE TABLE IF NOT EXISTS `t_users_infos` (
  `id_user` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `postnom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `prenom` varchar(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `sexe` enum('M','F') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `pwd_hash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'bcrypt hash - minimum 8 caractères requis',
  `salt` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Salt pour le hachage du mot de passe',
  `telephone` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_ecole` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `fk_role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `type_user` enum('SYSTEM','ECOLE') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'ECOLE',
  `profil` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `user_index` int NOT NULL DEFAULT '0',
  `last_password_change` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Dernière modification du mot de passe',
  `failed_login_attempts` int DEFAULT '0' COMMENT 'Nombre de tentatives de connexion échouées',
  `account_locked_until` timestamp NULL DEFAULT NULL COMMENT 'Compte verrouillé jusqu''à cette date',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `derniere_connexion` datetime DEFAULT NULL,
  `tentatives_connexion` int DEFAULT '0',
  `compte_verrouille` tinyint(1) DEFAULT '0',
  `date_verrouillage` datetime DEFAULT NULL,
  PRIMARY KEY (`id_user`),
  UNIQUE KEY `user_index` (`user_index`),
  UNIQUE KEY `idx_users_infos_user_index` (`user_index`),
  UNIQUE KEY `idx_users_infos_username` (`username`),
  UNIQUE KEY `idx_users_infos_telephone` (`telephone`),
  KEY `idx_users_infos_fk_role` (`fk_role`),
  KEY `idx_users_infos_derniere_connexion` (`derniere_connexion`),
  KEY `idx_users_infos_compte_verrouille` (`compte_verrouille`),
  KEY `nom` (`nom`) USING BTREE,
  KEY `prenom` (`prenom`) USING BTREE,
  KEY `FK_t_users_infos_t_ecoles` (`id_ecole`),
  CONSTRAINT `FK_t_users_infos_t_ecoles` FOREIGN KEY (`id_ecole`) REFERENCES `t_ecoles` (`id_ecole`),
  CONSTRAINT `fk_users_infos_role` FOREIGN KEY (`fk_role`) REFERENCES `t_roles` (`id_role`) ON DELETE SET NULL,
  CONSTRAINT `chk_users_failed_attempts` CHECK (((`failed_login_attempts` >= 0) and (`failed_login_attempts` <= 10)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Listage des données de la table ecole_db.t_users_infos : ~1 rows (environ)
INSERT INTO `t_users_infos` (`id_user`, `nom`, `postnom`, `prenom`, `sexe`, `username`, `pwd_hash`, `salt`, `telephone`, `id_ecole`, `fk_role`, `type_user`, `profil`, `user_index`, `last_password_change`, `failed_login_attempts`, `account_locked_until`, `created_at`, `updated_at`, `derniere_connexion`, `tentatives_connexion`, `compte_verrouille`, `date_verrouillage`) VALUES
	('USR00100000000032025', 'ADMIN', 'SUPER', 'Administrateur', 'M', 'admin', '$2a$11$ykFE/HO4MmLFUFmkA50UWub89/bXq9zr2beyL1e9qvuzdtT4EeXmW', '$2a$$11$', NULL, NULL, 'ROL00000000001', 'SYSTEM', NULL, 3, '2026-01-05 18:21:15', 0, NULL, '2026-01-05 18:21:15', '2026-01-11 09:51:04', NULL, 0, 0, NULL);

-- Listage de la structure de vue ecole_db. view_cours
-- Création d'une table temporaire pour palier aux erreurs de dépendances de VIEW
CREATE TABLE `view_cours` (
	`Id_Cours` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Intitule` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`periode_max` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Titulaire` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`tel_titulaire` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`CodP_romo` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`annee_scol` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Indice` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`DescripPromo` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`DescripOpt` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Id_Ecole` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Cod_Sect` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`DescripSect` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Denomination` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci'
);

-- Listage de la structure de vue ecole_db. vue_avenue_hierarchie
-- Création d'une table temporaire pour palier aux erreurs de dépendances de VIEW
CREATE TABLE `vue_avenue_hierarchie` (
	`Id_Avenue` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Avenue` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Id_Quartier` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Quartier` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Id_Commune` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Commune` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Id_Ville` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Ville` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Id_Province` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Province` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci'
);

-- Listage de la structure de vue ecole_db. vue_ecole
-- Création d'une table temporaire pour palier aux erreurs de dépendances de VIEW
CREATE TABLE `vue_ecole` (
	`id_ecole` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Ecole` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`logo` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`NumParcelle` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Avenue` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Quartier` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Commune` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Ville` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Province` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci'
);

-- Listage de la structure de vue ecole_db. vue_entite_administrative
-- Création d'une table temporaire pour palier aux erreurs de dépendances de VIEW
CREATE TABLE `vue_entite_administrative` (
	`IdEntite` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`IntituleEntite` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`Nature` LONGTEXT NULL COLLATE 'utf8mb4_unicode_ci',
	`Nationalite` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci'
);

-- Listage de la structure de vue ecole_db. vue_options
-- Création d'une table temporaire pour palier aux erreurs de dépendances de VIEW
CREATE TABLE `vue_options` (
	`Code_Option` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Options` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`Sections` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci'
);

-- Listage de la structure de vue ecole_db. vue_roles_permissions
-- Création d'une table temporaire pour palier aux erreurs de dépendances de VIEW
CREATE TABLE `vue_roles_permissions` (
	`id_role` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`nom_role` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`description_role` TEXT NULL COLLATE 'utf8mb4_unicode_ci',
	`niveau_acces` INT NULL,
	`id_permission` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`nom_permission` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`description_permission` TEXT NULL COLLATE 'utf8mb4_unicode_ci',
	`module_concerne` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`action_permission` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`ressource_cible` VARCHAR(1) NULL COLLATE 'utf8mb4_unicode_ci',
	`accordee` TINYINT(1) NULL
);

-- Listage de la structure de déclencheur ecole_db. tr_users_infos_before_insert
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO';
DELIMITER //
CREATE DEFINER=`root`@`localhost` TRIGGER `tr_users_infos_before_insert` BEFORE INSERT ON `t_users_infos` FOR EACH ROW BEGIN
    IF NEW.user_index = 0 OR NEW.user_index IS NULL THEN
        SET NEW.user_index = (SELECT COALESCE(MAX(user_index), 0) + 1 FROM t_users_infos);
    END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Suppression de la table temporaire et création finale de la structure de VIEW
DROP TABLE IF EXISTS `view_cours`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `view_cours` AS select distinct `ac`.`id_cours` AS `Id_Cours`,`c`.`intitule` AS `Intitule`,`ac`.`periode_max` AS `periode_max`,`ac`.`titulaire` AS `Titulaire`,`ac`.`tel_titulaire` AS `tel_titulaire`,`ac`.`cod_promo` AS `CodP_romo`,`ac`.`annee_scol` AS `annee_scol`,`ac`.`indice` AS `Indice`,`p`.`description` AS `DescripPromo`,`o`.`description` AS `DescripOpt`,`asct`.`id_ecole` AS `Id_Ecole`,`asct`.`cod_sect` AS `Cod_Sect`,`s`.`description` AS `DescripSect`,`e`.`denomination` AS `Denomination` from ((((((`t_affect_cours` `ac` join `t_cours` `c` on((`ac`.`id_cours` = `c`.`id_cours`))) join `t_promotions` `p` on((`ac`.`cod_promo` = `p`.`cod_promo`))) join `t_options` `o` on((`p`.`cod_opt` = `o`.`cod_opt`))) join `t_sections` `s` on((`o`.`cod_sect` = `s`.`cod_sect`))) join `t_affect_sect` `asct` on((`s`.`cod_sect` = `asct`.`cod_sect`))) join `t_ecoles` `e` on((`asct`.`id_ecole` = `e`.`id_ecole`)))
;

-- Suppression de la table temporaire et création finale de la structure de VIEW
DROP TABLE IF EXISTS `vue_avenue_hierarchie`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY INVOKER VIEW `vue_avenue_hierarchie` AS select `av`.`IdEntite` AS `Id_Avenue`,`av`.`IntituleEntite` AS `Avenue`,`q`.`IdEntite` AS `Id_Quartier`,`q`.`IntituleEntite` AS `Quartier`,`c`.`IdEntite` AS `Id_Commune`,`c`.`IntituleEntite` AS `Commune`,`v`.`IdEntite` AS `Id_Ville`,`v`.`IntituleEntite` AS `Ville`,`p`.`IdEntite` AS `Id_Province`,`p`.`IntituleEntite` AS `Province` from ((((`t_entite_administrative` `av` join `t_entite_administrative` `q` on(((`av`.`Fk_EntiteMere` = `q`.`IdEntite`) and (`q`.`Fk_TypeEntite` = 'TEA00000000092019')))) join `t_entite_administrative` `c` on(((`q`.`Fk_EntiteMere` = `c`.`IdEntite`) and (`c`.`Fk_TypeEntite` = 'TEA00000000072019')))) join `t_entite_administrative` `v` on(((`c`.`Fk_EntiteMere` = `v`.`IdEntite`) and (`v`.`Fk_TypeEntite` = 'TEA00000000032019')))) join `t_entite_administrative` `p` on(((`v`.`Fk_EntiteMere` = `p`.`IdEntite`) and (`p`.`Fk_TypeEntite` = 'TEA00000000022019')))) where (`av`.`Fk_TypeEntite` = 'TEA00000000132019')
;

-- Suppression de la table temporaire et création finale de la structure de VIEW
DROP TABLE IF EXISTS `vue_ecole`;
CREATE ALGORITHM=MERGE DEFINER=`root`@`localhost` SQL SECURITY INVOKER VIEW `vue_ecole` AS select `e`.`id_ecole` AS `id_ecole`,`e`.`denomination` AS `Ecole`,`e`.`logo` AS `logo`,`e`.`numero` AS `NumParcelle`,`av`.`IntituleEntite` AS `Avenue`,`q`.`IntituleEntite` AS `Quartier`,`c`.`IntituleEntite` AS `Commune`,`v`.`IntituleEntite` AS `Ville`,`p`.`IntituleEntite` AS `Province` from (((((`t_ecoles` `e` join `t_entite_administrative` `av` on(((`e`.`FkAvenue` = `av`.`IdEntite`) and (`av`.`Fk_TypeEntite` = 'TEA00000000132019')))) join `t_entite_administrative` `q` on(((`av`.`Fk_EntiteMere` = `q`.`IdEntite`) and (`q`.`Fk_TypeEntite` = 'TEA00000000092019')))) join `t_entite_administrative` `c` on(((`q`.`Fk_EntiteMere` = `c`.`IdEntite`) and (`c`.`Fk_TypeEntite` = 'TEA00000000072019')))) join `t_entite_administrative` `v` on(((`c`.`Fk_EntiteMere` = `v`.`IdEntite`) and (`v`.`Fk_TypeEntite` = 'TEA00000000032019')))) join `t_entite_administrative` `p` on(((`v`.`Fk_EntiteMere` = `p`.`IdEntite`) and (`p`.`Fk_TypeEntite` = 'TEA00000000022019'))))
;

-- Suppression de la table temporaire et création finale de la structure de VIEW
DROP TABLE IF EXISTS `vue_entite_administrative`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY INVOKER VIEW `vue_entite_administrative` AS select `ea`.`IdEntite` AS `IdEntite`,`ea`.`IntituleEntite` AS `IntituleEntite`,`te`.`IntituleTypeEntite` AS `Nature`,`ea`.`DenominationHabitant` AS `Nationalite` from (`t_entite_administrative` `ea` join `t_type_entite_administrative` `te` on((`ea`.`Fk_TypeEntite` = `te`.`IdTypeEntite`)))
;

-- Suppression de la table temporaire et création finale de la structure de VIEW
DROP TABLE IF EXISTS `vue_options`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `vue_options` AS select `o`.`cod_opt` AS `Code_Option`,`o`.`description` AS `Options`,`s`.`description` AS `Sections` from (`t_options` `o` join `t_sections` `s` on((`o`.`cod_sect` = `s`.`cod_sect`)))
;

-- Suppression de la table temporaire et création finale de la structure de VIEW
DROP TABLE IF EXISTS `vue_roles_permissions`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `vue_roles_permissions` AS select `r`.`id_role` AS `id_role`,`r`.`nom_role` AS `nom_role`,`r`.`description_role` AS `description_role`,`r`.`niveau_acces` AS `niveau_acces`,`p`.`id_permission` AS `id_permission`,`p`.`nom_permission` AS `nom_permission`,`p`.`description_permission` AS `description_permission`,`p`.`module_concerne` AS `module_concerne`,`p`.`action_permission` AS `action_permission`,`p`.`ressource_cible` AS `ressource_cible`,`rp`.`accordee` AS `accordee` from ((`t_roles` `r` left join `t_roles_permissions` `rp` on((`r`.`id_role` = `rp`.`fk_role`))) left join `t_permissions` `p` on((`rp`.`fk_permission` = `p`.`id_permission`))) where ((`r`.`etat` = 1) and (`p`.`etat` = 1)) order by `r`.`niveau_acces` desc,`p`.`module_concerne`,`p`.`nom_permission`
;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
