CREATE TABLE `mcu` (
`MCU_NAME` VARCHAR(50) NOT NULL,
`DESCRIPTION` VARCHAR(50) NULL DEFAULT NULL,
PRIMARY KEY (`MCU_NAME`)
)
COMMENT='This table will hold all distinct microcontroller unit names that could be used within a project'
COLLATE='latin1_swedish_ci'
ENGINE=InnoDB
;

