CREATE TABLE `lighting_effects` (
`LIGHTING_EFFECT` INT(11) NOT NULL,
`DESCRIPTION` VARCHAR(100) NULL DEFAULT NULL,
PRIMARY KEY (`LIGHTING_EFFECT`)
)
COMMENT='This table will hold all possible lighting effects for LEDs'
COLLATE='latin1_swedish_ci'
ENGINE=InnoDB
;

