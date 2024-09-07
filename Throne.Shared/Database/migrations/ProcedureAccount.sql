CREATE OR REPLACE PROCEDURE insert_account(
    p_email VARCHAR,
    p_password VARCHAR,
    p_character_count INTEGER DEFAULT 2,
    p_is_admin BOOLEAN DEFAULT FALSE,
    p_is_vip BOOLEAN DEFAULT FALSE,
    p_coins INTEGER DEFAULT 0,
    p_character_id INTEGER DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO accounts (email, password, character_count, is_admin, is_vip, coins, character_id)
    VALUES (p_email, p_password, p_character_count, p_is_admin, p_is_vip, p_coins, p_character_id);
END;
$$;