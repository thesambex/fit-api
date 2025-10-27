CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'assessments') THEN
        CREATE SCHEMA assessments;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'patients') THEN
        CREATE SCHEMA patients;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'professionals') THEN
        CREATE SCHEMA professionals;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'patients') THEN
        CREATE SCHEMA patients;
    END IF;
END $EF$;

CREATE TYPE patients.birth_genres AS ENUM ('male', 'female');
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE patients.patients (
    id bigint GENERATED ALWAYS AS IDENTITY,
    name character varying(60) NOT NULL,
    birth_date date NOT NULL,
    birth_genre patients.birth_genres NOT NULL,
    external_id uuid NOT NULL,
    CONSTRAINT "PK_patients" PRIMARY KEY (id)
);

CREATE TABLE professionals.professionals (
    id bigint GENERATED ALWAYS AS IDENTITY,
    name character varying(60) NOT NULL,
    external_id uuid NOT NULL,
    CONSTRAINT "PK_professionals" PRIMARY KEY (id)
);

CREATE TABLE assessments.body_assessments (
    id bigint GENERATED ALWAYS AS IDENTITY,
    age integer NOT NULL,
    birth_genre patients.birth_genres NOT NULL,
    height numeric(3,2) NOT NULL,
    weight numeric(5,2) NOT NULL,
    assessment_date timestamptz NOT NULL DEFAULT (now()),
    external_id uuid NOT NULL,
    patient_id bigint NOT NULL,
    professional_id bigint NOT NULL,
    CONSTRAINT "PK_body_assessments" PRIMARY KEY (id),
    CONSTRAINT "FK_body_assessments_patients" FOREIGN KEY (patient_id) REFERENCES patients.patients (id) ON DELETE CASCADE,
    CONSTRAINT "FK_body_assessments_professionals" FOREIGN KEY (professional_id) REFERENCES professionals.professionals (id) ON DELETE CASCADE
);

CREATE TABLE assessments.body_assessment_skin_folds (
    id bigint GENERATED ALWAYS AS IDENTITY,
    triceps numeric(5,2) NOT NULL,
    biceps numeric(5,2) NOT NULL,
    subscapular numeric(5,2) NOT NULL,
    suprailiac numeric(5,2) NOT NULL,
    median_axillary numeric(5,2) NOT NULL,
    thoracic numeric(5,2) NOT NULL,
    supraspinal numeric(5,2) NOT NULL,
    thigh numeric(5,2) NOT NULL,
    abdomen numeric(5,2) NOT NULL,
    calf numeric(5,2) NOT NULL,
    assessment_id bigint NOT NULL,
    CONSTRAINT "PK_body_assessment_skin_folds" PRIMARY KEY (id),
    CONSTRAINT "FK_body_assessment_skin_folds" FOREIGN KEY (assessment_id) REFERENCES assessments.body_assessments (id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_body_assessment_skin_folds_assessment_id" ON assessments.body_assessment_skin_folds (assessment_id);

CREATE INDEX "IX_body_assessments_patient_id" ON assessments.body_assessments (patient_id);

CREATE INDEX "IX_body_assessments_professional_id" ON assessments.body_assessments (professional_id);

CREATE OR REPLACE VIEW assessments.vw_assessments_brief AS
SELECT
    ba.external_id AS id,
    ba.assessment_date AS date,
    ba.weight AS weight,
    p1.name AS professional_name,
    p2.external_id AS patient_external_id
FROM assessments.body_assessments ba
INNER JOIN professionals.professionals p1 ON p1.id = ba.professional_id
INNER JOIN patients.patients p2 on p2.id = ba.patient_id
ORDER BY ba.assessment_date;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251027124121_Initial', '8.0.20');

COMMIT;