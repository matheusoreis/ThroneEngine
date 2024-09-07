CREATE OR REPLACE PROCEDURE insert_character(
    p_name VARCHAR,
    p_color VARCHAR DEFAULT '#ffffff',
    p_gender_id INTEGER DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO characters (name, color, gender_id)
    VALUES (p_name, p_color, p_gender_id);
END;
$$;