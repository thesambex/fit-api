CREATE VIEW assessments.vw_assessments_brief AS
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