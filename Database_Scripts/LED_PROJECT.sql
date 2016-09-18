CREATE TABLE `led_project` (
`PROJECT_NAME` VARCHAR(50) NOT NULL,
`DESCRIPTION` VARCHAR(50) NULL DEFAULT NULL,
`SONG_PATH` VARCHAR(100) NULL DEFAULT NULL,
PRIMARY KEY (`PROJECT_NAME`)
)
COMMENT='This table will hold all projects created within the LED Lighting Composer'
COLLATE='latin1_swedish_ci'
ENGINE=InnoDB
;

