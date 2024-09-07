CREATE OR REPLACE PROCEDURE insert_gender(
    p_name VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO genders (name)
    VALUES (p_name);
END;
$$;
