CREATE TABLE `mcu` (
	`MCU_NAME` VARCHAR(50) NOT NULL,
	`DESCRIPTION` VARCHAR(50) NULL DEFAULT NULL,
	`PROJECT_NAME` VARCHAR(50) NOT NULL,
	PRIMARY KEY (`MCU_NAME`),
	INDEX `mcu_project_name` (`PROJECT_NAME`),
	CONSTRAINT `mcu_project_name` FOREIGN KEY (`PROJECT_NAME`) REFERENCES `led_project` (`PROJECT_NAME`) ON UPDATE CASCADE ON DELETE CASCADE
)
COMMENT='This table will hold all distinct microcontroller unit names that could be used within a project'
COLLATE='latin1_swedish_ci'
ENGINE=InnoDB
;
